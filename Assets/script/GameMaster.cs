using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameMaster : MonoBehaviour {


	GameObject[] gos;
	SpriteRenderer objsprite;
	bool collected = false;
	public Text wintext;

	// Use this for initialization
	void Start () {
		objsprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

		gos = GameObject.FindGameObjectsWithTag("OxygenPickup");
		if(gos.Length == 0){

			objsprite.color = Color.blue;
			collected = true;

		}
		
	}

	void OnTriggerEnter2D(Collider2D target){
		if (target.CompareTag("Player") && collected){
			wintext.text = "got em all!";
		}
	}



}
