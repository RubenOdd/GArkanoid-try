// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;

namespace GA.GArkanoid;

public partial class Paddle : CharacterBody2D
{
    [Export]
    public float Speed { get; set; } = 200.0f;
    private float _horizontalInput = 0.0f;
    private float _mouseInput = 0.0f;
    private Vector2 _screenSize;
    private Vector2 _paddleSize;

    // TODO: Create level manager to control level objects
    [Export]
    private Ball _ball = null;

    #region Public Interface
    public override void _Ready()
    {
        _screenSize = GetViewportRect().Size;
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
        if (_ball != null && !_ball.IsLaunched && @event.IsActionPressed(Config.LaunchAction))
        {
            _ball.Launch(Config.BallSpeed, Config.BallDirection);
        }
    }


    public override void _PhysicsProcess(double delta)
    {
        if (Mathf.IsZeroApprox(_mouseInput))
        {
            // if there is no mouse movement, call the horizontal axis movement
            Vector2 movement = new Vector2(_horizontalInput, 0.0f);
            movement *= Speed * (float)delta;
            MoveAndCollide(movement);

            // Fix for the controllers that shut off while inputting, so that the paddle stops
            _horizontalInput = 0.0f;
        }
        else
        {
            // ignores the paddle's Speed completely, to make the movement feel better
            Vector2 movement = new Vector2(_mouseInput, 0.0f);
            MoveAndCollide(movement);
            _mouseInput = 0.0f;
        }

        ClampToScreen();
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
    #endregion
}
