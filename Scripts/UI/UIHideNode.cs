using GA.GArkanoid.States;
using GA.GArkanoid.Systems;
using Godot;
using Godot.Collections;

namespace GA.GArkanoid.UI;
public partial class UIHideNode : Node
{
    [Export] private Array<StateType> _enableStates = null;

    public override void _Ready()
    {
        Control parent = GetParent<Control>();
        bool isEnabled = _enableStates.Contains(GameManager.Instance.ActiveState.StateType);
        EnableParent(parent, isEnabled);
    }
    
    private void EnableParent(Control parent, bool enable)
    {
        if (parent == null)
        {
            return;
        }

        parent.Visible = enable;
    }

}
