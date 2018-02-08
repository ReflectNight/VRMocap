using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGhostState : GeneralGhostState {
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        Vector3 target = (Vector3)ghostController.currentAction.args[0];

        //If still far from target, move toward destination.
        if (Vector3.SqrMagnitude(target - ghostController.transform.position) > 1f) {
            ghostController.rigidbody.AddForce((target - ghostController.transform.position).normalized * Constants.MOVE_MULTIPLIER + new Vector3(0, 5f, 0));
            ghostController.transform.LookAt(new Vector3(target.x, ghostController.transform.position.y, target.z));
        }

        //Otherwise, tell controller that you've completed movement.
        else {
            ghostController.isExecutingAction = false;
        }

        //ghostController.animator.SetFloat("speed", ghostController.rigidbody.velocity.sqrMagnitude);
    }
}