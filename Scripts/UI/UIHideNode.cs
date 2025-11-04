using GA.GArkanoid.States;
using GA.GArkanoid.Systems;
using Godot;
using System;

namespace GA.GArkanoid.UI;
public partial class UIHideNode : Node
{
    public override void _EnterTree()
    {
        if (GameManager.Instance.ActiveState.StateType == StateType.Settings)
        {
            GetParent<Button>().Hide();
        }
    }

    public override void _ExitTree()
    {
        if (!GetParent<Button>().IsVisibleInTree())
        {
            GetParent<Button>().Show();
        }
    }


}
