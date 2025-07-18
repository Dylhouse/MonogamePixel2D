using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using System.Xml.Linq;

namespace MonoGamePixel2D.Assets.Graphics.Animations;

/// <summary>
/// A drawable object that contains multiple frames and and frame sections for animations.
/// </summary>
public class AnimatedSprite : IUpdatable, ISpriteSheetAsset, ILoadableAsset
{
    private const string PREFIX = "anim";
    internal const string DEFAULT_SECTION_NAME = "default";

    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private Point _sourceRectOffset;

    private int _direction = 1;

    private double _frameProgress;

    private Frame _frame;
    private readonly Frame[] _frames;

    private AnimationSection DefaultSection => _sections["default"];

    private AnimationSection _section;
    private readonly Dictionary<string, AnimationSection> _sections;

    private bool _needsToRestart = false;

    /// <inheritdoc></inheritdoc>/>
    public static string Prefix => PREFIX;

    /// <inheritdoc/>
    public Texture2D Texture { get; set; }

    /// <summary>
    /// Returns whether the animation is playing and advancing frames.
    /// Use <c>Play</c> to begin playback.
    /// </summary>
    public bool IsPlaying { get; private set; }

    /// <summary>
    /// Determines whether the animation will loop (as determined by AnimationDirection)
    /// or not once it reaches the final frame.
    /// </summary>
    public bool Looping { get; set; } = false;

