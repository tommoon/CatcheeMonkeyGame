using UnityEngine;
using System.Collections;

public class CameraTracksPlayer : MonoBehaviour {

	public GameObject player_go;
	Transform player;

	float offsetX;

	float currentPos;

	bool forward = true;
	public bool rewind = false;
	// Use this for initialization
	void Start () {
		
		if(player_go == null) {
			Debug.LogError("Couldn't find an object with tag 'Player'!");
			return;
		}

		player = player_go.transform;

		offsetX = transform.position.x - player.position.x;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if((player != null) && (forward)) {
			Vector3 pos = transform.position;
			pos.x = player.position.x + offsetX;
			transform.position = pos;
		}
		Vector3 viewPos = Camera.main.WorldToViewportPoint (player.transform.position);
		viewPos.x = Mathf.Clamp01 (viewPos.x);
		player.transform.position = Camera.main.ViewportToWorldPoint (viewPos);
		if (player.position.x < currentPos && rewind == false) {

			forward = false;
		} else if ((player.position.x > currentPos) && (forward == false)) {
			StartCoroutine (offScreen(viewPos.x));
			}

	


		currentPos = player.position.x;

	}
	IEnumerator offScreen (float xpos){
		while (xpos < 0.5f) {
			yield return null;
		}
		forward = true;
	}
}
