using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] float offsetZ;
    public float smoothing = 5f;
    Transform playerPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerPosition = FindAnyObjectByType<PlayerMovement>().transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        //position of the camera
        Vector3 targetPosition = new Vector3(playerPosition.position.x, transform.position.y, playerPosition.position.z - offsetZ);
        transform.position = Vector3.Lerp(transform.position,targetPosition, smoothing*Time.deltaTime );
    }
}