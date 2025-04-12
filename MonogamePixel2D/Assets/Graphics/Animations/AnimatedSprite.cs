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
    public bool Looping { get; set; }

    private double speed = 1.0d;

    /// <summary>
    /// The speed factor at which the animation will play. Defaults to 1.0 (regular playback speed).
    /// </summary>
    public double Speed
    {
        get { return speed; }
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Speed must be positive");
            }
            else speed = value;
        }
    }

    /// <summary>
    /// The current frame index. Use SetFrame to change it.
    /// </summary>
    public int FrameIndex { get; private set; }

    /// <summary>
    /// The progress (in milliseconds) of the current frame. The frame will advance
    /// once this is greater than or equal to <c>FrameDuration</c>.
    /// </summary>
    public double FrameProgress { get; private set; }

    /// <summary>
    /// The duration (in milliseconds) of the current frame. The frame will advance
    /// once <c>FrameProgress</c> is greater than or equal to this.
    /// </summary>
    public int FrameDuration { get => frame.Duration; }

    /// <summary>
    /// Returns the source rectangle of the current frame.
    /// </summary>
    public Rectangle FrameSourceRectangle { get => frame.SourceRectangle; }

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

    private int direction = 1;

    private Frame frame;
    private readonly Frame[] frames;

    private AnimationSection DefaultSection => sections["default"];

    private AnimationSection section;
    private readonly Dictionary<string, AnimationSection> sections;

    #region Constructors

    internal static AnimatedSprite LoadWithDTO(Texture2D texture, AnimatedSpriteDTO DTO)
    {
        return new AnimatedSprite(texture, DTO.Frames, DTO.Sections);
    }

    public AnimatedSprite(Texture2D texture, Frame[] frames, AnimationSection[] sections)
    {
        Texture = texture;

        this.frames = frames;
        frame = frames[0];

        var sectionsDict = sections.ToDictionary(section => section.Name, section => section);

        sectionsDict.Add("default", new AnimationSection(0, frames.Length - 1));
        this.sections = sectionsDict;

        ReadyAnimation("default");
    }

    public AnimatedSprite(Texture2D texture, Frame[] frames)
    {
        Texture = texture;

        this.frames = frames;
        frame = frames[0];

        var sectionsDict = new Dictionary<string, AnimationSection>
        {
            { "default", new AnimationSection(0, frames.Length - 1) }
        };

        sections = sectionsDict;
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
        ReadyAnimation(sectionName);
        Play();
    }

    /// <summary>
    /// Causes the animation to start playing.
    /// </summary>
    public void Play()
    {
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
    public void Stop()
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
            frame = frames[index];
        }

        catch (IndexOutOfRangeException e)
        {
            throw new ArgumentOutOfRangeException(
                "The given frame index is invalid.", e);
        }

        FrameIndex = index;
    }

    public void Update(GameTime gameTime)
    {
        if (!IsPlaying) return;
        FrameProgress += gameTime.ElapsedGameTime.TotalMilliseconds * speed;
        TrySetNextFrame();
    }

    /// <summary>
    /// Sets <c>FrameProgress</c> to 0.
    /// </summary>
    public void ResetFrameProgress()
    {
        FrameProgress = 0;
    }

    #region IComplexDrawable
    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color)
    {
        spriteBatch.Draw(Texture, destinationRectangle, frame.SourceRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
    {
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, destinationRectangle, frame.SourceRectangle, color);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(frame.SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, destinationRectangle, srcRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, SpriteEffects effects, float layerDepth)
    {
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, destinationRectangle, frame.SourceRectangle, color, rotation, origin.ToVector2(), effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(frame.SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, destinationRectangle, srcRectangle, color, rotation, origin.ToVector2(), effects, layerDepth);
    }

    public void Draw(SpriteBatch spriteBatch, Point position, Color color)
    {
        spriteBatch.Draw(Texture, position.ToVector2(), frame.SourceRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color)
    {
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position.ToVector2(), frame.SourceRectangle, color);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(frame.SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, position.ToVector2(), srcRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position.ToVector2(), frame.SourceRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(frame.SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, position.ToVector2(), srcRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
    }

    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, float scale, SpriteEffects effects, float layerDepth)
    {
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position.ToVector2(), frame.SourceRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(frame.SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, position.ToVector2(), srcRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
    }

    public void GetData(Color[] data)
    {
        var scrRect = frame.SourceRectangle;
        var size = scrRect.Width * scrRect.Height;
        Texture.GetData(0, scrRect, data, 0, size);
    }
    #endregion
    private void Loop()
    {
        switch (section.Direction)
        {
            case AnimationDirection.Forward:
                FrameIndex = section.StartIndex;
                break;

            case AnimationDirection.Reverse:
                FrameIndex = section.EndIndex;
                break;

            case AnimationDirection.PingPong or AnimationDirection.ReversePingPong:
                if (direction == 1)
                {
                    FrameIndex = section.EndIndex - 1;
                    direction = -1;
                }

                else
                {
                    FrameIndex = section.StartIndex + 1;
                    direction = 1;
                }
                break;


        }
    }

    private void TrySetNextFrame()
    {
        while (FrameProgress >= frame.Duration)
        {
            FrameProgress -= frame.Duration;

            // Checking if should loop
            // greater than is uneccessary, only equal to is necessary, but we cover
            // our bases just in case.
            if (direction == 1 && FrameIndex >= section.EndIndex || direction == -1 && FrameIndex <= section.StartIndex) Loop();
            else FrameIndex += direction;

            FrameChanged?.Invoke();
            frame = frames[FrameIndex];
        }
    }

    private void SetInitialDirection() => direction = section.Direction switch
    {
        AnimationDirection.Forward or AnimationDirection.PingPong => 1,
        _ => -1
    };

    /// <summary>
    /// Gets the animation ready to play with an animation section.
    /// </summary>
    /// <param name="sectionName"></param>
    private void ReadyAnimation(string sectionName)
    {
        section = sections[sectionName];
        SetInitialDirection();
        SetAbsoluteFrame(section.StartIndex);
    }

}