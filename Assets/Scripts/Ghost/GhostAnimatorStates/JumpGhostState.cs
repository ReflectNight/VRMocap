using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGhostState : GeneralGhostState {
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //Apply force to rigidbody.
    }
}