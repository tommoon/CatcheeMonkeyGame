using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExaGames.Common;
using UnityEngine.EventSystems;

public class controller : MonoBehaviour {

	public LivesManager livesManager;
	public menuManager menuManager;

	public GameObject eyes;
	public GameObject tail;
	public GameObject skull;

	public Animator godray;
	public GameObject zzz;
	public GameObject tutbutton;
	public GameObject releaseButton;
	public bool tutmoveon = false;
	bool tutMoveOnAgain = false;
	public GameObject skip;
	public GameObject missSnake;
	public Animator snakeMiss;

	Rigidbody2D rb;
	public GameObject GameMaster;
	public GameObject rope;
	public GameObject SnakeHead;
	public GameObject snakeHeadBite;
	gameMaster gm;
	ropeTextures ropey;
	DistanceJoint2D tailJoint;


	public LayerMask hits; //objects that are possible to hit
	public LineRenderer line; // the rope

	private Vector3 target; //where the mouse is
	private Vector3 currentPos; //where the character is
	private Vector3 clickpos;
	private Vector2 head;
	public Vector3 gravity; //just a theory
	private Vector2 gravOn1 = new Vector2(0,-20f);
	private Vector2 gravOff = Vector2.zero;


	public float speed = 12;// how fast the character moves towards the strike
	public float distance; //max length of the rope

	bool gravOn; //is gravity active
	public int disabledState = 0;
	bool inTut = false;
	bool played = false;

	Quaternion headrot;

	SpriteRenderer[] skins;
	public GameObject parent;

	ParticleSystem part;

	bool opening;

	int tutorialRun;
	// Use this for initialization
	void Start () {
		tutorialRun = PlayerPrefs.GetInt ("firstTime", 1);
		if (tutorialRun == 1) {
			Camera.main.transform.position = new Vector3 (transform.position.x, -6.5f, -10);
			Camera.main.orthographicSize = 3;
			zzz.SetActive (true);
			zzz.GetComponent<Animator> ().Play ("sleep");
			skip.SetActive (true);
			disabledState = 1;
			inTut = true;
		}
		part = GetComponent<ParticleSystem> ();
		snakeHeadBite.SetActive (false);
		SnakeHead.SetActive (false);
		ropey = rope.GetComponent<ropeTextures> ();
		gm = GameMaster.GetComponent<gameMaster> ();
		rb = GetComponent<Rigidbody2D> ();
		distance = 30;
		line.enabled = false;
		gravOn = false;
		Physics2D.gravity = gravOff;
		skins = parent.GetComponentsInChildren<SpriteRenderer> ();
		opening = true;
	}

	IEnumerator tutorial (){
		snakeMiss.Play ("snakeanims");
		yield return new WaitForSeconds (4);
		while ((Camera.main.orthographicSize < 10) && (inTut == true)) {
			yield return new WaitForFixedUpdate();
			Vector3 newpos = new Vector3 (0, 0, -10);
			Camera.main.orthographicSize = Mathf.MoveTowards (Camera.main.orthographicSize, 10, 7 * Time.fixedDeltaTime);
			Camera.main.transform.position = Vector3.MoveTowards (Camera.main.transform.position, newpos, 7 * Time.fixedDeltaTime);
		}
		if (godray != null) {
			godray.Play ("godrayopen");
		}
		yield return new WaitForSeconds(2);
		if (tutbutton != null) {
			tutbutton.SetActive (true);
		}
		disabledState = 0;


		while (tutmoveon == false) {
			yield return null;
		}
		if (tutbutton != null) {
			tutbutton.GetComponent<Animator> ().Play ("fin");
		}
		Destroy (zzz);

		while (transform.position.y < -3f) {
			yield return null;
		}
		if (releaseButton != null) {
			releaseButton.SetActive (true);
		}
		Time.timeScale = 0f;
		gravOn = true;

		while (tutMoveOnAgain == false) {
			yield return null;
		}
		Destroy (releaseButton);
		Destroy (tutbutton);
		Time.timeScale = 1f;
		PlayerPrefs.SetInt ("firstTime", 0);
		Destroy (skip);
		inTut = false;
		yield break;
	}

	public void tutorialTwo (){
		tutmoveon = true;
	}

	public void tutorialThree () {
		tutMoveOnAgain = true;
	}

	public void tutorialSkip () {
		Camera.main.orthographicSize = 10;
		inTut = false;
		Camera.main.transform.position = new Vector3 (transform.position.x, 0, -10);
		Destroy (releaseButton);
		Destroy (tutbutton);
		Destroy (godray);
		Destroy (skip);
		Destroy (snakeMiss.gameObject);
		Destroy (zzz);
		disabledState = 0;
		PlayerPrefs.SetInt ("firstTime", 0);
		StopCoroutine ("tutorial");
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (PlayerPrefs.GetInt ("firstTime", 1) == 1) {

			StartCoroutine ("tutorial");
		}

		//gravitity
		if (gravOn == true) {
			Physics2D.gravity = gravOn1;
		}
	}

	private bool IsPointerOverGameObject( int fingerId )
	{
		EventSystem eventSystem = EventSystem.current;
		return ( eventSystem.IsPointerOverGameObject( fingerId )
			&& eventSystem.currentSelectedGameObject != null );
	}

