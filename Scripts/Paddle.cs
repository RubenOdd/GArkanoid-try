// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using System;
using GA.Common;
using GA.GArkanoid.Save;
using GA.GArkanoid.Systems;
using Godot;
using Godot.Collections;

namespace GA.GArkanoid;

public partial class Paddle : CharacterBody2D, ISave
{
    private float _horizontalInput = 0.0f;
    private float _mouseInput = 0.0f;
    private Vector2 _screenSize;
    private Vector2 _paddleSize;
    private float _speed = 0.0f;
    private float _width = 1.0f;
    private CollisionObject2D _collisionShape = null;

    public Ball CurrentBall { get { return LevelManager.Active.CurrentBall; }}

    public float Width
    {
        get => _width;
        set
        {
            _width = value;
            UpdatePaddleWidth();
        }
    }


    #region Public Interface
    public override void _Ready()
    {
        _screenSize = GetViewportRect().Size;
        _speed = GameManager.Instance.CurrentPlayerData.PaddleSpeed;
        _collisionShape = this.GetNode<CollisionObject2D>();
    }

    public override void _Process(double delta)
    {
        _paddleSize = GetNode<Sprite2D>("CollisionShape2D/Sprite2D").GetRect().Size;

        _horizontalInput = Input.GetAxis(Config.MoveLeftAction, Config.MoveRightAction);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            _mouseInput = mouseMotionEvent.Relative.X;
        }

        // If we have a ball, let it be launched by player
        if (CurrentBall != null && !CurrentBall.IsLaunched && @event.IsActionPressed(Config.LaunchAction))
        {
            CurrentBall.Launch(GameManager.Instance.CurrentPlayerData.BallSpeed,
					GameManager.Instance.CurrentPlayerData.LaunchDirection.Normalized());
        }
    }


    public override void _PhysicsProcess(double delta)
    {
        if (Mathf.IsZeroApprox(_mouseInput))
        {
            // if there is no mouse movement, call the horizontal axis movement
            Vector2 movement = new(_horizontalInput, 0.0f);
            movement *= _speed * (float)delta;
            MoveAndCollide(movement);

            // Fix for the controllers that shut off while inputting, so that the paddle stops
            _horizontalInput = 0.0f;
        }
        else
        {
            // ignores the paddle's Speed completely, to make the movement feel better
            Vector2 movement = new(_mouseInput, 0.0f);
            MoveAndCollide(movement);
            _mouseInput = 0.0f;
        }

        ClampToScreen();
    }

    public Dictionary Save()
    {
        Dictionary positionData = new Dictionary
        {
            {"X", GlobalPosition.X},
            {"Y", GlobalPosition.Y}
        };

        Dictionary paddleData = [];
        paddleData["Speed"] = _speed;
        paddleData["Position"] = positionData;

        return paddleData;
    }

    public void Load(Dictionary data)
    {
        _speed = (float)data["Speed"];

        Dictionary positionData = (Dictionary)data["Position"];
        GlobalPosition = new Vector2((float)positionData["X"], (float)positionData["Y"]);
    }

    public void Expand(float multiplyer)
    {
        Width *= multiplyer;
    }

    public void Shrink(float multiplyer)
    {
        Width *= 1 / multiplyer;
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Used help from my classmate Joel to understand how to math it properly
    /// </summary>
    private void ClampToScreen()
    {
        Vector2 scaledSize = _paddleSize + GlobalScale / 2;
        GlobalPosition = GlobalPosition.Clamp(
            GetViewportRect().Position + scaledSize,
            GetViewportRect().End - scaledSize
        );
    }

    private void UpdatePaddleWidth()
    {
        if (_collisionShape != null)
        {
            GD.Print("it did it");
            _collisionShape.Scale = new Vector2(Scale.X, _width);
        }
    }
    #endregion
}
