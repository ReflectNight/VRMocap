using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AvatarRecorder : MonoBehaviour {
    public enum AvatarPart {
        NONE = -1,
        HEAD,
        LEFT_HAND,
        RIGHT_HAND
    }

    public class AvatarState {
        public Vector3 pos;
        public Quaternion rot;
        public Quaternion inv;

        public AvatarState() {
            pos = Vector3.zero;
            rot = Quaternion.identity;
            inv = Quaternion.identity;
        }
    }
    
    public float deltaTime = .1f;

    public GameObject trackedHead, trackedLeftHand, trackedRightHand;

    private AvatarState initialHead; //initialRightHand, initialLeftHand;

    private List<AvatarState> trackingRecord;
    private bool isTracking = false;

    void Update() {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (isTracking) EndRecording();
            else BeginRecording();
        }
#endif
    }

    AvatarState GetState(GameObject obj, ref AvatarState refState) {
        AvatarState state = new AvatarState();

        //Add the positions and rotations of the body part, if active. 
        if (obj != null && obj.activeInHierarchy) {
            state.pos = obj.transform.position - refState.pos;
            state.rot = refState.inv * obj.transform.rotation;
        }

        return state;
    }

    string GetFilePath(string originalPath, string fileName, bool canReplace = true) {
        string path = originalPath + fileName + ".anim";
        if (canReplace) return path;

        int index = 1;

        while(File.Exists(path)) {
            index++;
            path = originalPath + fileName + "_" + index + ".anim";
        }

        return path;
    }

    public void BeginRecording() {
        Debug.Log("Beginning recording!");

        //Enable tracking.
        isTracking = true;

        //Record initial data.
        initialHead = new AvatarState();
        initialHead.pos = trackedHead.transform.position;
        initialHead.pos.y = 0;
        initialHead.rot = trackedHead.transform.rotation;
        initialHead.inv = Quaternion.Inverse(initialHead.rot);

        /*
        initialRightHand = new AvatarState();
        initialRightHand.pos = trackedRightHand.transform.position;
        initialRightHand.rot = trackedRightHand.transform.rotation;
        initialRightHand.inv = Quaternion.Inverse(initialRightHand.rot);

        initialLeftHand = new AvatarState();
        initialLeftHand.pos = trackedLeftHand.transform.position;
        initialLeftHand.rot = trackedLeftHand.transform.rotation;
        initialLeftHand.inv = Quaternion.Inverse(initialLeftHand.rot);
        */
        StartCoroutine(Record());
    }

    IEnumerator Record() {
        WaitForSeconds delta = new WaitForSeconds(deltaTime);
        trackingRecord = new List<AvatarState>();

        while (isTracking) {
            trackingRecord.Add(GetState(trackedHead, ref initialHead));
            //trackingRecord.Add(GetState(trackedLeftHand, ref initialLeftHand));
            //trackingRecord.Add(GetState(trackedRightHand, ref initialRightHand));
            trackingRecord.Add(GetState(trackedLeftHand, ref initialHead));
            trackingRecord.Add(GetState(trackedRightHand, ref initialHead));
            yield return delta;
        }
    }

    public void EndRecording(string filePath = "", string state = "test") {
        Debug.Log("Ending recording!");

#if UNITY_EDITOR
        SaveToClip(filePath, state);
#endif

        //Disable tracking.
        isTracking = false;
    }

    void SaveToClip(string filePath, string stateName) {
        //Put this data in an animation clip.
        AnimationClip clip = new AnimationClip();
        //clip.legacy = true;

        int numKeys = trackingRecord.Count / 3;

        //Create head keyframes.
        Keyframe[] headKeysPosX = new Keyframe[numKeys];
        Keyframe[] headKeysPosY = new Keyframe[numKeys];
        Keyframe[] headKeysPosZ = new Keyframe[numKeys];

        Keyframe[] headKeysRotW = new Keyframe[numKeys];
        Keyframe[] headKeysRotX = new Keyframe[numKeys];
        Keyframe[] headKeysRotY = new Keyframe[numKeys];
        Keyframe[] headKeysRotZ = new Keyframe[numKeys];

        //Create left hand keyframes.
        Keyframe[] leftHandKeysPosX = new Keyframe[numKeys];
        Keyframe[] leftHandKeysPosY = new Keyframe[numKeys];
        Keyframe[] leftHandKeysPosZ = new Keyframe[numKeys];

        Keyframe[] leftHandKeysRotW = new Keyframe[numKeys];
        Keyframe[] leftHandKeysRotX = new Keyframe[numKeys];
        Keyframe[] leftHandKeysRotY = new Keyframe[numKeys];
        Keyframe[] leftHandKeysRotZ = new Keyframe[numKeys];

        //Create right hand keyframes.
        Keyframe[] rightHandKeysPosX = new Keyframe[numKeys];
        Keyframe[] rightHandKeysPosY = new Keyframe[numKeys];
        Keyframe[] rightHandKeysPosZ = new Keyframe[numKeys];

        Keyframe[] rightHandKeysRotW = new Keyframe[numKeys];
        Keyframe[] rightHandKeysRotX = new Keyframe[numKeys];
        Keyframe[] rightHandKeysRotY = new Keyframe[numKeys];
        Keyframe[] rightHandKeysRotZ = new Keyframe[numKeys];

        float time;
        AvatarState state;

        for (int i = 0; i < numKeys; i++) {
            time = i * deltaTime;
            state = trackingRecord[i * 3];

            //Add head positions and rotations.
            headKeysPosX[i] = new Keyframe(time, state.pos.x);
            headKeysPosY[i] = new Keyframe(time, state.pos.y);
            headKeysPosZ[i] = new Keyframe(time, state.pos.z);

            headKeysRotW[i] = new Keyframe(time, state.rot.w);
            headKeysRotX[i] = new Keyframe(time, state.rot.x);
            headKeysRotY[i] = new Keyframe(time, state.rot.y);
            headKeysRotZ[i] = new Keyframe(time, state.rot.z);

            state = trackingRecord[i * 3 + 1];

            //Add left hand.
            leftHandKeysPosX[i] = new Keyframe(time, state.pos.x);
            leftHandKeysPosY[i] = new Keyframe(time, state.pos.y);
            leftHandKeysPosZ[i] = new Keyframe(time, state.pos.z);

            leftHandKeysRotW[i] = new Keyframe(time, state.rot.w);
            leftHandKeysRotX[i] = new Keyframe(time, state.rot.x);
            leftHandKeysRotY[i] = new Keyframe(time, state.rot.y);
            leftHandKeysRotZ[i] = new Keyframe(time, state.rot.z);

            state = trackingRecord[i * 3 + 2];

            //Add right hand.
            rightHandKeysPosX[i] = new Keyframe(time, state.pos.x);
            rightHandKeysPosY[i] = new Keyframe(time, state.pos.y);
            rightHandKeysPosZ[i] = new Keyframe(time, state.pos.z);

            rightHandKeysRotW[i] = new Keyframe(time, state.rot.w);
            rightHandKeysRotX[i] = new Keyframe(time, state.rot.x);
            rightHandKeysRotY[i] = new Keyframe(time, state.rot.y);
            rightHandKeysRotZ[i] = new Keyframe(time, state.rot.z);
        }

        AnimationCurve headPosXCurve = new AnimationCurve(headKeysPosX);
        clip.SetCurve("Head", typeof(Transform), "localPosition.x", headPosXCurve);
        AnimationCurve headPosYCurve = new AnimationCurve(headKeysPosY);
        clip.SetCurve("Head", typeof(Transform), "localPosition.y", headPosYCurve);
        AnimationCurve headPosZCurve = new AnimationCurve(headKeysPosZ);
        clip.SetCurve("Head", typeof(Transform), "localPosition.z", headPosZCurve);
        AnimationCurve headRotWCurve = new AnimationCurve(headKeysRotW);
        clip.SetCurve("Head", typeof(Transform), "localRotation.w", headRotWCurve);
        AnimationCurve headRotXCurve = new AnimationCurve(headKeysRotX);
        clip.SetCurve("Head", typeof(Transform), "localRotation.x", headRotXCurve);
        AnimationCurve headRotYCurve = new AnimationCurve(headKeysRotY);
        clip.SetCurve("Head", typeof(Transform), "localRotation.y", headRotYCurve);
        AnimationCurve headRotZCurve = new AnimationCurve(headKeysRotZ);
        clip.SetCurve("Head", typeof(Transform), "localRotation.z", headRotZCurve);

        AnimationCurve leftHandPosXCurve = new AnimationCurve(leftHandKeysPosX);
        clip.SetCurve("LeftHand", typeof(Transform), "localPosition.x", leftHandPosXCurve);
        AnimationCurve leftHandPosYCurve = new AnimationCurve(leftHandKeysPosY);
        clip.SetCurve("LeftHand", typeof(Transform), "localPosition.y", leftHandPosYCurve);
        AnimationCurve leftHandPosZCurve = new AnimationCurve(leftHandKeysPosZ);
        clip.SetCurve("LeftHand", typeof(Transform), "localPosition.z", leftHandPosZCurve);
        AnimationCurve leftHandRotWCurve = new AnimationCurve(leftHandKeysRotW);
        clip.SetCurve("LeftHand", typeof(Transform), "localRotation.w", leftHandRotWCurve);
        AnimationCurve leftHandRotXCurve = new AnimationCurve(leftHandKeysRotX);
        clip.SetCurve("LeftHand", typeof(Transform), "localRotation.x", leftHandRotXCurve);
        AnimationCurve leftHandRotYCurve = new AnimationCurve(leftHandKeysRotY);
        clip.SetCurve("LeftHand", typeof(Transform), "localRotation.y", leftHandRotYCurve);
        AnimationCurve leftHandRotZCurve = new AnimationCurve(leftHandKeysRotZ);
        clip.SetCurve("LeftHand", typeof(Transform), "localRotation.z", leftHandRotZCurve);

        AnimationCurve rightHandPosXCurve = new AnimationCurve(rightHandKeysPosX);
        clip.SetCurve("RightHand", typeof(Transform), "localPosition.x", rightHandPosXCurve);
        AnimationCurve rightHandPosYCurve = new AnimationCurve(rightHandKeysPosY);
        clip.SetCurve("RightHand", typeof(Transform), "localPosition.y", rightHandPosYCurve);
        AnimationCurve rightHandPosZCurve = new AnimationCurve(rightHandKeysPosZ);
        clip.SetCurve("RightHand", typeof(Transform), "localPosition.z", rightHandPosZCurve);
        AnimationCurve rightHandRotWCurve = new AnimationCurve(rightHandKeysRotW);
        clip.SetCurve("RightHand", typeof(Transform), "localRotation.w", rightHandRotWCurve);
        AnimationCurve rightHandRotXCurve = new AnimationCurve(rightHandKeysRotX);
        clip.SetCurve("RightHand", typeof(Transform), "localRotation.x", rightHandRotXCurve);
        AnimationCurve rightHandRotYCurve = new AnimationCurve(rightHandKeysRotY);
        clip.SetCurve("RightHand", typeof(Transform), "localRotation.y", rightHandRotYCurve);
        AnimationCurve rightHandRotZCurve = new AnimationCurve(rightHandKeysRotZ);
        clip.SetCurve("RightHand", typeof(Transform), "localRotation.z", rightHandRotZCurve);

        clip.EnsureQuaternionContinuity();

        AssetDatabase.CreateAsset(clip, GetFilePath(filePath, stateName + "_1"));
        AssetDatabase.SaveAssets();

        //Clear data when done to save memory.
        trackingRecord.Clear();

        Debug.Log("Saved animation clip!");
    }
}