using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePixel2D.Assets.Graphics.Animations;

/// <summary>
/// A drawable object that contains multiple frames and and frame sections for animations.
/// </summary>
public class AnimatedSprite : IComplexDrawable, IUpdatable
{
    internal const string DEFAULT_SECTION_NAME = "default";

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
    /// The current frame index. Use <see cref="SetAbsoluteFrame(int)"/> to change it.
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

    private int _direction = 1;

    private double _frameProgress;

    private Frame _frame;
    private readonly Frame[] _frames;

    private AnimationSection DefaultSection => _sections["default"];

    private AnimationSection _section;
    private readonly Dictionary<string, AnimationSection> _sections;

    private bool _needsToRestart = false;

    #region Constructors

    internal static AnimatedSprite LoadWithDTO(Texture2D texture, AnimatedSpriteDTO DTO)
    {
        return new AnimatedSprite(texture, DTO.Frames, DTO.Sections);
    }

    public AnimatedSprite(Texture2D texture, Frame[] frames, AnimationSection[] sections)
    {
        Texture = texture;

        this._frames = frames;
        _frame = frames[0];

        var sectionsDict = sections.ToDictionary(section => section.Name, section => section);

        sectionsDict.Add("default", new AnimationSection(0, frames.Length - 1));
        this._sections = sectionsDict;

        ReadyAnimationSection("default");
    }

    public AnimatedSprite(Texture2D texture, Frame[] frames)
    {
        Texture = texture;

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
    public void SetAbsoluteFrame(System.Index index)
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

        FrameIndex = index.Value;
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
        spriteBatch.Draw(Texture, destinationRectangle, _frame.SourceRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
    {
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, destinationRectangle, _frame.SourceRectangle, color);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(_frame.SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, destinationRectangle, srcRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, SpriteEffects effects, float layerDepth)
    {
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, destinationRectangle, _frame.SourceRectangle, color, rotation, origin.ToVector2(), effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(_frame.SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, destinationRectangle, srcRectangle, color, rotation, origin.ToVector2(), effects, layerDepth);
    }

    public void Draw(SpriteBatch spriteBatch, Point position, Color color)
    {
        spriteBatch.Draw(Texture, position.ToVector2(), _frame.SourceRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color)
    {
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position.ToVector2(), _frame.SourceRectangle, color);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(_frame.SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, position.ToVector2(), srcRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position.ToVector2(), _frame.SourceRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(_frame.SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, position.ToVector2(), srcRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
    }

    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, float scale, SpriteEffects effects, float layerDepth)
    {
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position.ToVector2(), _frame.SourceRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(_frame.SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, position.ToVector2(), srcRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
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

}