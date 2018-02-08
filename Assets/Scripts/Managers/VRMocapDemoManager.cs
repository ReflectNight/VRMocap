using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMocapDemoManager : MonoBehaviour {
    public static VRMocapDemoManager instance;

    private VRMocapState _currentState;
    public AvatarRecorder recorder;
    public string recordingFilePath;

    private AnimationType _currentType = AnimationType.NONE;

    public GhostController[] ghosts;

    [SerializeField]
    private TextMesh recordingLabel;
    [SerializeField]
    private TextMesh selectedLabel;

    void Awake() {
        instance = this;

        //Toggle recording state.
        WandController.GeneralTriggerClicked += ToggleRecording;

        //Toggle experience state.
        StateButton.OnButtonPress += ToggleState;

        //Register state switch functions to button press events. 
        RecordingAnimStateButton.OnButtonPress += UpdateAnimationType;
    }

    //Be able to switch between recording state and play state.
    public void ToggleState(VRMocapState state) {
        switch(state) {
            case VRMocapState.RECORDING:
                SwitchToRecordingState();
                break;
            case VRMocapState.FREE_REIGN:
                SwitchToFreeReignState();
                break;
            case VRMocapState.FOLLOW:
                SwitchToFollowState();
                break;
            default: break;
        }
    }


    public void SwitchToRecordingState() {
        _currentState = VRMocapState.RECORDING;
    }

    public void SwitchToFreeReignState() {
        _currentState = VRMocapState.FREE_REIGN;

        //Load up the new animation clips into the animator.
        //TODO: Update all known ghost animators.
        for(int i = 0; i < ghosts.Length; i++) {
            UpdateAnimationClips(ghosts[i].animator);
            ghosts[i].ResetGhostController();
        }
    }

    public void SwitchToFollowState() {

    }

    protected AnimatorOverrideController animatorOverrideController;
    protected AnimationClipOverrides clipOverrides;

    public void UpdateAnimationClips(Animator animator) {
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);
        
        clipOverrides["idle"] = Resources.Load<AnimationClip>(recordingFilePath + "idle_1");
        clipOverrides["walk"] = Resources.Load<AnimationClip>(recordingFilePath + "walk_1");
        clipOverrides["run"] = Resources.Load<AnimationClip>(recordingFilePath + "run_1");
        clipOverrides["jump"] = Resources.Load<AnimationClip>(recordingFilePath + "jump_1");
        clipOverrides["free1"] = Resources.Load<AnimationClip>(recordingFilePath + "free1_1");
        clipOverrides["free2"] = Resources.Load<AnimationClip>(recordingFilePath + "free2_1");
        clipOverrides["free3"] = Resources.Load<AnimationClip>(recordingFilePath + "free3_1");
        animatorOverrideController.ApplyOverrides(clipOverrides);
    }

    #region RECORDING BEHAVIOR
    bool isRecording = false;

    public void ToggleRecording() {
        if (isRecording) EndRecording();
        else StartRecording();
    }

    void StartRecording() {
        if(_currentState == VRMocapState.RECORDING) {
            if (recorder != null) {
                isRecording = true;
                recorder.BeginRecording();
                recordingLabel.text = "Recording Status: \nON";
            }
        }
    }

    void EndRecording() {
        if (_currentState == VRMocapState.RECORDING) {
            if (recorder != null) {
                isRecording = false;
                recorder.EndRecording("Assets/Resources/" + recordingFilePath, GetName());
                recordingLabel.text = "Recording Status: \nOFF";
            }
        }
    }

    public void UpdateAnimationType(AnimationType type) {
        _currentType = type;
        selectedLabel.text = "Selected: " + GetName();
    }

    string GetName() {
        string name;

        switch(_currentType) {
            case AnimationType.IDLE:
                name = "idle";
                break;
            case AnimationType.WALK:
                name = "walk";
                break;
            case AnimationType.RUN:
                name = "run";
                break;
            case AnimationType.JUMP:
                name = "jump";
                break;
            case AnimationType.FREE_1:
                name = "free1";
                break;
            case AnimationType.FREE_2:
                name = "free2";
                break;
            case AnimationType.FREE_3:
                name = "free3";
                break;
            default:
                name = "test";
                break;
        }

        return name;
    }
    #endregion

    #region FREE REIGN BEHAVIOR
    #endregion

    #region FOLLOW BEHAVIOR
    #endregion
}
