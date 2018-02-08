using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingAnimStateButton : GeneralButton {
    public delegate void ButtonPressed(AnimationType type);
    public static event ButtonPressed OnButtonPress;
    public AnimationType animationType;

    public override void Init() {
        base.Init();
    }

    public override void OnPress() {
        base.OnPress();

        //Change state of VRMocapDemoManager.
        if (OnButtonPress != null) OnButtonPress(animationType);
    }
}