    /// <summary>
    /// The speed factor at which the animation will play. Defaults to 1.0 (regular playback speed).
    /// </summary>
    public double Speed
    {
        get { return _speed; }
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Speed must be positive");
            }
            else _speed = value;
        }
    }
    private double _speed = 1.0d;

    /// <summary>
    /// Gets the current frame index.
    /// </summary>
    public int FrameIndex { get; private set; }

    /// <summary>
    /// The progress (in milliseconds) of the current frame. The frame will advance
    /// once this is greater than or equal to <c>FrameDuration</c>.
    /// </summary>
    public double FrameProgress => _frameProgress;

    /// <summary>
    /// The duration (in milliseconds) of the current frame. The frame will advance
    /// once <c>FrameProgress</c> is greater than or equal to this.
    /// </summary>
    public int FrameDuration => _frame.Duration;

    /// <summary>
    /// Returns the source rectangle of the current frame.
    /// </summary>
    public Rectangle FrameSourceRectangle => _frame.SourceRectangle;

    /// <summary>
    /// The <c>AnimationDirection</c> that will be used if no <c>AnimationSection</c> is given when
    /// <c>Play</c> is called. Defaults to <c>AnimationDirection.Forward</c>.
    /// </summary>
    public AnimationDirection DefaultDirection
    {
        get => DefaultSection.Direction;
        set => DefaultSection.Direction = value;
    }

    /// <summary>
    /// Invoked whenever the frame changes. Can be used to reduce the amount of draw calls by
    /// only drawing when/if the frame changes.
    /// </summary>
    public event Action? FrameChanged;

    /// <summary>
    /// Invoked whenever the animation reaches the end of the current section.
    /// </summary>
    public event Action? Finished;

    #region Constructors

    /// <inheritdoc></inheritdoc>/>
    public static object Load(ContentManager content, string contentPath, string path)
    {
        var animJson = File.ReadAllText(Path.ChangeExtension(path, ".json"));
        var dto = JsonSerializer.Deserialize<AnimatedSpriteDTO>(animJson, jsonOptions);
        var texture = content.Load<Texture2D>(contentPath);

        return LoadWithDTO(texture, dto);
    }

    internal static AnimatedSprite LoadWithDTO(Texture2D texture, AnimatedSpriteDTO DTO)
    {
        return new AnimatedSprite(texture, DTO.Frames, DTO.Sections);
    }

    public AnimatedSprite(Texture2D texture, Frame[] frames, AnimationSection[] sections, Point offset = default)
    {
        Texture = texture;
        _sourceRectOffset = offset;

        _frames = frames;
        _frame = frames[0];

        var sectionsDict = sections.ToDictionary(section => section.Name, section => section);

        sectionsDict.Add("default", new AnimationSection(0, frames.Length - 1));
        _sections = sectionsDict;

        ReadyAnimationSection("default");
    }

    public AnimatedSprite(Texture2D texture, Frame[] frames, Point offset = default)
    {
        Texture = texture;
        _sourceRectOffset = offset;

        this._frames = frames;
        _frame = frames[0];

        var sectionsDict = new Dictionary<string, AnimationSection>
        {
            { "default", new AnimationSection(0, frames.Length - 1) }
        };

        _sections = sectionsDict;
    }

    #endregion

    /// <summary>
    /// Creates a <see cref="StaticSprite"/> representation of the given <paramref name="frame"/>.
    /// </summary>
    /// <param name="frame">The requested frame.</param>
    /// <returns>The sprite of the given <paramref name="frame"/>.</returns>
    public StaticSprite GetSprite(int frame) =>
        new(Texture, _frames[frame].SourceRectangle);

    /// <summary>
    /// Creates a <see cref="StaticSprite"/> representation of the current frame.
    /// </summary>
    /// <returns>The sprite of the current frame.</returns>
    public StaticSprite GetSprite() =>
        new(Texture, _frame.SourceRectangle);

    /// <summary>
    /// Causes the animation to start playing using the given animation section. Does <b>not</b> reset
    /// frame progress.
    /// </summary>
    /// <param name="sectionName">The section of the animation to play. Use "default" 
    /// to play the entire animation forward. </param>
    public void Play(string sectionName)
    {
        ReadyAnimationSection(sectionName);
        Play();
    }

    /// <summary>
    /// Causes the animation to start playing.
    /// </summary>
    public void Play()
    {
        if (_needsToRestart) SetAbsoluteFrame(_section.StartIndex);
        IsPlaying = true;
    }

    /// <summary>
    /// Pauses the animation, but maintains everything else as if the game froze.
    /// </summary>
    public void Pause()
    {
        IsPlaying = false;
    }

    /// <summary>
    /// Stops the animation, resets frame progress, and sets it to its first frame.
    /// </summary>
    public void Reset()
    {
        IsPlaying = false;
        SetAbsoluteFrame(0);
    }

    /// <summary>
    /// Sets the absolute frame, disregarding any sections.
    /// </summary>
    /// <param name="index">The index of the new frame.</param>
    public void SetAbsoluteFrame(int index)
    {
        try
        {
            _frame = _frames[index];
        }

        catch (IndexOutOfRangeException e)
        {
            throw new ArgumentOutOfRangeException(
                "The given frame index is invalid.", e);
        }

        FrameIndex = index;
    }

    /// <summary>
    /// Animates the animation; increases animation progress and updates the frame. 
    /// The animation will not animate if this is not called.
    /// </summary>
    /// <param name="gameTime"><inheritdoc></inheritdoc></param>
    public void Update(GameTime gameTime)
    {
        if (!IsPlaying) return;
        _frameProgress += gameTime.ElapsedGameTime.TotalMilliseconds * _speed;
        TrySetNextFrame();
    }

    /// <summary>
    /// Sets <see cref="FrameProgress"/> to 0.
    /// </summary>
    public void ResetFrameProgress()
    {
        _frameProgress = 0;
    }

    #region IComplexDrawable
    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color)
    {
        var frameRect = _frame.SourceRectangle;
        frameRect.Location += _sourceRectOffset;
        spriteBatch.Draw(Texture, destinationRectangle, _frame.SourceRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
    {
        var frameRect = _frame.SourceRectangle;
        frameRect.Location += _sourceRectOffset;
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, destinationRectangle, _frame.SourceRectangle, color);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(_frame.SourceRectangle, sourceRectangle!.Value);
        spriteBatch.Draw(Texture, destinationRectangle, srcRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color,
        float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
    {
        var frameRect = _frame.SourceRectangle;
        frameRect.Location += _sourceRectOffset;
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, destinationRectangle, _frame.SourceRectangle, color, rotation, origin, effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(_frame.SourceRectangle, sourceRectangle!.Value);
        spriteBatch.Draw(Texture, destinationRectangle, srcRectangle, color, rotation, origin, effects, layerDepth);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        var frameRect = _frame.SourceRectangle;
        frameRect.Location += _sourceRectOffset;
        spriteBatch.Draw(Texture, position, _frame.SourceRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Rectangle? sourceRectangle, Color color)
    {
        var frameRect = _frame.SourceRectangle;
        frameRect.Location += _sourceRectOffset;
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position, _frame.SourceRectangle, color);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(_frame.SourceRectangle, sourceRectangle!.Value);
        spriteBatch.Draw(Texture, position, srcRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Rectangle? sourceRectangle, Color color,
        float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        var frameRect = _frame.SourceRectangle;
        frameRect.Location += _sourceRectOffset;
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position, _frame.SourceRectangle, color, rotation, origin, scale, effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(_frame.SourceRectangle, sourceRectangle!.Value);
        spriteBatch.Draw(Texture, position, srcRectangle, color, rotation, origin, scale, effects, layerDepth);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Rectangle? sourceRectangle, Color color,
        float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        var frameRect = _frame.SourceRectangle;
        frameRect.Location += _sourceRectOffset;
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position, _frame.SourceRectangle, color, rotation, origin, scale, effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(_frame.SourceRectangle, sourceRectangle!.Value);
        spriteBatch.Draw(Texture, position, srcRectangle, color, rotation, origin, scale, effects, layerDepth);
    }

    public void GetData(Color[] data)
    {
        var scrRect = _frame.SourceRectangle;
        var size = scrRect.Width * scrRect.Height;
        Texture.GetData(0, scrRect, data, 0, size);
    }
    #endregion
    private void Loop()
    {
        switch (_section.Direction)
        {
            case AnimationDirection.Forward:
                FrameIndex = _section.StartIndex;
                break;

            case AnimationDirection.Reverse:
                FrameIndex = _section.EndIndex;
                break;

            case AnimationDirection.PingPong or AnimationDirection.ReversePingPong:
                if (_direction == 1)
                {
                    FrameIndex = _section.EndIndex - 1;
                    _direction = -1;
                }

                else
                {
                    FrameIndex = _section.StartIndex + 1;
                    _direction = 1;
                }
                break;


        }
    }

    private void TrySetNextFrame()
    {
        while (_frameProgress >= _frame.Duration)
        {
            _frameProgress -= _frame.Duration;

            // Checking if should loop
            // greater than is uneccessary, only equal to is necessary, but we cover
            // our bases just in case.
            if (_direction == 1 && FrameIndex >= _section.EndIndex || _direction == -1 && FrameIndex <= _section.StartIndex)
            {
                if (Looping) Loop();
                else
                {
                    _needsToRestart = true;
                    Pause();
                    ResetFrameProgress();
                }

                Finished?.Invoke();
            }
            else FrameIndex += _direction;

            FrameChanged?.Invoke();
            _frame = _frames[FrameIndex];
        }
    }

    private void SetInitialDirection() => _direction = _section.Direction switch
    {
        AnimationDirection.Forward or AnimationDirection.PingPong => 1,
        _ => -1
    };

    /// <summary>
    /// Gets the animation ready to play with an <see cref="AnimationSection"/>.
    /// </summary>
    /// <param name="sectionName">The name of the <see cref="AnimationSection"/>.</param>
    private void ReadyAnimationSection(string sectionName)
    {
        _section = _sections[sectionName];
        SetInitialDirection();
        SetAbsoluteFrame(_section.StartIndex);
    }

    public static object Load(Texture2D atlasTexture, Rectangle sourceRectangle, string path)
    {
        var animJson = File.ReadAllText(Path.ChangeExtension(path, ".json"));
        var dto = JsonSerializer.Deserialize<AnimatedSpriteDTO>(animJson, jsonOptions);

        return new AnimatedSprite(atlasTexture, dto.Frames, dto.Sections, sourceRectangle.Location);
    }

    public static void Unload(ContentManager content, string contentPath) =>
        content.UnloadAsset(contentPath);
}