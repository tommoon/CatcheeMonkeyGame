using UnityEngine.Audio;
using UnityEngine;


[System.Serializable]
public class sound  {

	public string name;

	public bool isEffect;

	public AudioClip clip;

	[Range(0f,1f)]
	public float pitch;

	[Range(0f,1f)]
	public float volume;

	public bool loop;

	[HideInInspector]
	public AudioSource source;
}