		void Update () {

		tail.transform.localPosition = new Vector3 (0.9640498f, -1.442163f, 0);

		if (disabledState == 0) {
			foreach (SpriteRenderer skin in skins) {
				skin.color = new Color32 (255, 255, 255, 255);
			}
		} else if (disabledState == 1) {
			foreach (SpriteRenderer skin in skins) {
				skin.color = new Color32 (255, 255, 255, 255);
			}
		} else if (disabledState == 2) {
			foreach (SpriteRenderer skin in skins) {
				skin.color = new Color32 (255, 255, 0, 255);
			}
		} else if (disabledState == 3) {
			foreach (SpriteRenderer skin in skins) {
				skin.color = new Color32 (148, 30, 30, 255);
			}
		} 
			


		if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began)) {
			if ((EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId)) && (inTut == false)) {
				Debug.Log("Hit UI, Ignore Touch");
			} else {
				target = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				target.z = transform.position.z;
				currentPos = transform.position;
				currentPos.z = transform.position.z;
				//chck for collisions in the direction of click
				if (disabledState == 0) { 
					RaycastHit2D hit = Physics2D.Raycast (target, Vector3.up, distance, hits);
					if (hit.collider != null && opening == false) {
						StartCoroutine (hookshot (hit.point, currentPos));
						line.enabled = true;
						SnakeHead.SetActive (true);
					} else if (hit.collider != null && opening == true) {
						if (livesManager.ConsumeLife ()) {
							gm.business = true;
							StartCoroutine (hookshot (hit.point, currentPos));
							line.enabled = true;
							SnakeHead.SetActive (true);
							opening = false;
						} else {
							menuManager.noLivesOn ();
						}
					} else {
						return;
					}
				}
			}
		} 
	}
		

	IEnumerator hookshot (Vector3 strike, Vector3 pos) {
		float timePassed = 0;
		while (((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Stationary) || (Input.GetMouseButton (0))) && disabledState == 0) {
			head = Vector2.MoveTowards (pos, strike, 1.5f * (timePassed/Time.fixedDeltaTime));
			timePassed += Time.fixedDeltaTime;
			ropey.head = head;

			Vector2 headPos = (new Vector2 (transform.position.x, transform.position.y) - head);
			if (headPos != Vector2.zero) {
				headrot = Quaternion.LookRotation (headPos);
			}
			headrot *= Quaternion.Euler (0, 90, 0);
			SnakeHead.transform.rotation = headrot;
			SnakeHead.transform.position = new Vector3 (head.x, head.y, 0);

			line.SetPosition (1, transform.position);
			line.SetPosition (0, head);
			yield return new WaitForFixedUpdate();
			if (played == false) {
				FindObjectOfType<audioManager> ().play ("extend");
				played = true;
			}
			if (head == (Vector2)strike) {
				StartCoroutine (hookHit (timePassed, strike, pos));
			}
		}
		played = false;
		SnakeHead.SetActive (false);
		line.enabled = false;
		gravOn = true;
		rb.drag = 0.05f;
		FindObjectOfType<audioManager> ().play ("Swoosh");
	}

	IEnumerator hookHit (float timePassed, Vector3 strike, Vector3 pos) {
		while (((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Stationary) || (Input.GetMouseButton (0))) && disabledState == 0) {
			SnakeHead.SetActive (false);
			snakeHeadBite.SetActive (true);
			snakeHeadBite.transform.position = strike;

			rb.drag = 0;
			gravOn = false;
			Physics2D.gravity = gravOff;
			//move towards the hitpoint
			Vector2 direction = (strike - pos).normalized;
			rb.velocity = Vector2.zero;
			rb.AddForceAtPosition (direction * 1f * (timePassed / Time.fixedDeltaTime),transform.position);
			line.SetPosition (1, transform.position);
			line.SetPosition (0, strike);
			yield return new WaitForFixedUpdate();
		}
		played = false;
		SnakeHead.SetActive (false);
		snakeHeadBite.SetActive (false);
		line.enabled = false;
		gravOn = true;
		rb.drag = 0.05f;
		FindObjectOfType<audioManager> ().play ("Swoosh");
	}

	public IEnumerator stunned (float timeout) {
		FindObjectOfType<audioManager> ().play ("stunned");
		disabledState = 2;
		yield return new WaitForSeconds (timeout);
		disabledState = 0;
	}


	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.collider.tag == "floor" ) {
				gm.gamefin = true;
			gm.StartCoroutine ("gameOver");
				FindObjectOfType<audioManager> ().collision ();
		} 
		if (coll.collider.tag == "bounds") {
			if (disabledState == 0) {
				FindObjectOfType<audioManager> ().collision ();
				disabledState = 3;
			}
		}
		if (coll.collider.name == "brightfeathers" && coll.relativeVelocity.magnitude > 10 && disabledState == 0) {
				FindObjectOfType<audioManager> ().collision ();
				StartCoroutine (stunned (2));
		}
		if (coll.collider.tag == "croc" && coll.relativeVelocity.magnitude > 10 && disabledState == 0) {
				FindObjectOfType<audioManager> ().collision ();
				StartCoroutine (stunned (1));
		}
		if (coll.collider.tag == "vine") {
			if (disabledState == 0) {
				FindObjectOfType<audioManager> ().play ("deathTouch");
				disabledState = 3;
			}
		}
	}

	void OnTriggerExit2D (Collider2D coll) {
		if (coll is EdgeCollider2D) {
			if ((coll.tag == "spider") || (coll.tag == "peacock")) {
				Destroy (coll);
				PlayerPrefs.SetInt ("bonus", PlayerPrefs.GetInt ("bonus") + 5);
			} else if ((coll.tag == "croc") || (coll.tag == "vine")) {
				Destroy (coll);
				PlayerPrefs.SetInt ("bonus", PlayerPrefs.GetInt ("bonus") + 10);
				part.Emit (20);
			}
		}
	}
}

