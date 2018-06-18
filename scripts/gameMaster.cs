using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameMaster : MonoBehaviour {

	public GameObject player;
	private float startingPos;
	private float currentPos;
	public Rigidbody2D thisone;
	public Rigidbody2D tailone;
	public headholder heady;

	private int score;
	private int targetScore;

	public Text theScore;
	public Text highScoretext;
	public GameObject scoring;
	public GameObject shieldCount;
	public GameObject slomoCount;
	public GameObject rewindCount;
	public Text currentScoreWatermark;

	Camera main;

	public Animator charanims;

	public bool gamefin = false;
	public bool business = false;

	public Button[] powerUps;

	public static gameMaster instance;

	bool actiontaken1 = false;
	bool actiontaken2 = false;
	bool actiontaken3 = false;
	bool actiontaken4 = false;


	bool skip = false;

	UniShare sharer;

	// Use this for initialization
	void Start () {
		sharer = GetComponent<UniShare> ();
		main = Camera.main;
		startingPos = player.transform.position.x;
		score = 0;
		targetScore = 500;
		theScore.text = score.ToString ();
		highScoretext.text = PlayerPrefs.GetInt ("highscore").ToString ();
		PlayerPrefs.SetInt ("bonus", 0);
		foreach (Button powerup in powerUps) {
			powerup.interactable = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		currentPos = player.transform.position.x + 1;
		score = (Mathf.FloorToInt((currentPos - startingPos)/10)) + PlayerPrefs.GetInt("bonus", 0);
		Mathf.Clamp (score, 0, Mathf.Infinity);
		theScore.text = score.ToString ();

		if (Input.GetKeyDown (KeyCode.A)) {
			PlayerPrefs.DeleteAll();
		}
		if (gamefin == true) {
			if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown (0))) {
				skip = true;
			}
		}
			
		if (score >= 250)
			rewardSoundTwo ();
	
		if (score >= 500)
			rewardSoundThree ();

		if (score >= 1000)
			rewardSoundFour ();

		if (score >= targetScore) {
			StartCoroutine (reward (Random.Range (1, 4))); 
				targetScore += 500;
		}

		if (skip == true) {
			gamefin = false;
			charanims.enabled = true;
			if (thisone.transform.position.x < tailone.transform.position.x) {
				heady.StartCoroutine ("runTearsLeft");
			} else if (thisone.transform.position.x > tailone.transform.position.x) {
				heady.StartCoroutine ("runTearsRight");
			}

			if (score > PlayerPrefs.GetInt ("highscore", 0)) {
				PlayerPrefs.SetInt ("highscore", score);
				highScoretext.text = score.ToString ();
				score = 0;
			}

			foreach (Button powerup in powerUps) {
				powerup.interactable = false;
			}

			player.GetComponent<controller> ().disabledState = 1;

			player.GetComponent<Rigidbody2D> ().interpolation = RigidbodyInterpolation2D.None;
			thisone.interpolation = RigidbodyInterpolation2D.None;
			tailone.interpolation = RigidbodyInterpolation2D.None;

			player.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Static;
			thisone.bodyType = RigidbodyType2D.Static;
			tailone.bodyType = RigidbodyType2D.Static;

			main.orthographicSize = 2;
			main.transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, main.transform.position.z);

		}
	}

	IEnumerator reward (int randNum){

		if (randNum == 1) { //shield
			shieldCount.GetComponent<Animator>().Play("income");
			yield return new WaitForSecondsRealtime (0.3f);
			PlayerPrefs.SetInt("shields", PlayerPrefs.GetInt("shields") + 1);
		}

		if (randNum == 2) { //slomo
			slomoCount.GetComponent<Animator>().Play("income");
			yield return new WaitForSecondsRealtime (0.3f);
			PlayerPrefs.SetInt("slos", PlayerPrefs.GetInt("slos") + 1);
		}

		if (randNum == 3) { //reverse
			rewindCount.GetComponent<Animator>().Play("income");
			yield return new WaitForSecondsRealtime (1f);
			PlayerPrefs.SetInt("rewinds", PlayerPrefs.GetInt("rewinds") + 1);
		}

		scoring.GetComponent<Animator> ().Play ("wellDone");
		yield break;
	}


	void rewardSoundOne () {
		if (actiontaken1 == false) {
			FindObjectOfType<audioManager> ().play ("nice");
			actiontaken1 = true;
		}
	}

	void rewardSoundTwo () {
		if (actiontaken2 == false) {
			FindObjectOfType<audioManager> ().play ("realnice");
			actiontaken2 = true;
		}
	}

	void rewardSoundThree () {
		if (actiontaken3 == false) {
			FindObjectOfType<audioManager> ().play ("awyeah");
			actiontaken3 = true;
		}
	}

	void rewardSoundFour () {
		if (actiontaken4 == false) {
			FindObjectOfType<audioManager> ().play ("justwhat");
			actiontaken4 = true;
		}
	}
	public IEnumerator gameOver () {
		business = false;
		gamefin = true;
		sharer.points = score;
		currentScoreWatermark.text = "I scored: " + score.ToString () + " points!";
		while (skip == false) {
			if (score > PlayerPrefs.GetInt ("highscore", 0)) {
				PlayerPrefs.SetInt ("highscore", score);
				highScoretext.text = score.ToString ();
				score = 0;
			}

			foreach (Button powerup in powerUps) {
				powerup.interactable = false;
			}

			player.GetComponent<controller> ().disabledState = 1;

			player.GetComponent<Rigidbody2D> ().interpolation = RigidbodyInterpolation2D.None;
			thisone.interpolation = RigidbodyInterpolation2D.None;
			tailone.interpolation = RigidbodyInterpolation2D.None;

			yield return new WaitForFixedUpdate ();

			float speed = player.GetComponent<Rigidbody2D> ().velocity.magnitude;
			float angularsp = player.GetComponent<Rigidbody2D> ().angularVelocity;
			while (speed > 0.01f) {
				yield return null;
				speed = player.GetComponent<Rigidbody2D> ().velocity.magnitude;

			}
			while (angularsp > 0.1f) {
				yield return null;
				angularsp = player.GetComponent<Rigidbody2D> ().angularVelocity;
			}
			player.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Static;
			thisone.bodyType = RigidbodyType2D.Static;
			tailone.bodyType = RigidbodyType2D.Static;

			yield return new WaitForSeconds (1);
	
			while ((main.orthographicSize > 2) && (main.transform.position.y != player.transform.position.y)) {
				yield return new WaitForFixedUpdate ();
				Vector3 newpos = new Vector3 (player.transform.position.x, player.transform.position.y, main.transform.position.z);
				main.orthographicSize = Mathf.MoveTowards (main.orthographicSize, 2, 7 * Time.fixedDeltaTime);
				main.transform.position = Vector3.MoveTowards (main.transform.position, newpos, 7 * Time.fixedDeltaTime);
			}

			yield return new WaitForEndOfFrame ();


			charanims.enabled = true;
			if (thisone.transform.position.x < tailone.transform.position.x) {
				charanims.Play ("leftlanding");
				yield break;
			} else if (thisone.transform.position.x > tailone.transform.position.x) {
				charanims.Play ("rightlanding");
				yield break;
			}
			yield return null;
		} 
	}
}
