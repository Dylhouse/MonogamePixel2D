using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGamePixel2D;
public class RectangleConverter : JsonConverter<Rectangle>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(Rectangle).IsAssignableFrom(typeToConvert);
    }   

    public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var json = JsonDocument.ParseValue(ref reader);
        int x = json.RootElement.GetProperty("x").GetInt32();
        int y = json.RootElement.GetProperty("y").GetInt32();
        int w = json.RootElement.GetProperty("w").GetInt32();
        int h = json.RootElement.GetProperty("h").GetInt32();
        
        return new Rectangle(x, y, w, h);
    }

    public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("x", value.X);
        writer.WriteNumber("y", value.Y);
        writer.WriteNumber("w", value.Width);
        writer.WriteNumber("h", value.Height);
        writer.WriteEndObject();
    }
}