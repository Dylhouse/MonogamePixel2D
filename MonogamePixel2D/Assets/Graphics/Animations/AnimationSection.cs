using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePixel2D.Assets.Graphics.Animations;

public class AnimationSection
{
    public string Name { get; init; }
    public int StartIndex { get; init; }
    public int EndIndex { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AnimationDirection Direction { get; set; }

    public AnimationSection(int startIndex, int endIndex, string name = "default", AnimationDirection direction = AnimationDirection.Forward)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
        Name = name;
        Direction = direction;
    }
}

public enum AnimationDirection
{
    Forward,
    Reverse,
    PingPong,
    ReversePingPong
}