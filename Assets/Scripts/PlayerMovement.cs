using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float sprintMultiplier = 1.6f;

    [Header("Run Timing")]
    [SerializeField] float runAfterSeconds = 2f;   // erst nach 2s laufen -> rennen
    private float moveHeldTime = 0f;

    [Header("Gravity")]
    [SerializeField] float gravity = -9.81f;
    private float verticalVelocity;

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

        // Safety: falls du vergisst es im Inspector zu setzen
        if (model == null && animator != null)
            model = animator.transform;
    }

    void Update()
    {
        Move();
        Interact();

        if (Input.GetKey(KeyCode.RightBracket))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Advance the entire day
                for (int i = 0; i < 60 * 24; i++)
                    TimeManager.Instance.Tick();
            }
            else
            {
                TimeManager.Instance.Tick();
            }
        }
    }

    public void Interact()
    {
        // Safety
        if (playerInteraction == null) return;
        if (animator == null) return;

        // Use tool (Fire1 = meistens Linksklick)
        // Fallback zusätzlich auf Mouse0, falls Fire1 im Projekt nicht gemappt ist
        if (Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0))
        {
            // nur wenn wir auf Land stehen und nicht gerade ein Item in der Hand halten
            if (!playerInteraction.CanStartLandToolAction())
                return;

            // nur wenn ein Tool equipped ist
            if (!InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Tool))
                return;

            // welches Tool ist equipped?
            ItemData equipped = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Tool);

            if (equipped is SeedData)
            {
                animator.SetTrigger("Water"); // Or use a specific "Plant" trigger if you have one
                playerInteraction.DoLandToolAction(); 
                return;
            }
            EquipmentData tool = equipped as EquipmentData;
            if (tool == null)
                return;

            // optional: Trigger resetten, damit nix "hängen bleibt"
            animator.ResetTrigger("Water");
            animator.ResetTrigger("Dig");
            animator.ResetTrigger("SwingHoe");
            animator.ResetTrigger("SwingAxe");
            animator.ResetTrigger("SwingPick");

            // richtigen Trigger setzen (deine Triggernamen aus dem Animator)
            switch (tool.toolType)
            {
                case EquipmentData.ToolType.WateringCan:
                    animator.SetTrigger("Water");
                    break;

                case EquipmentData.ToolType.Hoe:
                    animator.SetTrigger("SwingHoe");
                    break;

                case EquipmentData.ToolType.Axe:
                    animator.SetTrigger("SwingAxe");
                    break;

                case EquipmentData.ToolType.Pickaxe:
                    animator.SetTrigger("SwingPick");
                    break;

                case EquipmentData.ToolType.Shovel:
                    animator.SetTrigger("Dig");
                    break;
            }

            // Action ausführen (ohne Animation Events)
            playerInteraction.DoLandToolAction();
        }

        // Pickup/store (Fire2)
        if (Input.GetButtonDown("Fire2"))
        {
            if (InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
            {
                animator.SetTrigger("StoreItem");
            }

            playerInteraction.ItemInteract();
        }

        // Keep items (Fire3)
        if (Input.GetButtonDown("Fire3"))
        {
            playerInteraction.ItemKeep();
        }
    }

    public void Move()
    {
        if (controller == null || !controller.enabled) return;

        float directionX = Input.GetAxisRaw("Horizontal");
        float directionY = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(directionX, 0f, directionY).normalized;

        bool isMoving = dir.magnitude > 0.01f;

        // Timer: zählt nur hoch, solange du wirklich läufst
        if (isMoving) moveHeldTime += Time.deltaTime;
        else moveHeldTime = 0f;

        // Run-Bedingung: Shift gedrückt UND schon lange genug am Laufen
        bool wantsRun = Input.GetKey(KeyCode.LeftShift);
        bool isRunning = isMoving && wantsRun && moveHeldTime >= runAfterSeconds;

        // Gravity (CharacterController braucht das manuell)
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f; // kleiner Downforce, damit er am Boden bleibt

        verticalVelocity += gravity * Time.deltaTime;

        // Rotation nur am Model
        if (isMoving && model != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            model.rotation = Quaternion.Slerp(model.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Speed: erst Walk, dann Run (nach Timer)
        float currentSpeed = playerSpeed * (isRunning ? sprintMultiplier : 1f);

        // Move
        Vector3 move = dir * currentSpeed;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // Animator Parameter setzen
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRunning", isRunning);
    }
}
