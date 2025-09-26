// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;

namespace GA.GArkanoid;

public static class Config
{
    public static StringName MoveRightAction = "MoveRight";
    public static StringName MoveLeftAction = "MoveLeft";
    public static StringName LaunchAction = "LaunchBall";

    #region Initial Ball Settings
    public const float BallSpeed = 250.0f;
    public static Vector2 BallDirection = new Vector2(1, -1).Normalized();
    #endregion
}
