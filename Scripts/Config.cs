// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;

namespace GA.GArkanoid;

public static class Config
{
    public static StringName MoveRightAction = "MoveRight";
    public static StringName MoveLeftAction = "MoveLeft";
    public static StringName LaunchAction = "LaunchBall";
    public static StringName PauseAction = "Pause";

    #region Settings
    public static Vector2I Window640 = new(640, 360);
    public static Vector2I Window1280 = new(1280, 720);
    public static Vector2I Window1920 = new(1920, 1080);
    #endregion

    #region Initial Ball Settings
    public const float BallSpeed = 200.0f;
    public static Vector2 BallDirection = new Vector2(1, -1).Normalized();
    #endregion

    #region Player data
    public static int MaxLives = 100;
    public static int InitialLives = 3;
    public static int InitialScore = 0;
    #endregion Player data
}
