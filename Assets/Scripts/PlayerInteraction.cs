using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    PlayerMovement playerMovement;
    Land selectedLand = null;
    InteractableObject selectedInteractable = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = transform.parent.GetComponent<PlayerMovement>();
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 1))
        {
            OnInteractableHit(hit);
        }
        
    }

    void OnInteractableHit(RaycastHit hit)
    {
        Collider other = hit.collider;

        if(other.tag == "Land")
        {
            Land land = other.GetComponent<Land>();
            SelectLand(land);
            return;
        }

        //deselect the land if the player is not standing on any land
        if (selectedLand != null)
        {
            selectedLand.Select(false);
            selectedLand = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            selectedInteractable = other.GetComponent<InteractableObject>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // 1. Leaving Land
        if (other.CompareTag("Land"))
        {
            Land land = other.GetComponent<Land>();
            // Only deselect if we are leaving the SPECIFIC land tile we currently have selected
            if (land == selectedLand)
            {
                selectedLand.Select(false);
                selectedLand = null;
            }
        }
        // 2. Leaving Item
        else if (other.CompareTag("Item"))
        {
            InteractableObject item = other.GetComponent<InteractableObject>();
            // Only deselect if we are leaving the SPECIFIC item we have selected
            if (item == selectedInteractable)
            {
                selectedInteractable = null;
            }
        }
    }

    void SelectLand(Land land)
    {
        if (selectedLand != null)
        {
            selectedLand.Select(false);
        }
        selectedLand = land;
        land.Select(true);
    }

    public void Interact()
    {
        // player shouldnt be able to use a tool when hands full with an item
        if(InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
        {
            return;
        }

        if (selectedLand != null)
        {
            selectedLand.Interact();
            return;
        }

        Debug.Log("Not on any Land");


    }

    public void ItemInteract()
    {
        //if the player is holding sth, keep it in inventory
        if(InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Item))
        {
            InventoryManager.Instance.HandToInventory(InventorySlot.InventoryType.Item);
            return;
        }

        // if the player isn't holding anything, pick up an item

        //check if there is an interactable selected
        if(selectedInteractable != null)
        {
            // pick up
            selectedInteractable.Pickup();
        }


    }

}
