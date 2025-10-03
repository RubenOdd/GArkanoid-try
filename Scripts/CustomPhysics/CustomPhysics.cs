// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;

namespace GA.Garkanoid;

public static class CustomPhysics
{
    public static void PointVsRect(Vector2 rect, Vector2 point)
    {

    }
}

/// <summary>
/// Store information about collision between two objects
/// </summary>
public class Hit
{
    public Vector2 Point { get; set; }
    public Vector2 Normal { get; set; }
    public Vector2 Delta { get; set; }
}