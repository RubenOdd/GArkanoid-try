// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using GA.GArkanoid.Systems;
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
        // If the ball is not launched and there is a paddle, stay with the paddle
        // Otherwise, exit early
        if (!IsLaunched)
        {
            if (_paddle == null) return;
            else
            {
                GlobalPosition = _paddle.GlobalPosition + _offset;
            }
        }

        var collisionInfo = MoveAndCollide(Velocity * (float)delta);

        // The bouncing mechanic
        if (Bounce(collisionInfo))
        {
            GodotObject collidedObject = collisionInfo.GetCollider();
            if (collidedObject is Block block)
            {
                LevelManager.Active.EffectPlayer.PlayEffect(EffectType.Hit, GlobalPosition);
                block.Hit();
            }
            else if (collidedObject is Wall wall && wall.IsHazard)
            {
                LevelManager.Active.EffectPlayer.PlayEffect(EffectType.Death, GlobalPosition);
                GameManager.Instance.DecreaseLives();
            }
            else
            {
                LevelManager.Active.EffectPlayer.PlayEffect(EffectType.Bounce, GlobalPosition);
            }
        }
    }

    private bool Bounce(KinematicCollision2D collisionInfo)
    {
        if (collisionInfo != null)
        {
            Direction = Direction.Bounce(collisionInfo.GetNormal()).Normalized();
            Velocity = Direction * Speed;
        }

        return collisionInfo != null;
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

    public void ResetBall()
    {
        Speed = 0;
        Direction = Vector2.Zero;
        Velocity = Vector2.Zero;
    }
}
