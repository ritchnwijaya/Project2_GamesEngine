using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance{ get; private set;}

    private void Awake()
    {
        if(Instance!= null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        } 
    }

    public ItemIndex itemIndex; 
    //tool slots
    [Header("Tools")]
    [SerializeField]
    private ItemSlotData[] toolSlots = new ItemSlotData[8];
    [SerializeField]
    private ItemSlotData equippedToolSlot = null;

    //Item slots
    [Header("Items")]
    [SerializeField]
    private ItemSlotData[] itemSlots = new ItemSlotData[8];
    [SerializeField]
    private ItemSlotData equippedItemSlot = null;


    //for player to hold item
    public Transform handPoint;

    public void LoadInventory(ItemSlotData[] toolSlots, ItemSlotData equippedToolSlot, ItemSlotData[] itemSlots, ItemSlotData equippedItemSlot)
    {
        this.toolSlots = toolSlots;
        this.equippedToolSlot = equippedToolSlot;
        this.itemSlots = itemSlots;
        this.equippedItemSlot = equippedItemSlot;
        UIManager.Instance.RenderInventory();
        RenderHand();
    }


    //Handles movement of item from Inventory to Hand
    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        //The slot to equip (Tool by default)
        ItemSlotData handToEquip = (inventoryType == InventorySlot.InventoryType.Item) ? equippedItemSlot : equippedToolSlot;
        ItemSlotData[] inventoryToAlter = (inventoryType == InventorySlot.InventoryType.Item) ? itemSlots : toolSlots;


        if (handToEquip.Stackable(inventoryToAlter[slotIndex]))
        {
            ItemSlotData slotToAlter = inventoryToAlter[slotIndex];

            handToEquip.AddQuantity(slotToAlter.quantity);

            slotToAlter.Empty();

        } else
        {
            //Not stackable 
            ItemSlotData tempInventorySlot = new ItemSlotData(inventoryToAlter[slotIndex]);
        
            // Move hand to inventory
            inventoryToAlter[slotIndex] = new ItemSlotData(handToEquip);
        
            // Move inventory back to the CORRECT hand slot
            if (inventoryType == InventorySlot.InventoryType.Item) {
                equippedItemSlot = tempInventorySlot;
            } else {
                equippedToolSlot = tempInventorySlot;
        }
        }

        //Update the changes to the UI
        RenderHand();
        UIManager.Instance.RenderInventory();
    }

    //Handles movement of item from Hand to Inventory
    public void HandToInventory(InventorySlot.InventoryType inventoryType)
    {
        ItemSlotData handSlot = equippedToolSlot;
        //The array to change
        ItemSlotData[] inventoryToAlter = toolSlots;

        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            handSlot = equippedItemSlot;
            inventoryToAlter = itemSlots;
        }

        if (!StackItemToInventory(handSlot, inventoryToAlter))
        {
            //Find an empty slot to put the item in
            for (int i = 0; i < inventoryToAlter.Length; i++)
            {
                if (inventoryToAlter[i].IsEmpty())
                {
                    //Send the equipped item over to its new slot
                    inventoryToAlter[i] = new ItemSlotData(handSlot);
                    //Remove the item from the hand
                    handSlot.Empty();
                    break;
                }
            }
        }

        //Update the changes in the scene
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            RenderHand();
        }

        //Update changes in the inventory
        UIManager.Instance.RenderInventory();
    }

    public bool StackItemToInventory(ItemSlotData itemSlot, ItemSlotData[] inventoryArray)
    {
        
        for (int i = 0; i < inventoryArray.Length; i++)
        {
            if (inventoryArray[i].Stackable(itemSlot))
            {
                //Add to the inventory slot's stack
                inventoryArray[i].AddQuantity(itemSlot.quantity);
                //Empty the item slot
                itemSlot.Empty();
                return true; 
            }
        }

        //Can't find any slot that can be stacked
        return false; 
    }

    public void ShopToInventory(ItemSlotData itemSlotToMove)
    {
        //The inventory array to change
        ItemSlotData[] inventoryToAlter = IsTool(itemSlotToMove.itemData) ? toolSlots : itemSlots; 

        //Try stacking the hand slot. 
        //Check if the operation failed
        if (!StackItemToInventory(itemSlotToMove, inventoryToAlter))
        {
            //Find an empty slot to put the item in 
            //Iterate through each inventory slot and find an empty slot
            for (int i = 0; i < inventoryToAlter.Length; i++)
            {
                if (inventoryToAlter[i].IsEmpty())
                {
                    inventoryToAlter[i] = new ItemSlotData(itemSlotToMove);
                    break;
                }
            }

        }

        //Update the changes to the UI
        UIManager.Instance.RenderInventory();
        RenderHand();
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
        if(SlotEquipped(InventorySlot.InventoryType.Item))
        {
            Instantiate(GetEquippedSlotItem(InventorySlot.InventoryType.Item).gameModel, handPoint);
        }
    }

    //Inventory Slot Data 
    //Get the slot item (ItemData) 
    public ItemData GetEquippedSlotItem(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            return equippedItemSlot.itemData;
        }
        return equippedToolSlot.itemData; 
    }

    //Get function for the slots (ItemSlotData)
    public ItemSlotData GetEquippedSlot(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return equippedItemSlot;
        }
        return equippedToolSlot;
    }

    //Get function for the inventory slots
    public ItemSlotData[] GetInventorySlots(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return itemSlots;
        }
        return toolSlots;
    }

    //Check if a hand slot has an item
    public bool SlotEquipped(InventorySlot.InventoryType inventoryType)
    {
        if (inventoryType == InventorySlot.InventoryType.Item)
        {
            return !equippedItemSlot.IsEmpty();
        }
        return !equippedToolSlot.IsEmpty();
    }

    //Check if the item is a tool
    public bool IsTool(ItemData item)
    {
        //Is it equipment? 
        //Try to cast it as equipment first
        EquipmentData equipment = item as EquipmentData;
        if(equipment != null)
        {
            return true; 
        }

        //Is it a seed?
        //Try to cast it as a seed
        SeedData seed = item as SeedData;
        //If the seed is not null it is a seed 
        return seed != null; 

    }

    //Equip the hand slot with an ItemData (Will overwrite the slot)
    public void EquipHandSlot(ItemData item)
    {
        if (IsTool(item))
        {
            equippedToolSlot = new ItemSlotData(item); 
        } else
        {
            equippedItemSlot = new ItemSlotData(item); 
        }

    }

    public void EquipHandSlot(ItemSlotData itemSlot)
    {
        ItemData item = itemSlot.itemData;
        
        if (IsTool(item))
        {
            equippedToolSlot = new ItemSlotData(itemSlot);
        }
        else
        {
            equippedItemSlot = new ItemSlotData(itemSlot);
        }
    }

    public void ConsumeItem(ItemSlotData itemSlot)
    {
        if (itemSlot.IsEmpty())
        {
            Debug.LogError("There is nothing to consume!");
            return; 
        }

        //Use up one of the item slots
        itemSlot.Remove();
        //Refresh inventory
        RenderHand();
        UIManager.Instance.RenderInventory(); 
    }

    private void OnValidate()
    {
        //Validate the hand slots
        ValidateInventorySlot(equippedToolSlot);
        ValidateInventorySlot(equippedItemSlot);

        //Validate the slots in the inventory
        ValidateInventorySlots(itemSlots);
        ValidateInventorySlots(toolSlots);
    }
    
    //When giving the itemData value in the inspector, automatically set the quantity to 1 
    void ValidateInventorySlot(ItemSlotData slot)
    {
        if(slot.itemData != null && slot.quantity == 0)
        {
            slot.quantity = 1;
        }
    }

    //Validate arrays
    void ValidateInventorySlots(ItemSlotData[] array)
    {
        foreach (ItemSlotData slot in array)
        {
            ValidateInventorySlot(slot);
        }
    }

    void Start()
    {
          
    }
    void Update()
    {

    }

}
