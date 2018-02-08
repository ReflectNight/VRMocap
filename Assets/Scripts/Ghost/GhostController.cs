using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GhostState {
    NONE = -1,
    IDLE,
    WALK,
    RUN,
    JUMP,
    ACT
}

public class GhostAIActionData {
    public GhostState state;
    public object[] args;

    public GhostAIActionData(GhostState state, params object[] args) {
        this.state = state;
        this.args = args;
    }
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GhostController : MonoBehaviour {
    public Animator animator;
    public Rigidbody rigidbody { get; private set; }

    public GhostState currentState;

    //Action queue. Reworked to only contain data concerning
    //the current action and the upcoming action.
    public GhostAIActionData currentAction;
    //public GhostAIActionData upcomingAction;
    public bool isExecutingAction = false;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();

        InvokeRepeating("UpdateMoveData", Random.Range(5f, 15f), Random.Range(5f, 15f));
    }

    private void Start() {
        ResetGhostController();
    }

    public void ResetGhostController() {
        //Assign this to all states as this ghost's controller.
        GeneralGhostState[] behaviors = animator.GetBehaviours<GeneralGhostState>();
        for (int i = 0; i < behaviors.Length; i++) {
            behaviors[i].ghostController = this;
        }
    }

    //For now, this ghost randomly moves around, until it hits a collider, in which it
    //performs a free action.
    void Update() {
        //TODO: If is not moving, begin movement.
        if(!isExecutingAction) {
            isExecutingAction = true;
            animator.SetBool("isMoving", true);

            //Set destination.
            UpdateMoveData();
        }
        
        //Something went wrong, so we reset.
        /*
        else if(isExecutingAction && currentState == GhostState.IDLE) {
            animator.SetBool("isMoving", true);
            //Set destination.
            UpdateMoveData();
        }
        */
    }

    void UpdateMoveData() {
        currentAction = RandomizeMoveData(2f, 6f);
    }

    GhostAIActionData RandomizeMoveData(float minDistance, float maxDistance) {
        GhostAIActionData data = new GhostAIActionData(GhostState.WALK);

        //TODO: Pick a point to move to. 
        //Generate a donut 5m in the air, radius 2 to 7.5m. 
        Vector2 unitCirclePoint = Random.insideUnitCircle * (maxDistance - minDistance);
        unitCirclePoint.x += ((unitCirclePoint.x > 0) ? minDistance : -minDistance);
        unitCirclePoint.y += ((unitCirclePoint.y > 0) ? minDistance : -minDistance);

        //Raycast downwards. 
        Vector3 raycastPoint = new Vector3(unitCirclePoint.x, 5f, unitCirclePoint.y);
        Vector3 destinationPoint = raycastPoint;
        RaycastHit hit;

        //Find a point on the terrain.
        if (Physics.Raycast(raycastPoint, -Vector3.up, out hit, Constants.TERRAIN_LAYER_MASK)) {
            raycastPoint = hit.point;
        }

        //If no point found, use the xz of raycast point and set y to 0.
        else {
            raycastPoint.y = 0;
        }

        data.args = new object[1];
        data.args[0] = raycastPoint;

        return data;
    }

    bool ignoringCollisions = false;

    public IEnumerator collisionCheck() {
        ignoringCollisions = true;
        yield return new WaitForSeconds(2f);
        ignoringCollisions = false;
    }

    private void OnCollisionEnter(Collision collision) {
        if (ignoringCollisions) return;

        if (collision.gameObject.layer != Constants.PROP_LAYER &&
            collision.gameObject.layer != Constants.GHOST_LAYER) return;

        Debug.Log(gameObject.name + " collided with " + collision.gameObject.name);

        //Initiate a free action.
        ResetAnimator();
        //Randomize a number.
        animator.SetFloat("rand01", Random.value);
        //Set the free acting trigger.
        animator.SetTrigger("isFreeActing");
        isExecutingAction = true;
    }

    private void ResetAnimator() {
        animator.ResetTrigger("isJumping");
        animator.SetBool("isMoving", false);
        animator.SetFloat("speed", 0f);
        animator.ResetTrigger("isFreeActing");
        animator.SetFloat("rand01", Random.value);

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
}