// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;
using System;

namespace GA.GArkanoid;

public partial class Ball : CharacterBody2D
{
    public float Speed { get; private set; } = 0.0f;
    public Vector2 Direction { get; private set; } = Vector2.Zero;
    private CharacterBody2D _paddle = null;
    // HACK Very bad, bad, bad hard coded magic numbers
    private Vector2 _offset = new(0, -10);

    public bool IsLaunched
    {
        get { return !Direction.IsZeroApprox(); }
    }

    public override void _Ready()
    {
        Velocity = new Vector2(Speed * -1, Speed);
        _paddle = GetNode<CharacterBody2D>("/root/Level/Paddle");
    }


    public override void _PhysicsProcess(double delta)
    {
        // If the ball is not launched, exit early
        if (!IsLaunched)
        {
            if (_paddle == null) return;
            else
            {
                GlobalPosition = _paddle.GlobalPosition + _offset;
            }
        }

        var collisionInfo = MoveAndCollide(Velocity * (float)delta);

        if (collisionInfo != null)
        {
            Velocity = Velocity.Bounce(collisionInfo.GetNormal());
        }
    }

    /// <summary>
    /// Launches the ball in a certain direction.
    /// Precondition: Direction should be normalized before calling this method.
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="direction"></param>
    public void Launch(float speed, Vector2 direction)
    {
        Speed = speed;
        Direction = direction;
        Velocity = Speed * Direction;
    }

}
