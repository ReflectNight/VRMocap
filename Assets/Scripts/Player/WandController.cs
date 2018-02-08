using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider))]
[RequireComponent (typeof(SteamVR_TrackedController))]
public class WandController : MonoBehaviour {
    private SteamVR_TrackedController _controller;

    public delegate void SimpleEventHandler();
    public static event SimpleEventHandler GeneralTriggerClicked;

    //What hand this is
    //What it is currently holding (if any)
    //What it is intersecting with

    void Awake() {
        _controller = GetComponent<SteamVR_TrackedController>();

        //Register all button press events.
        _controller.TriggerClicked += OnTriggerClicked;
        _controller.TriggerUnclicked += OnTriggerUnclicked;

    }

    #region CALLBACKS
    void OnTriggerClicked(object sender, ClickedEventArgs e) {
        //If triggering something, begin interaction with that object.
        //Otherwise, call the general callback.
        if (GeneralTriggerClicked != null) GeneralTriggerClicked();
    }

    void OnTriggerUnclicked(object sender, ClickedEventArgs e) {
        Debug.Log("Trigger unclicked");
    }

    void OnPadClicked(object sender, ClickedEventArgs e) {
    }

    void OnPadUnclicked(object sender, ClickedEventArgs e) {
    }

    void OnPadTouched(object sender, ClickedEventArgs e) {
    }

    void OnPadUntouched(object sender, ClickedEventArgs e) {
    }

    void OnGripped(object sender, ClickedEventArgs e) {
    }

    void OnUngripped(object sender, ClickedEventArgs e) {
    }
    #endregion

    void Pickup() {

    }

    void OnTriggerEnter(Collider col) {

    }

    void OnTriggerExit(Collider col) {

    }
}
