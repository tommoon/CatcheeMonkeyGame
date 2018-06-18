using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colliderscript : MonoBehaviour {

	gameMaster gm;
	controller cont;

	public GameObject gamemaster;
	public GameObject player;
	public GameObject eyes;

	void Start () {
		gm = gamemaster.GetComponent<gameMaster> ();
		cont = player.GetComponent<controller> ();
	}


	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.collider.tag == "floor" ) {
			FindObjectOfType<audioManager> ().collision();
			StartCoroutine (gm.gameOver ());
		} 
		if (coll.collider.tag == "bounds") {
			FindObjectOfType<audioManager> ().collision();
			FindObjectOfType<audioManager> ().play ("deathTouch");
			cont.disabledState = 3;
		}
		if (coll.collider.name == "brightfeathers" && coll.relativeVelocity.magnitude > 10 && cont.disabledState == 0) {
			FindObjectOfType<audioManager> ().collision();
			StartCoroutine (cont.stunned (2));
		}
		if (coll.collider.tag == "croc" && coll.relativeVelocity.magnitude > 10 && cont.disabledState == 0) {
			FindObjectOfType<audioManager> ().collision();
			StartCoroutine (cont.stunned (1));
		}
		if (coll.collider.tag == "vine") {
			FindObjectOfType<audioManager> ().play ("deathTouch");
			cont.disabledState = 3;
		}
	}
}
