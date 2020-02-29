using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using plats.Assets.script;

public class playerMovement : MonoBehaviour {

	Rigidbody2D rb2d;

	//object which contains all externalized configs
	configVals configInterface = new configVals();
	//layer for all ground objects for jumping
	public LayerMask groundLayer;
	//player's box collider
	BoxCollider2D playerCollider;
	//bool for checking if the player inputed jump on a given frame
	bool jump;
	//float for storing horizontal axis value
	float xAxisVal;
	//how fast the player will jump
	public float jumpVelocity;
	//how much faster the player will fall relative to rising
	public float fallMultiplier;
	//how much shorter the short jump will be than the tall jump
	public float lowJumpMultiplier;
	//reference for storing the sprite of the player
	SpriteRenderer playerSprite;
	//the value for storing the oyxgen level, default set to full or max
	public float globalBrightness = 1.0f;
	//the value that the player's velocity will be assignned to when moving on the ground
	Vector2 playerMoveSpeedVector = new Vector2(5f, 0f);
	//the factor by which the player's mobility (amount that they can influence their speed) will be reduced in the air
	public float airMoveMult = 15.0f;
	//the sprite for the background
	public SpriteRenderer bckg;

	//check to see if the grapple is currently flying
	bool isGrappling = false;
	//empty list for counting the number of an object
	GameObject[] gameObjects;
	//object to use as hook
	public GameObject grappleObj;
	//bool to see whether or not to fire hook
	bool fireGrapple = false;
	//which direction the player is facing
	public float facing;
	//referencing animator
	public Animator anima;


	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
		//fetch player sprite from unity object
		playerSprite = GetComponent<SpriteRenderer>();
		playerCollider = GetComponent<BoxCollider2D>();
		jumpVelocity = configInterface.jumpVelocity;
		fallMultiplier = configInterface.fallMultiplier;
		lowJumpMultiplier = configInterface.lowJumpMultiplier;
	}
	
	// Update is called once per frame
	void Update () {
		//check for left and right movement
		xAxisVal = Input.GetAxisRaw("Horizontal");
		anima.SetFloat("Blend", xAxisVal);

		if (xAxisVal != 0)
			facing = xAxisVal;


		//check for grounded and if the jump button is being pressed
		if (IsGrounded() && Input.GetButtonDown("Jump") && !isGrappling){
		jump = true;
		}
		//if shooting check to see when you can give player control back
		if(isGrappling){
			gameObjects = GameObject.FindGameObjectsWithTag("grapple");
			if (gameObjects.Length == 0)
				isGrappling = false;
		}

		//check for fire
		if(Input.GetButtonDown("Fire") && !isGrappling){

			isGrappling = true;
			fireGrapple = true;

		}




		//decrement oxygen
		if (globalBrightness > 0.0f){
			globalBrightness -= Time.deltaTime/40;

		}


		//change sprite color according to oxygen
		bckg.color = Color.Lerp(Color.red, Color.green, globalBrightness);
		if(globalBrightness < 0){
			Destroy(this.gameObject);
		}
	}





	//check for pickups and other trigger collisions  
	void OnTriggerEnter2D(Collider2D target){
		if (target.CompareTag("OxygenPickup")){
			globalBrightness = 1f;
		}
	}

	void FixedUpdate(){
		
		//move left and right on the ground
		if(IsGrounded()){
			rb2d.velocity = playerMoveSpeedVector * xAxisVal;
			anima.SetFloat("Grounded", 1f);
		}
		//aerial movement
		else{
			anima.SetFloat("Grounded", 0f);
			if(xAxisVal > 0)
				if(rb2d.velocity.x <= 0 || rb2d.velocity.x < playerMoveSpeedVector.x)
					rb2d.velocity += (playerMoveSpeedVector*xAxisVal)/airMoveMult;
			if(xAxisVal < 0)
				if (rb2d.velocity.x >= 0 || rb2d.velocity.x > - playerMoveSpeedVector.x)
					rb2d.velocity += (playerMoveSpeedVector*xAxisVal)/airMoveMult;
		}
		//preform jump
		if (jump)
			rb2d.velocity += Vector2.up * jumpVelocity;


		//long jumps and fast falling
		if(rb2d.velocity.y < 0)
			rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		else if (rb2d.velocity.y > 0 && !Input.GetButton("Jump"))
			rb2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;

		if(fireGrapple){
			GameObject newhook = Object.Instantiate(grappleObj, transform.position, Quaternion.identity);
			Vector2 direction = new Vector2(10*facing,0);
			direction.Normalize();
			newhook.GetComponent<Rigidbody2D>().velocity = direction*20;
			fireGrapple = false;
		}
	    jump = false;

	}
	//function for checking if the player is grounded
	bool IsGrounded()
    {

        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);

        if (hit.collider != null)
            return true;

        return false;

    }




}
