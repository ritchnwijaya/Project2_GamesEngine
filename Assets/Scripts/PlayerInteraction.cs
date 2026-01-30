using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Land Detection")]
    [SerializeField] private float landRayDistance = 1.1f;

    private PlayerMovement playerMovement;

    private Land selectedLand = null;
    private InteractableObject selectedInteractable = null;

    void Start()
    {
        // Robust: works whether this script is on the player root or a child
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    void Update()
    {
        // Detect land directly beneath the player
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, landRayDistance))
        {
            OnInteractableHit(hit);
        }
        else
        {
            // Nothing under us -> deselect land
            DeselectLand();
        }
    }

    private void OnInteractableHit(RaycastHit hit)
    {
        Collider other = hit.collider;

        if (other != null && other.CompareTag("Land"))
        {
            Land land = other.GetComponent<Land>();
            if (land != null) SelectLand(land);
            return;
        }

        // Not land -> deselect current land if any
        DeselectLand();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            selectedInteractable = other.GetComponent<InteractableObject>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Leaving Land
        if (other.CompareTag("Land"))
        {
            Land land = other.GetComponent<Land>();
            if (land != null && land == selectedLand)
            {
                DeselectLand();
            }
        }
        // Leaving Item
        else if (other.CompareTag("Item"))
        {
            InteractableObject item = other.GetComponent<InteractableObject>();
            if (item != null && item == selectedInteractable)
            {
                selectedInteractable = null;
            }
        }
    }

    private void SelectLand(Land land)
    {
        if (selectedLand == land) return;

        if (selectedLand != null)
            selectedLand.Select(false);

        selectedLand = land;
        selectedLand.Select(true);
    }

    private void DeselectLand()
    {
        if (selectedLand == null) return;
        selectedLand.Select(false);
        selectedLand = null;
    }

    /// <summary>
    /// Call this from your input (e.g., Fire1) to START the tool use animation.
    /// The actual land interaction should be triggered via Animation Event calling DoLandToolAction().
    /// </summary>
    public bool CanStartLandToolAction()
    {
        // player shouldn't be able to use a tool when hands full with an item
        if (InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
            return false;

        return selectedLand != null;
    }

    /// <summary>
    /// This is the "real" tool action (watering/hoeing/etc.).
    /// Call this via an Animation Event at the correct frame.
    /// </summary>
    public void DoLandToolAction()
    {
        // player shouldn't be able to use a tool when hands full with an item
        if (InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
            return;

        if (selectedLand == null)
        {
            Debug.Log("Not on any Land");
            return;
        }

        if (PlayerStats.Stamina <= 0)
        {
            Debug.Log("Too tired to work!");
            return;
        }

        PlayerStats.UseStamina(2);
        selectedLand.Interact();
    }

    // Backwards compatible: if old code still calls Interact(), route it to the new method.
    // You can later delete Interact() once everything uses animation events.
    public void Interact()
    {
        DoLandToolAction();
    }

    public void ItemInteract()
    {
        // if there is an interactable selected, pick up an item
        if (selectedInteractable != null)
        {
            selectedInteractable.Pickup();
        }
    }

    public void ItemKeep()
    {
        // If the player is holding something, keep it in inventory
        if (InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
        {
            InventoryManager.Instance.HandToInventory(InventorySlot.InventoryType.Item);
        }
    }
}
