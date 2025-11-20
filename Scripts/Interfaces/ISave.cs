using Godot.Collections;

namespace GA.GArkanoid.Save;
public interface ISave
{
    Dictionary Save();
    void Load(Dictionary data);
}
