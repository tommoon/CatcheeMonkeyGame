using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeBody : MonoBehaviour {

	List<PointInTime> pointsInTime;
	List<int> disableds;

	Rigidbody2D rb;

	bool isRewinding = false;
	public float rewindTime = 3f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		pointsInTime = new List<PointInTime> ();
		disableds = new List<int> ();
	}
		

	void FixedUpdate () {
		if (isRewinding) {
			Camera.main.GetComponent<CameraTracksPlayer> ().rewind = true;
			Rewind ();
		} else {
			Camera.main.GetComponent<CameraTracksPlayer> ().rewind = false;
			Record ();
		}
	}

	void Rewind () {
		if (pointsInTime.Count > 0) {
			if (disableds.Count > 0) {
				if (GetComponent<controller> () != null) {
					GetComponent<controller> ().disabledState = disableds [0];
					disableds.RemoveAt (0);
				}
			}
			PointInTime pointInTime = pointsInTime [0];
			transform.position = pointInTime.position;
			transform.rotation = pointInTime.rotation;
			rb.velocity = pointInTime.Velocity;
			rb.angularVelocity = pointInTime.angularVelocity;
			pointsInTime.RemoveAt (0);
		} else {
			stopRewind ();
		}
	}

	void Record () {
		if (pointsInTime.Count > Mathf.Round (rewindTime / Time.fixedDeltaTime)) {
			pointsInTime.RemoveAt (pointsInTime.Count - 1);
		}
		pointsInTime.Insert (0, new PointInTime(transform,rb.velocity,rb.angularVelocity));
		if (GetComponent<controller> () != null)
			disableds.Insert (0, GetComponent<controller> ().disabledState);
	}

	public void startRewind () {
		isRewinding = true;
		rb.isKinematic = true;
	}

	public void stopRewind () {
		isRewinding = false;
		rb.isKinematic = false;

	}
}
