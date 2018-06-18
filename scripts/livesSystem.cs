using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExaGames.Common;

public class livesSystem : MonoBehaviour {

	public LivesManager livesManager;
	public List<GameObject> lives;
	int livesNum;
	public Text TimeToNextLifeText;
	public Slider time;

	public Image fillRect;

	bool hold;

	void Start () {
		bool hold = true;
	}
	void Update () {
		
		if (livesManager.HasMaxLives) {
			if (livesManager.HasInfiniteLives) {
				fillRect.color = new Color32 (255, 215, 0, 255);
				time.value = time.maxValue;
				time.gameObject.SetActive (true);
			} else {
				fillRect.color = new Color32 (216, 0, 0, 255);
				time.gameObject.SetActive (false);
				hold = true;
			}
		} else {
			if (hold) {
				time.gameObject.SetActive (true);
				hold = false;
			}
			fillRect.color = new Color32 (216, 0, 0, 255);
			float num = Mathf.InverseLerp (1, 1800, (float)livesManager.SecondsToNextLife);
			time.value = 1 - num;
		}
	}

	public void OnLivesChanged() {
		livesNum = livesManager.Lives;
		foreach (GameObject life in lives) {
			life.SetActive (false);
		}

		for (int i = 0; i < livesNum; i++) {
			lives [i].SetActive (true);
		}

	}
	public void OnTimeToNextLifeChanged() {
		TimeToNextLifeText.text = livesManager.RemainingTimeString;
	}
}
