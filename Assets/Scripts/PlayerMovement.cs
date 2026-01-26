using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float playerSpeed;
    private CharacterController controller;

    PlayerInteraction playerInteraction;
    private Animator animator;
    private float gravity = 9.81f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInteraction = GetComponentInChildren<PlayerInteraction>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Interact();
        
        // debuggging purpose only
        // skip the time when the right square bracket is pressed 
        if(Input.GetKey(KeyCode.RightBracket))
        {
            TimeManager.Instance.Tick();
        }
    }

    public void Interact()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            playerInteraction.Interact();
        }

        //item interaction
        if (Input.GetButtonDown("Fire2"))
        {
            playerInteraction.ItemInteract();
        }

        
    }

    public void Move()
    {
        float directionX = Input.GetAxisRaw("Horizontal");
        float directionY = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(directionX, 0f, directionY).normalized;
        Vector3 velocity = playerSpeed * Time.deltaTime * dir;

        if (controller.isGrounded)
        {
            velocity.y = 0; 
        }
        velocity.y -= Time.deltaTime * gravity; 

        if(dir.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            float rotationSpeed = 10f;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,         
                targetRotation,             
                Time.deltaTime * rotationSpeed 
            );
        }

        controller.Move(velocity);

    }
}
