// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;
using System;

namespace GA.GArkanoid;
public partial class Wall : Node2D
{
    [Export]
    public bool IsHazard = false;
}
