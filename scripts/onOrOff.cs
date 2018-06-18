using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onOrOff : MonoBehaviour {


	public static bool onoroff = false;

	void Start() {

		SpriteRenderer[] sprend = gameObject.GetComponentsInChildren<SpriteRenderer> ();
		if (onoroff == true) {
			foreach (SpriteRenderer spr in sprend)
				spr.enabled = false;
		}
		if (onoroff == false) {
			foreach (SpriteRenderer spr in sprend)
				spr.enabled = true;
		}
	}

	void Update () {
		SpriteRenderer[] sprend = gameObject.GetComponentsInChildren<SpriteRenderer> ();
		if (onoroff == true) {
			foreach (SpriteRenderer spr in sprend)
				spr.enabled = false;
		}
		if (onoroff == false) {
			foreach (SpriteRenderer spr in sprend)
				spr.enabled = true;
		}
	}

	public void toggleChanged (bool newVal) {
		if (newVal == true)
			onoroff = newVal;
		if (newVal == false)
			onoroff = newVal;
	}
}
