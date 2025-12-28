using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance{ get; private set;}

    private void Awake()
    {
        //if there is more than one instance, destroy the extra
        if(Instance!= null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //setting the static instance to this instance
            Instance = this;
        }
    }

    //tool slots
    [Header("Tools")]
    public ItemData[] tools = new ItemData[8];
    public ItemData equippedTool = null;

    //Item slots
    [Header("Items")]
    public ItemData[] items = new ItemData[8];
    public ItemData equippedItem = null;

    //for player to hold item
    public Transform handPoint;

    //Equipping

    //Handles movement of item from Inventory to Hand
    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            //Cache the Inventory slot ItemData from InventoryManager
            ItemData itemToEquip = items[slotIndex];

            //Change the Inventory slot to the Hand Slot
            items[slotIndex] = equippedItem;

            //Change the Hand Slot to the Inventory Slot
            equippedItem = itemToEquip; 

            // update changes in scene
            RenderHand();

        }else
        {
            //Cache the Inventory slot ItemData from InventoryManager
            ItemData toolToEquip = tools[slotIndex];

            //Change the Inventory slot to the Hand Slot
            tools[slotIndex] = equippedTool;

            //Change the Hand Slot to the Inventory Slot
            equippedTool = toolToEquip;
        }
        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
    }

    //Handles movement of item from Hand to Inventory
    public void HandToInventory(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            //Iterate through each inventory slot and find an empty slot
            for(int i = 0; i < items.Length; i++)
            {
                if(items[i] == null)
                {
                    //Send the equipped item over to its new slot
                    items[i] = equippedItem;
                    //Remove the item from the hand
                    equippedItem = null;
                    break;
                }
            }
            RenderHand();

        }
        else
        {
            //Iterate through each inventory slot and find an empty slot
            for(int i = 0; i < tools.Length; i++)
            {
                if(tools[i] == null)
                {
                    //Send the equipped item over to its new slot
                    tools[i] = equippedTool;
                    //Remove the item from the hand
                    equippedTool = null;
                    break;
                }
            }

        }
        //Update changes in the inventory
        UIManager.Instance.RenderInventory();
    }

    // handle the player's equipped item in the scene
    public void RenderHand()
    {
        // reset objects on hand
        if(handPoint.childCount > 0)
        {
            Destroy(handPoint.GetChild(0).gameObject);
        }
        // instantiate the game model on the player's hand and put it on screen
        if(equippedItem != null)
        {
            Instantiate(equippedItem.gameModel, handPoint);
        }
    }

    void Start()
    {
          
    }
    void Update()
    {

    }

}
