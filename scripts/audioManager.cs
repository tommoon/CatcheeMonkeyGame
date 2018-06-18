using UnityEngine.Audio;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class audioManager : MonoBehaviour {

	public sound[] sounds;


	public static audioManager instance;


	void Awake () {



		if (instance == null)
			instance = this;
		else {
			Destroy (gameObject);
			return;
		}

		DontDestroyOnLoad (gameObject);

		foreach (sound s in sounds) {
			s.source = gameObject.AddComponent<AudioSource> ();
			s.source.clip = s.clip;

			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
		}
	}

	void Start () {
		play ("Theme");
	}


	void Update () {
		foreach (sound s in sounds) {
			if (s.isEffect == false){
				s.source.volume = PlayerPrefs.GetFloat ("musicVol", 1);
				s.volume = PlayerPrefs.GetFloat ("musicVol", 1);
			}
		}

		foreach (sound s in sounds ) {
			if (s.isEffect == true){
				s.source.volume = PlayerPrefs.GetFloat ("SFXVol", 1);
				s.volume = PlayerPrefs.GetFloat ("SFXVol", 1);
			}
		}
	}
		

	public void play (string name) {
		sound s = Array.Find (sounds, sound => sound.name == name);
		s.source.Play ();
		if (s == null)
			return;
	}

	public void collision () {
		float num = UnityEngine.Random.Range(0,1);
		if (num <= 0.33)
			play ("crash");
		else if (num >= 0.66)
			play ("bang");
		else
			play ("wallop");
	}
}
