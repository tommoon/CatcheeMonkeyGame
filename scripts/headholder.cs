using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class headholder : MonoBehaviour {

	DistanceJoint2D head;
	Animator headanim;
	public Animator text;
	public Animator gotext;

	public Text thegameover;
	public Text goagain;

	public Text highscore;
	public Text highscoretext;

	public GameObject share;
	public controller Cont;

	// Use this for initialization
	void Start () {
		headanim = GetComponent<Animator> ();
		goagain.gameObject.SetActive (false);
		share.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.localPosition = new Vector3 (-0.3600384f, 0.2172839f, 0);

	}

	public IEnumerator runTearsLeft () {
		Cont.disabledState = 1;
		headanim.Play ("tears");

		while (thegameover.gameObject.transform.localScale.y < 1) {
			text.Play ("gamover");
			yield return null;
		}

		highscore.color = Color.white;
		highscoretext.color = Color.white;
		goagain.gameObject.SetActive (true);
		share.gameObject.SetActive (true);
		gotext.Play ("goagain");
		yield return null;
	}

	public IEnumerator runTearsRight () {
		Cont.disabledState = 1;
		headanim.Play ("tearsright");

		while (thegameover.gameObject.transform.localScale.y < 1) {
			text.Play ("gamover");
			yield return null;
		}

		highscore.color = Color.white;
		highscoretext.color = Color.white;
		goagain.gameObject.SetActive (true);
		share.gameObject.SetActive (true);
		gotext.Play ("goagain");
		yield return null;
	}

	public void startAgain () {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
