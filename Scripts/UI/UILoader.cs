// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using GA.GArkanoid.Systems;
using GA.GArkanoid.States;
using Godot;
using System;

namespace GA.GArkanoid.UI;
public partial class UILoader : Control
{
    [Export] private StateType _initialStateType = StateType.MainMenu;

    public override void _Ready()
    {
        GameManager.Instance.ChangeState(_initialStateType);
        QueueFree();
    }

}
