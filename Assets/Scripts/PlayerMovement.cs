using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float sprintMultiplier = 1.6f;

    [Header("References")]
    [SerializeField] Transform model; // Model-Child hier reinziehen

    private CharacterController controller;
    private Animator animator;
    private PlayerInteraction playerInteraction;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        playerInteraction = GetComponentInChildren<PlayerInteraction>();
    }

    void Update()
    {
        Move();
        Interact();

        if (Input.GetKey(KeyCode.RightBracket))
        {
            TimeManager.Instance.Tick();
        }
    }

    public void Interact()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (animator != null)
                animator.SetTrigger("Harvest");

            playerInteraction.Interact();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (animator != null &&
                InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
            {
                animator.SetTrigger("StoreItem");
            }

            playerInteraction.ItemInteract();
        }
    }

    public void Move()
    {
        float directionX = Input.GetAxisRaw("Horizontal");
        float directionY = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(directionX, 0f, directionY).normalized;

        // 👉 Rotation NUR auf dem Model
        if (dir.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            model.rotation = Quaternion.Slerp(
                model.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }

        bool sprint = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = playerSpeed * (sprint ? sprintMultiplier : 1f);

        Vector3 velocity = currentSpeed * Time.deltaTime * dir;
        controller.Move(velocity);

        animator.SetFloat("Speed", dir.magnitude * (sprint ? 1f : 0.5f));
    }
}
