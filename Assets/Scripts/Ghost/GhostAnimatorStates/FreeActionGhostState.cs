using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeActionGhostState : GeneralGhostState {
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateExit(animator, stateInfo, layerIndex);
        ghostController.isExecutingAction = false;
        ghostController.StartCoroutine(ghostController.collisionCheck());
    }
}