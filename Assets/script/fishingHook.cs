using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishingHook : MonoBehaviour {


	//rigidbody of the hook
	Rigidbody2D rb;
	//collider of the hook
	Collider2D collide;
	//whether or not hook is dragging an object back to the player
	bool dragging = false;
	//whether or not the player is being dragged to the hook
	bool draggingplayer = false;
	//reference to the player object
	GameObject Player;
	//whether or not the hook missed and is returning to the player
	bool miss = false;
	//how far the hook has traveled (number of frames)
	float dist = 0.0f;
	//the direciton the grapple was thrown in (used to simulate momentum)
	Vector3 grappledirection;
	//scale of the object the hook is dragging
	Vector3 savescale;


	// Use this for initialization
	void Start () {
		//initializing
		rb = transform.GetComponent<Rigidbody2D>();
		collide = transform.GetComponent<Collider2D>();
		Player = GameObject.FindGameObjectWithTag("Player");


		grappledirection = rb.velocity;
		grappledirection.Normalize();

	}
	
	// Update is called once per frame
	void Update () {
		//movement if something is being dragged or if a miss occurs
		if (dragging)
			transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, 0.5f);
		if (miss)
			transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, 0.5f);
		//movement if player is being dragged to hook
		if(draggingplayer)
			Player.transform.position = Vector2.MoveTowards(Player.transform.position, transform.position, 0.5f);
		//once hook has met with player or player has met with hook
		if((transform.position == Player.transform.position)){
			//detach and restore any dragged objects
			if(dragging){
				Transform thing = transform.GetChild(0);
				thing.GetComponent<Collider2D>().enabled = true;
				if (thing.CompareTag("moveable"))
					thing.GetComponent<Rigidbody2D>().isKinematic = false;
				transform.DetachChildren();
				//suppress weird growing glitch
				thing.transform.localScale = savescale;
			}


			
			Destroy(this.gameObject);
			//apply velocity to player to simulate momentum when getting pulled by hook
			if(draggingplayer)
				Player.transform.GetComponent<Rigidbody2D>().velocity = grappledirection * 20;



		}
		if (dist >= 15.0f && !dragging && !draggingplayer){
			miss = true;
			rb.velocity = Vector2.zero;
		}
		dist += 1;
	}

	void OnTriggerEnter2D(Collider2D target){
		if ((target.CompareTag("moveable") || target.CompareTag("OxygenPickup")) && !dragging){
			GameObject drag = target.gameObject;
			drag.GetComponent<Collider2D>().enabled = false;
			drag.GetComponent<Rigidbody2D>().isKinematic = true;
			savescale = drag.transform.localScale;

			drag.transform.parent = transform;
			//drag.transform.localPosition = Vector2.right;
			dragging = true;
			try{
			rb.velocity = Vector2.zero;
			}catch{

			}
		}
		else if(target.CompareTag("Platform")){

			draggingplayer = true;
			try{
			rb.velocity = Vector2.zero;
			}catch{
			}
		}
	}


}
