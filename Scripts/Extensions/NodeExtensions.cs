// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;

namespace GA.Common;

public static class NodeExtensions
{
    /// <summary>
    /// Returns the half of the rectangle size
    /// </summary>
    /// <param name="rectangle">The rectangle which extents are being calculated</param>
    /// <returns>Extents as Vector2</returns>
    public static Vector2 GetExtents(this Rect2 rectangle)
    {
        return rectangle.Size / 2;
    }

    public static Rect2 GetBoundingBox(this Sprite2D sprite)
    {
        // There's no texture , the resulting rect hsould be empty as well.
        if (sprite.Texture == null)
        {
            return new Rect2();
        }

        Vector2 textureSize = sprite.Texture.GetSize();

        // Scale the texture by sprite's scale
        // TODO: Should a local or Global scale be used?
        Vector2 scaledSize = textureSize * sprite.Scale;
        Vector2 offset = sprite.Offset;

        Vector2 topLeft = sprite.GlobalPosition -
            (scaledSize * (sprite.Centered ? 0.5f : 0.0f)) + offset;

        return new Rect2(topLeft, scaledSize);
    }
}
