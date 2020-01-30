﻿using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    private GameObject player;        //Public variable to store a reference to the player game object

    [SerializeField]
    private Vector3 offset;            //Private variable to store the offset distance between the player and camera

    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {

        

        if (player != null)
        {
            Vector3 playerTransform = player.transform.position;

            // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
            transform.position = new Vector3(playerTransform.x,  playerTransform.y, transform.position.z);
        }
    }
}