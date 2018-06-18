using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class peacockscript : MonoBehaviour {

	public GameObject brightFeathers;
	public GameObject dullFeathers;

	public Sprite sleeping;
	public Sprite awake;

	bool first = true;

	SpriteRenderer thisone;
	// Use this for initialization
	void Start () {
		thisone = GetComponent<SpriteRenderer> ();
		thisone.sprite = sleeping;
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D col) {
		StartCoroutine (spiritsAwake ());
	}
	IEnumerator spiritsAwake() {
		if (first) {
			FindObjectOfType<audioManager> ().play ("peacockAwake");
			FindObjectOfType<audioManager> ().play ("peacockAwake");
			first = false;
			thisone.sprite = awake;
			dullFeathers.SetActive (false);
			brightFeathers.SetActive (true);
			yield return new WaitForSeconds (5);
			thisone.sprite = sleeping;
			dullFeathers.SetActive (true);
			brightFeathers.SetActive (false);
			FindObjectOfType<audioManager> ().play ("peacockAsleep");
			first = true;
		}
	}
}
