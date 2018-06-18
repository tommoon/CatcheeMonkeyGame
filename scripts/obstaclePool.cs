using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstaclePool : MonoBehaviour {

	public GameObject player;

	float GcurrentPos;
	float GDistTraveled;
	public int godRayPoolSize = 5;
	public int currentGodRay = 0;
	public GameObject godRayPrefab;
	public float GSpawnrate = 25;
	public float gMin = 1f;
	public float gMax = 2f;
	float gChance = 0;

	float pCurrentPos;
	float pDistTraveled;
	public int peacockPoolSize = 5;
	private int currentPeacock = 0;
	public GameObject peackockPrefab;
	public float pSpawnrate = 20f;
	public float pMin = -7f;
	public float pMax = 7f;
	float pChance = 0;

	float sCurrentPos;
	float sDistTraveled;
	public int spiderPoolSize = 5;
	private int currentSpider = 0;
	public GameObject spiderPrefab;
	public float sSpawnrate = 20f;
	public float sMin = -7f;
	public float sMax = 7f;
	float sChance = 0;

	float cCurrentPos;
	float cDistTraveled;
	public int crocPoolSize = 5;
	private int currentCroc = 0;
	public GameObject crocPrefab;
	public float cSpawnrate = 20f;
	public float cMin = -11f;
	public float cMax = -2f;
	float wChance = 0;

	public int vinePoolSize = 5;
	private int currentVine = 0;
	public GameObject vinePrefab;
	public float vSpawnrate = 20f;
	public float vMin = 11f;
	public float vMax = 2f;

	public float wMin = 5f;
	public float wMax = 12f;

	private GameObject[] peacocks;
	private GameObject[] siders;
	private GameObject[] crocodiles;
	private GameObject[] vines;
	private GameObject[] godrays;

	private float spawnXpos;

	private Vector2 peacockPoolPosition = new Vector2(-40,-40);
	private Vector2 spiderPoolPosition = new Vector2(-40,-40);
	private Vector2 crocodilePoolPosition = new Vector2(-40,40);
	private Vector2 vinePoolPosition = new Vector2(-40,40);
	private Vector2 godRayPoolPosition = new Vector2 (-40, 40);

	void randomRanges () {
		sChance = Random.value;
		pChance = Random.value;
		wChance = Random.value;
		gChance = Random.value;
	}
	// Use this for initialization
	void Start () {

		InvokeRepeating ("randomRanges", 0, 2f);

		pSpawnrate = 20f;
		pCurrentPos = 0;
		sCurrentPos = 0;
		sSpawnrate = 30f;
		cSpawnrate = 45f;
		cCurrentPos = 0;
		vSpawnrate = 45f;
		GSpawnrate = 50f;
		GcurrentPos = 0;


		peacocks = new GameObject[peacockPoolSize];
		for (int i = 0; i < peacockPoolSize; i++) {
			peacocks [i] = GameObject.Instantiate (peackockPrefab, peacockPoolPosition, Quaternion.identity);
		}

		godrays = new GameObject[godRayPoolSize];
		for (int i = 0; i < godRayPoolSize; i++) {
			godrays [i] = GameObject.Instantiate (godRayPrefab, godRayPoolPosition, Quaternion.identity);
		}

		siders = new GameObject[spiderPoolSize];
		for (int i = 0; i < spiderPoolSize; i++) {
			siders [i] = GameObject.Instantiate (spiderPrefab, spiderPoolPosition, Quaternion.identity);
		}

		crocodiles = new GameObject[crocPoolSize];
		for (int i = 0; i < crocPoolSize; i++) {
			crocodiles [i] = GameObject.Instantiate (crocPrefab, crocodilePoolPosition, Quaternion.identity);
		}

		vines = new GameObject[vinePoolSize];
		for (int i = 0; i < vinePoolSize; i++) {
			vines [i] = GameObject.Instantiate (vinePrefab, vinePoolPosition, Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () {

		pDistTraveled = player.transform.position.x - pCurrentPos;
		sDistTraveled = player.transform.position.x - sCurrentPos;
		cDistTraveled = player.transform.position.x - cCurrentPos;
		GDistTraveled = player.transform.position.x - GcurrentPos;

		if ((GDistTraveled >= GSpawnrate) && (gChance < 0.8f)) {
			spawnXpos = player.transform.position.x + 26f;
			GcurrentPos = player.transform.position.x;
			float spawnWidth = Random.Range (gMin, gMax);
			Vector2 spawnPos = new Vector2 (spawnXpos, 0);
			godrays [currentGodRay].transform.position = spawnPos;
			Vector3 raywidth = new Vector3 (spawnWidth,1.914213f, 1);
			godrays [currentGodRay].transform.localScale = raywidth;
			currentGodRay++;
			if (currentGodRay >= godRayPoolSize) {
				currentGodRay = 0;
			}
		}

		if ((pDistTraveled >= pSpawnrate) && (pChance < 0.3f)) {
			spawnXpos = player.transform.position.x + 26f;
			pCurrentPos = player.transform.position.x;
			float spawnYposition = Random.Range (pMin, pMax);
			Collider2D pcheck = Physics2D.OverlapCircle ((new Vector2 (spawnXpos, spawnYposition)), 3f);
			if (pcheck == null) {
				Vector2 spawnPos = new Vector2 (spawnXpos, spawnYposition);
				peacocks [currentPeacock].transform.position = spawnPos;
				currentPeacock++;
				if (currentPeacock >= peacockPoolSize) {
					currentPeacock = 0;
				}
			}
		}

		if ((cDistTraveled >= cSpawnrate) && (wChance < 0.4f)) {
			spawnXpos = player.transform.position.x + 26f;
			cCurrentPos = player.transform.position.x;
			float spawnYposition = Random.Range (cMin, cMax);
				crocodiles [currentCroc].transform.position = new Vector2 (spawnXpos, spawnYposition);
				currentCroc++;
				if (currentCroc >= crocPoolSize) {
					currentCroc = 0;
				}
		} else if ((cDistTraveled >= cSpawnrate) && (wChance > 0.6f)) {
			spawnXpos = player.transform.position.x + 26f;
			cCurrentPos = player.transform.position.x;
			float spawnYposition = Random.Range (vMin, vMax);
				vines [currentVine].transform.position = new Vector2 (spawnXpos, spawnYposition);
				currentVine++;
				if (currentVine >= vinePoolSize) {
					currentVine = 0;
				}
		} else if ((cDistTraveled >= cSpawnrate) && (0.4f <= wChance) && (wChance <= 0.6f)) {
			spawnXpos = player.transform.position.x + 26f;
			cCurrentPos = player.transform.position.x;
			float spawnYposition = Random.Range (wMin, wMax);
				vines [currentVine].transform.position = new Vector2 (spawnXpos, spawnYposition);
				currentVine++;
				if (currentVine >= vinePoolSize) {
					currentVine = 0;
				}
				crocodiles [currentCroc].transform.position = new Vector2 (spawnXpos, spawnYposition - 19f);
				currentCroc++;
				if (currentCroc >= crocPoolSize) {
					currentCroc = 0;
			}
		}


		if ((sDistTraveled >= sSpawnrate) && (sChance <0.5f)) {
			spawnXpos = player.transform.position.x + 26f;
			sCurrentPos = player.transform.position.x;
			float spawnYposition = Random.Range (sMin, sMax);
			siders [currentSpider].transform.position = new Vector2 (spawnXpos, spawnYposition);
			currentSpider++;
			if (currentSpider >= spiderPoolSize) {
				currentSpider = 0;
			}
		}
	}
}
