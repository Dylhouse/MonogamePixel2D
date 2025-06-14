﻿using Microsoft.Xna.Framework;
namespace MonoGamePixel2D;



/// <summary>
/// A class for basic timers/stopwatches.
/// </summary>
public class Timer : IUpdatable
{
    /// <summary>
    /// The way the timer will (or won't) loop.
    /// </summary>
    public enum LoopMode
    {
        /// <summary>
        /// Will cause the timer to stop once it is finished.
        /// </summary>
        NoLoop,
        /// <summary>
        /// Will cause the timer to reset and continue once it is finished.
        /// </summary>
        Loop,
        /// <summary>
        /// Will cause the timer to continue once it is finished, while maintaining the remaining time.
        /// For example, if a frame lasts for 0.4 seconds and the timer has 0.1 seconds left of 1.0 duration,
        /// it will go to 0.7 seconds left. This can also lead to <c>Finished</c> being invoked multiple times
        /// in one frame.
        /// </summary>
        ContinousLoop
    }

    private TimeSpan _progress = TimeSpan.Zero;

    private bool _running = true;

    #region Properties
    /// <summary>
    /// Determines how the timer will (or won't) loop.
    /// </summary>
    public LoopMode Loop { get; set; }

    /// <summary>
    /// Returns the time elapsed since the timer started, or has looped
    /// </summary>
    public TimeSpan Progress => _progress;

    /// <summary>
    /// Returns the time left on the timer.
    /// </summary>
    public TimeSpan TimeLeft => Duration - _progress;

    /// <summary>
    /// Returns whether or not the timer has been started.
    /// </summary>
    public bool IsRunning => _running;

    /// <summary>
    /// The time the timer starts at, or how long it will need to run until it finishes.
    /// In seconds.
    /// </summary>
    public TimeSpan Duration { get; set; }

    #endregion

    /// <summary>
    /// Creates a new <c>Timer</c>.
    /// </summary>
    /// <param name="duration">The duration of the timer.</param>
    /// <param name="loopMode">Determines how the timer will or won't loop when it finishes.</param>
    public Timer(TimeSpan duration, LoopMode loopMode = LoopMode.NoLoop)
    {
        Duration = duration;
        Loop = loopMode;
    }

    /// <summary>
    /// Invoked when the timer reaches the end.
    /// </summary>
    public event Action? Finished;

    #region Public Methods
    /// <summary>
    /// Starts the timer.
    /// </summary>
    public void Start()
    {
        _running = true;
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    public void Stop()
    {
        _running = false;
    }

    /// <summary>
    /// Resets the timer.
    /// </summary>
    public void Reset()
    {
        _progress = TimeSpan.Zero;
    }
    #endregion

    /// <inheritdoc/>
    public void Update(GameTime gameTime)
    {
        if (!_running) return;

        _progress += gameTime.ElapsedGameTime;

        if (Loop == LoopMode.ContinousLoop)
        {
            while (_progress >= Duration)
            {
                _progress -= Duration;
                Finished?.Invoke();
            }
        }

        else
        {
            if (_progress >= Duration)
            {
                _progress = Duration;
                Finished?.Invoke();

                if (Loop == LoopMode.NoLoop) _running = false;
                else _progress = TimeSpan.Zero;
            }
        }
    }
}
