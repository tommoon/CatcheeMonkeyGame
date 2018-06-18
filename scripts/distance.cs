using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class distance : MonoBehaviour {
	DistanceJoint2D joint;
	// Use this for initialization
	void Start () {
		joint = GetComponent<DistanceJoint2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		joint.distance = 0;
	}
}
