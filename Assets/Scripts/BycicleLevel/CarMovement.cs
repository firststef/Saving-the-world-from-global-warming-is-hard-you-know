﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarMovement : MonoBehaviour {

	Rigidbody2D rb;
    MapManager gameManagerMap;

    [SerializeField]
	float acceleration = 5f;
	[SerializeField]
	float steering = 3f;
	float steeringAmount, speed, direction;

    public bool HitCheckpoint;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
        HitCheckpoint = false;

        gameManagerMap = GameObject.Find("GameManagerMap").GetComponent<MapManager>();
        gameManagerMap.player.SetActive(false);

    }
	
	// Update is called once per frame
	void FixedUpdate () {

		steeringAmount = - Input.GetAxis ("Horizontal");
		speed = Input.GetAxis ("Vertical") * acceleration;
		direction = Mathf.Sign(Vector2.Dot (rb.velocity, rb.GetRelativeVector(Vector2.up)));
		rb.rotation += steeringAmount * steering * rb.velocity.magnitude * direction;

		rb.AddRelativeForce (Vector2.up * speed);

		rb.AddRelativeForce ( - Vector2.right * rb.velocity.magnitude * steeringAmount / 2);
		
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManagerMap.completedMiniGame = false;
            gameManagerMap.playingMiniGame = false;
            SceneManager.LoadScene(1);
            gameManagerMap.player.SetActive(true);
            MapManager.dangerPopupsHolder.SetActive(true);
            gameManagerMap.gameProgress.SetActive(true);
            GameObject.Find("Canvas").transform.Find("Bicycle").gameObject.SetActive(false);
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name=="CheckPoint")
            HitCheckpoint = true;

        if (other.name=="Finish")
        {
            if(HitCheckpoint)
            {
                GameObject.Find("Canvas").transform.Find("Bicycle").gameObject.SetActive(false);
                gameManagerMap.completedMiniGame = true;
                gameManagerMap.playingMiniGame = false;
                SceneManager.LoadScene(1);
                gameManagerMap.player.SetActive(true);
                MapManager.dangerPopupsHolder.SetActive(true);
                gameManagerMap.gameProgress.SetActive(true);
            }
        }
    }


}
