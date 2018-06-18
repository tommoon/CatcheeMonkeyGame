using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stunned : MonoBehaviour {


	void onCollisionEnter2D (Collision2D col) {
		if (col.collider != null) {
			Debug.Log (col.relativeVelocity.magnitude);
		}
	}
}
