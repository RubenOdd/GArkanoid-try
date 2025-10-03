// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;
using GA.Common;

public partial class CustomBall : Sprite2D
{
    /// <summary>
    /// THe movement direction. This should always be normalized.
    /// </summary>
    [Export] private Vector2 _direction = Vector2.Zero;
    /// <summary>
    /// The movement speed in pixel per second. (pixels / second).
    /// </summary>
    [Export] private float _speed = 100.0f;
    /// <summary>
    /// Radius of the ball.
    /// </summary>
    [Export] private float _radius = 16.0f;
    [Export] private Sprite2D[] _wall;
    public Vector2 Velocity
    {
        get { return _direction * _speed; }
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _direction = _direction.Normalized();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        //Move the ball my modifying it's GlobalPosition each frame.
        Vector2 movement = Velocity * (float)delta;
        Vector2 newPos = GlobalPosition + movement;

        // TODO: Calculate possible collisions with walls here.
        // TODO: Bounce the ball when collisions occurs.
        GlobalPosition = newPos;

    }

    /// <summary>
    /// Demonstration of extents
    /// </summary>Demo
    private void ExtenstionExamples()
    {
        Vector2 spriteSize = this.Texture.GetSize();
        Rect2 boundingBox = this.GetBoundingBox();

        Vector2 extents = boundingBox.GetExtents();
    }
}
