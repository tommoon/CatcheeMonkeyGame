using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExaGames.Common;
using UnityEngine.Purchasing;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System;

public class shopScript : MonoBehaviour {

	public LivesManager livesManager;
	public Button shields;
	public Button slomo;
	public Button rewind;

	public float msToWait = 3599999;
	public Button VideoWatch;
	private ulong lastWatch;
	private Text textTimer;

	public Button infiniteLives;
	public Button fiveLives;


	void Start () {

		textTimer = VideoWatch.gameObject.GetComponentInChildren<Text> ();
		if (PlayerPrefs.GetString ("lastWatch") != null) {
			lastWatch = ulong.Parse (PlayerPrefs.GetString ("lastWatch"));
		}

		if (!isVideoReady ())
			VideoWatch.interactable = false;
	}
	void Update () {
		if (!VideoWatch.IsInteractable ()) {
			if (isVideoReady ()) {
				VideoWatch.interactable = true;
				textTimer.text = "Watch";
				return;
			}

			ulong diff = ((ulong)DateTime.Now.Ticks - lastWatch);
			ulong milliseconds = diff / TimeSpan.TicksPerMillisecond;

			float secondsLeft = (float)(msToWait - milliseconds) / 1000.0f;

			string r = " ";
			r += ((int)secondsLeft/60).ToString("00") + ":";

			r += (secondsLeft % 60).ToString ("00");;
			textTimer.text = r;
		}


		if (livesManager.HasInfiniteLives)
			infiniteLives.interactable = false;
		else
			infiniteLives.interactable = true;

		if (livesManager.HasMaxLives)
			fiveLives.interactable = false;
		else
			fiveLives.interactable = true;
	}


	public void restoreLive () {
		livesManager.FillLives ();
	}

	public void InfiniteLives () {
		livesManager.GiveInifinite (10080);
	}

	public void addShields () {
		PlayerPrefs.SetInt("shields", (PlayerPrefs.GetInt("shields", 2) + 3));
		shields.interactable = true;
	}

	public void addSlomo () {
		PlayerPrefs.SetInt("slos", (PlayerPrefs.GetInt("slos", 2) + 3));
		slomo.interactable = true;
	}

	public void addRewind () {
		PlayerPrefs.SetInt("rewinds", (PlayerPrefs.GetInt("rewinds", 2) + 3));
		rewind.interactable = true;
	}

	public void addTrio () {
		PlayerPrefs.SetInt("rewinds", (PlayerPrefs.GetInt("rewinds", 2) + 3));
		PlayerPrefs.SetInt("slos", (PlayerPrefs.GetInt("slos", 2) + 3));
		PlayerPrefs.SetInt("shields", (PlayerPrefs.GetInt("shields", 2) + 3));
		shields.interactable = true;
		slomo.interactable = true;
		rewind.interactable = true;
	}

	private bool isVideoReady () {
		ulong diff = ((ulong)DateTime.Now.Ticks - lastWatch);
		ulong milliseconds = diff / TimeSpan.TicksPerMillisecond;

		float secondsLeft = (float)(msToWait - milliseconds) / 1000.0f;

		if (secondsLeft < 0) 
			return true;
		
		return false;
	}
		

	public void watchVideo () {
		if (Advertisement.IsReady("rewardedVideo"))
		{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("rewardedVideo", options);
		}
	}

	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			lastWatch = (ulong)DateTime.Now.Ticks;
			PlayerPrefs.SetString ("lastWatch", lastWatch.ToString ());
			VideoWatch.interactable = false;
			livesManager.GiveOneLife ();
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}
}

