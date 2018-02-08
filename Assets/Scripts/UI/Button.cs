using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralButton : MonoBehaviour {
    public virtual void Init() { }

    public virtual void OnPress() {
        //TODO: Trigger animation.
    }

    public void OnCollisionEnter(Collision col) {
        //If the button has collided with a hand, then trigger the press event.
        if (col.gameObject.layer == Constants.PLAYER_LAYER) {
            OnPress();
        }
    }
}