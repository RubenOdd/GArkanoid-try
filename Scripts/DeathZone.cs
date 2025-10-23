// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using GA.GArkanoid.Systems;
using Godot;
using System;

namespace GA.GArkanoid;
public partial class DeathZone : Area2D
{
    public void OnZoneEntered(Node2D body)
    {
        GD.Print("wowowa");
        GameManager.Instance.DecreaseLives();
    }
}
