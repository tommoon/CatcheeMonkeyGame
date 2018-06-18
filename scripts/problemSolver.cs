using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class problemSolver : MonoBehaviour {

	void OnTriggerStay2D (Collider2D col) {
		if ((col.tag == "spider") || (col.tag == "peacock")) {
			if (col.transform.position.x - transform.position.x < 0)
				transform.position += new Vector3 (8, 0, 0);
		} else if ((col.tag == "spider") || (col.tag == "peacock")) {
			if (col.transform.position.x - transform.position.x > 0)
				transform.position += new Vector3 (-8, 0, 0);
		}
	}
}
