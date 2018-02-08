using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour {
    public Transform targetHead;

    public Transform rootNode;
    public Transform headNode;
    private Vector3 _headOffset;

    public Vector3 forwardDir;
    public Vector3 upDir;

    public float angleThreshold;
    public float rotationMultiplier = 1f;

    private int _chainLength;
    private List<Quaternion> _referenceNodes;
    private Quaternion _headOffsetRot;

    void Awake() {
        if (rootNode == null) {
            rootNode = transform;
        }

        _chainLength = 0;
        _referenceNodes = new List<Quaternion>();

        Transform t = headNode;
        while (t != rootNode && t != t.root) {
            _chainLength++;
            _referenceNodes.Add(t.localRotation);
            Debug.Log(t.name + " Local Rotation: " + t.localRotation.eulerAngles);
            t = t.parent;
        }

        _referenceNodes.Add(t.localRotation);

        if (headNode != null && rootNode != null) {
            _headOffset = transform.position - headNode.position;
            _headOffsetRot = Quaternion.Inverse(headNode.transform.rotation);
        }
    }

    void Update() {
        if (Time.deltaTime == 0) return;
        if (targetHead == null) return;

        //Update position of body based on headset.
        transform.position = targetHead.position + _headOffset;

        //Get rotation of headset.
        Quaternion targetRot = targetHead.rotation;

        //Desired look direction in world space.
        Vector3 lookDirWorld = targetRot * forwardDir;
        //Current direction of root.
        Vector3 lookDirRoot = rootNode.rotation * forwardDir;

        //Find the horizontal angle between head and root.
        float hAngle = AngleAroundAxis(lookDirRoot, lookDirWorld, upDir);

        //Find the vertical angle between head and root.
        Vector3 rightDir = Vector3.Cross(upDir, forwardDir).normalized;
        Vector3 lookDirRootInHPlane = lookDirRoot - Vector3.Project(lookDirRoot, upDir);
        float vAngle = AngleAroundAxis(lookDirRootInHPlane, lookDirWorld, rightDir);

        //If horizontal angle is wider than threshold, correct the body.
        Transform t;
        
        if (Mathf.Abs(hAngle) > angleThreshold || Mathf.Abs(vAngle) > angleThreshold) {
            Quaternion hRot = Quaternion.AngleAxis(hAngle, upDir);
            rootNode.rotation = Quaternion.Slerp(rootNode.rotation, 
                hRot * rootNode.rotation, Time.deltaTime * rotationMultiplier);
        }

        Quaternion rootInv = Quaternion.Inverse(rootNode.rotation);

        //Keep position of first transform fixed and lerp intermediary transforms.
        Quaternion dividedRotation =
            Quaternion.Slerp(rootNode.rotation, targetRot, 1f / _chainLength);

        t = headNode;
        float offset = 1f / _chainLength;

        //Apply distributed rotations over all joints.
        for (int i = 0; i < _chainLength; i++) {
            t.localRotation = _referenceNodes[i];
            t.rotation = dividedRotation * rootInv * t.rotation;
            t = t.parent;
        }

        Debug.DrawLine(headNode.transform.position,
            headNode.transform.position + headNode.forward, Color.green);
        Debug.DrawLine(rootNode.transform.position,
            rootNode.transform.position + rootNode.forward, Color.blue);
        Debug.DrawLine(headNode.transform.position,
            headNode.transform.position + targetRot * Vector3.forward, Color.red);
    }

    // The angle between dirA and dirB around axis
    public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis) {
        // Project A and B onto the plane orthogonal target axis
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);

        // Find (positive) angle between A and B
        float angle = Vector3.Angle(dirA, dirB);

        // Return angle multiplied with 1 or -1
        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }
}
