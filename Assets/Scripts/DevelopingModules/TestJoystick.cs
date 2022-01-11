using UnityEngine;
using UnityEngine.EventSystems;

using DefenceGameSystem.OS.Kernel;

public class TestJoystick : CustomButton
{
    public string axisName;
    public AxisType axis;

    public override void Bind(KeyType key)
    {
        base.Bind(key);
        SetAxis();
    }

    private void SetAxis()
    {
        if(axisName == null || axisName == "") return;
        if(axis == default(AxisType) || axis == AxisType.None) return;

        InputModule.AddAxis(this, axisName, axis);
    }
}