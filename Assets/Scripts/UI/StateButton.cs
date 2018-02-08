using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateButton : GeneralButton {
    public delegate void ButtonPressed(VRMocapState state);
    public static event ButtonPressed OnButtonPress;
    public VRMocapState state;

    public override void OnPress() {
        Debug.Log("Button pressed! " + state);

        base.OnPress();

        //Change state of VRMocapDemoManager.
        if (OnButtonPress != null) OnButtonPress(state);
    }
}
