using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oxygenPickup : MonoBehaviour {

	PolygonCollider2D col;
	public LayerMask playlayer;
	// Use this for initialization
	void Start () {
		col = GetComponent<PolygonCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	//check for collisions with the player
	void OnTriggerEnter2D(Collider2D target)
    {
        if(target.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
