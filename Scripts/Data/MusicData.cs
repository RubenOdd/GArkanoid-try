using Godot;
using GA.GArkanoid.States;

namespace GA.GArkanoid.Data;
[GlobalClass, Tool]
public partial class MusicData : Resource
{
    [Export] public StateMusicMap[] StateMusicMapping { get; set; }

    /// <summary>
    /// Gets the music track for a specific game state.
    /// </summary>
    /// <param name="stateType">The state to get music for.</param>
    /// <returns>The AudioStream for the state, or null if no music is mapped.</returns>
    public AudioStream GetMusicForState(StateType stateType)
    {
        foreach (StateMusicMap map in StateMusicMapping)
        {
            if (map.StateType == stateType)
            {
                return map.MusicTrack;
            }
        }

        return null;
    }
}