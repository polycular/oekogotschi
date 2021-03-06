﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof (QCARBehaviour))]
public class SmoothARCamera : MonoBehaviour, ITrackerEventHandler  {

    public int smoothingFrames = 10;
    private QCARBehaviour qcarBehavior;

    private Quaternion smoothedRotation;
    private Vector3 smoothedPosition;

    private Queue<Quaternion> rotations;
    private Queue<Vector3> positions;

    public void OnInitialized() {
    }

    public void OnTrackablesUpdated() {
        if (rotations.Count >= smoothingFrames) {
            rotations.Dequeue();
            positions.Dequeue();
        }
        
        rotations.Enqueue(transform.rotation);
        positions.Enqueue(transform.position);

        Vector4 avgr = Vector4.zero;
        foreach (Quaternion singleRotation in rotations) {
            Math3d.AverageQuaternion(ref avgr, singleRotation, rotations.Peek(), rotations.Count);
        }

        Vector3 avgp = Vector3.zero;
        foreach (Vector3 singlePosition in positions) {
            avgp += singlePosition;
        }
        avgp /= positions.Count;

        smoothedRotation = new Quaternion(avgr.x, avgr.y, avgr.z, avgr.w);
        smoothedPosition = avgp;
    }

	// Use this for initialization
    void Start () {
        rotations = new Queue<Quaternion>(smoothingFrames);
        positions = new Queue<Vector3>(smoothingFrames);
        qcarBehavior = GetComponent<QCARBehaviour>();
        qcarBehavior.RegisterTrackerEventHandler(this, true);
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.rotation = smoothedRotation;
        transform.position = smoothedPosition;
	}

}
