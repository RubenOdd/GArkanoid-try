// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;
using System;

namespace GA.GArcanoid;

public partial class Paddle : CharacterBody2D
{
    [Export]
    private float _speed = 300.0f;
    [Export]
    private float _ballSpeed = 100.0f;
    private Vector2 _screenSize;
    private Vector2 _paddleSize;

    public override void _Ready()
    {
        _screenSize = GetViewportRect().Size;
    }
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        float horizontal = Input.GetAxis("Move Left", "Move Right");

        _paddleSize = GetNode<Sprite2D>("CollisionShape2D/Sprite2D").GetRect().Size;


        if (Input.IsActionJustPressed("Launch Ball"))
        {
            GD.Print($"BALL is shot at {_ballSpeed} velocity");
        }

        if (horizontal != 0)
        {
            velocity.X = horizontal * _speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
        }

        Velocity = velocity;
        MoveAndSlide();
        ClampToScreen();
    }

    /// <summary>
    /// Used help from my classmate Joel
    /// </summary>
    private void ClampToScreen()
    {
        Vector2 scaledSize = _paddleSize + GlobalScale / 2;
        GlobalPosition = GlobalPosition.Clamp(
            GetViewportRect().Position + scaledSize,
            GetViewportRect().End - scaledSize
        );
    }
}
