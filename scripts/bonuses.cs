using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bonuses : MonoBehaviour {

	public GameObject player;
	SpriteRenderer[] skins;
	timeBody[] timeBodys;
	public gameMaster GameMaster;
	public controller cont;

	public Material normal;
	public Material glow;

	float sloTimer = 3;
	float shieldTimer = 5;
	float rewindTimer = 2;

	public Slider sloJuice;
	public Slider shieldJuice;
	public Slider rewindJuice;

	public Text shieldRemaining;
	public Text sloRemaining;
	public Text rewindRemaining;

	public Button shieldButton;
	public Button sloButton;
	public Button rewindButton;

	public Text timeRemains;
	int timeRemainCount;

	bool sloOn = false;
	bool rewinding = false;
	bool shielding = false;
	// Use this for initialization

	void Start () {
		rewinding = false;
		shieldRemaining.text = PlayerPrefs.GetInt ("shields", 2).ToString();
		sloRemaining.text = PlayerPrefs.GetInt ("slos", 2).ToString();
		rewindRemaining.text = PlayerPrefs.GetInt ("rewinds", 2).ToString ();
		timeBodys = player.GetComponentsInChildren<timeBody>();
		timeRemainCount = 3;
		timeRemains.gameObject.SetActive (false);

		if (PlayerPrefs.GetInt ("shields", 2) <= 0) {
			shieldButton.interactable = false;
		} else if (PlayerPrefs.GetInt ("shields", 2) > 0) {
			shieldButton.interactable = true;
		}

		if (PlayerPrefs.GetInt ("slos", 2) <= 0) {
			sloButton.interactable = false;
		} else if (PlayerPrefs.GetInt ("slos", 2) > 0) {
			sloButton.interactable = true;
		}

		if (PlayerPrefs.GetInt ("rewinds", 2) <= 0) {
			rewindButton.interactable = false;
		} else if (PlayerPrefs.GetInt ("rewinds", 2) > 0) {
			sloButton.interactable = true;
		}

		skins = player.GetComponentsInChildren<SpriteRenderer> ();
		sloJuice.gameObject.SetActive (false);
		sloJuice.value = 3;

		shieldJuice.gameObject.SetActive (false);
		shieldJuice.value = 5;

		rewindJuice.gameObject.SetActive (false);
	}

	void Update () {
		shieldRemaining.text = PlayerPrefs.GetInt ("shields", 2).ToString();
		sloRemaining.text = PlayerPrefs.GetInt ("slos", 2).ToString();
		rewindRemaining.text = PlayerPrefs.GetInt ("rewinds", 2).ToString ();


		if ((PlayerPrefs.GetInt ("rewinds", 2) > 0) && rewinding == false && GameMaster.business == true && sloOn == false)
			rewindButton.interactable = true;
		else
			rewindButton.interactable = false;

		if ((PlayerPrefs.GetInt ("slos", 2) > 0) && sloOn == false && GameMaster.business == true)
			sloButton.interactable = true;
		else
			sloButton.interactable = false;

		if ((PlayerPrefs.GetInt ("shields", 2) > 0) && shielding == false && GameMaster.business == true && cont.disabledState == 0)
			shieldButton.interactable = true;
		else
			shieldButton.interactable = false;
	}
		
	public void shield () {
		StartCoroutine (shieldPlay ());
	}

	public void Slomo () {
		StartCoroutine (SlomoPlay ());
	}

	public void rewind () {
		StartCoroutine (rewindPlay ());
	}

	IEnumerator rewindPlay () {
		rewinding = true;
		PlayerPrefs.SetInt ("rewinds", PlayerPrefs.GetInt ("rewinds", 2) - 1);
		rewindRemaining.text = PlayerPrefs.GetInt ("rewinds").ToString ();
		rewindJuice.gameObject.SetActive (true);
		while (rewindTimer >= 0) {
			foreach (timeBody tb in timeBodys) { 
				tb.startRewind ();
			}
			rewindTimer = rewindTimer - Time.deltaTime;
			rewindJuice.value = rewindTimer;
			yield return new WaitForEndOfFrame ();
		}
		timeRemains.gameObject.SetActive (true);
		while (timeRemainCount > 0) {
			Time.timeScale = 0;
			timeRemains.text = timeRemainCount.ToString ();
			yield return new WaitForSecondsRealtime (0.5f);
			timeRemainCount = timeRemainCount - 1;
		}
		timeRemains.gameObject.SetActive (false);
		Time.timeScale = 1;
		foreach (timeBody tb in timeBodys) { 
			tb.stopRewind ();
		}
		timeRemainCount = 3;
		rewindJuice.gameObject.SetActive (false);
		rewindJuice.value = 3;
		rewindTimer = 5;
	}

	IEnumerator shieldPlay () {
		shielding = true;
			PlayerPrefs.SetInt ("shields", PlayerPrefs.GetInt ("shields", 2) - 1);
			shieldRemaining.text = PlayerPrefs.GetInt ("shields").ToString ();
			shieldJuice.gameObject.SetActive (true);
			shieldButton.interactable = false;
			while (shieldTimer >= 0 && cont.disabledState == 0) {
				foreach (SpriteRenderer skin in skins) {
					skin.material = glow;
				}
				Physics2D.IgnoreLayerCollision (0, 9);
				shieldTimer = shieldTimer - Time.deltaTime;
				shieldJuice.value = shieldTimer;
				yield return new WaitForEndOfFrame ();
			}
			foreach (SpriteRenderer skin in skins) {
				skin.material = normal;
			}
			Physics2D.IgnoreLayerCollision (0, 9, false);
			shieldJuice.gameObject.SetActive (false);
			shieldJuice.value = 5;
			shieldTimer = 5;
		shielding = false;
	}

	IEnumerator SlomoPlay () {
		PlayerPrefs.SetInt ("slos", PlayerPrefs.GetInt ("slos", 2) - 1);
		sloRemaining.text = PlayerPrefs.GetInt ("slos").ToString();
		sloJuice.gameObject.SetActive (true);
		while (sloTimer >= 0) {
			sloOn = true;
			Time.timeScale = 0.5f;
			sloTimer = sloTimer - Time.deltaTime;
			sloJuice.value = sloTimer;
			yield return new WaitForEndOfFrame();
		}
		sloOn = false;
		Time.timeScale = 1;
		sloJuice.gameObject.SetActive (false);
		sloJuice.value = 3;
		sloTimer = 3;
		yield break;
	}
}
