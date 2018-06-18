using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
 

public class menuManager : MonoBehaviour {
	public Button shopButton;
	public Button optionButton;

	public Slider musicVol;
	public Slider SFXVol;
	public Toggle background;

	public GameObject optionsMenu;
	public GameObject shopMenu;

	controller cont;

	public Animator noLives;
	public GameObject noLivesPanel;

	public static bool workHours;
	// Use this for initialization
	void Start () {
		if (onOrOff.onoroff == false) {
			background.isOn = false;
		}
		if (onOrOff.onoroff == true) {
			background.isOn = true;
		}
		
		musicVol.value = PlayerPrefs.GetFloat ("musicVol", 1);
		SFXVol.value = PlayerPrefs.GetFloat ("SFXVol", 1);

		musicVol.GetComponent<Slider>().onValueChanged.AddListener(musicChange);
		SFXVol.GetComponent<Slider>().onValueChanged.AddListener(SFXChange);


		cont = FindObjectOfType<controller> ();

		workHours = true;
	}

	// Update is called once per frame

	public void noLivesOn () {
		StartCoroutine (noLivesAction ());
	}

	IEnumerator noLivesAction () {
		noLivesPanel.SetActive (true);
		noLives.Play ("noLives");
		yield break;
	}
		

	public void OptionsMenuOn () {
		cont.disabledState = 1;
		Time.timeScale = 0;
		optionsMenu.SetActive (true);
		shopButton.interactable = false;
		optionButton.interactable = false;
	}

	public void shopMenuOn () {
		cont.disabledState = 1;
		Time.timeScale = 0;
		shopMenu.SetActive (true);
		if (noLivesPanel.activeSelf == true)
			noLivesPanel.SetActive (false);
		shopButton.interactable = false;
		optionButton.interactable = false;
	}

	public void Back () {
		StartCoroutine (backAction ());
	}
		IEnumerator backAction () {
		optionsMenu.SetActive (false);
		shopMenu.SetActive (false);
		shopButton.interactable = true;
		optionButton.interactable = true;
		Time.timeScale = 1;
		yield return new WaitForEndOfFrame ();
		cont.disabledState = 0;
	}

	public void musicChange(float vol) {
		PlayerPrefs.SetFloat ("musicVol", vol);
	}

	public void SFXChange (float vol) {
		PlayerPrefs.SetFloat ("SFXVol", vol);
	}

	public void Help () {
		PlayerPrefs.SetInt ("firstTime", 1);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		Time.timeScale = 1;
	}
}
