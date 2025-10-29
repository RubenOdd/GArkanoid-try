// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using GA.GArkanoid.States;
using GA.GArkanoid.Systems;
using Godot;
using System;

namespace GA.GArkanoid.UI;
public partial class UISettings : Control
{
    [Export] private Button _closeButton = null;

    public override void _Ready()
    {
        _closeButton.Pressed += OnClose;
    }

    private void OnClose()
    {
        GameManager.Instance.ChangeState(StateType.MainMenu);
    }

}
