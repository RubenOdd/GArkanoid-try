// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;
using GA.GArkanoid.States;

namespace GA.GArkanoid.Data;

[GlobalClass, Tool]
public partial class StateMusicMap : Resource
{
    [Export] public StateType StateType {get; set;}
    [Export] public AudioStream MusicTrack {get; set;}
}