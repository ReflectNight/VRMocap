using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralGhostState : StateMachineBehaviour {
    public GhostController ghostController;
    public GhostState state = GhostState.NONE;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        ghostController.currentState = state;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
