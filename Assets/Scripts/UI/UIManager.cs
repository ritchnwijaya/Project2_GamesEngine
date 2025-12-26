using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {get; private set;}
    [Header("Status Bar")]
    //Tool equip slot on the status bar
    public Image toolEquipSlot;


    [Header("Inventory System")]
    public GameObject inventoryPanel;
    //The tool equip slot UI on the Inventory panel
    public HandInventorySlot itemHandSlot;
    public HandInventorySlot toolHandSlot;

    public InventorySlot[] toolSlots;
    public InventorySlot[] itemSlots;

    public Text itemNameText;
    public Text itemDescriptionText;

    private void Awake()
    {
        if(Instance!=null && Instance!= this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        RenderInventory();
        AssignSlotIndexes();
    }

    //Iterate through the slot UI elements and assign it its reference slot index
    public void AssignSlotIndexes()
    {
        for (int i = 0; i < toolSlots.Length; i++)
        {
            toolSlots[i].AssignIndex(i);
            itemSlots[i].AssignIndex(i);
        }
    }

    public void RenderInventory()
    {
        //get the inventory tool slots from inventory manager
        ItemData[] inventoryToolSlots = InventoryManager.Instance.tools;
        ItemData[] inventoryItemSlots = InventoryManager.Instance.items;
        RenderInventoryPanel(inventoryToolSlots, toolSlots);
        RenderInventoryPanel(inventoryItemSlots, itemSlots);

        toolHandSlot.Display(InventoryManager.Instance.equippedTool);
        itemHandSlot.Display(InventoryManager.Instance.equippedItem);
        

        ItemData equippedTool = InventoryManager.Instance.equippedTool;

        //Check if there is an item to display
        if (equippedTool != null)
        {
            //Switch the thumbnail over
            toolEquipSlot.sprite = equippedTool.thumbnail;

            toolEquipSlot.gameObject.SetActive(true);

            return;
        }

        toolEquipSlot.gameObject.SetActive(false);
    }

    void RenderInventoryPanel(ItemData[] slots, InventorySlot[] uiSlots)
    {
        for(int i = 0; i< uiSlots.Length; i++)
        {
            uiSlots[i].Display(slots[i]);
        }
    }

    public void ToggleInventoryPanel()
    {
        //if the panel is hidden, show it and vice versa.
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        RenderInventory();
    }

    public void DisplayItemInfo(ItemData data)
    {
        if (data == null)
        {
            itemNameText.text = "";
            itemDescriptionText.text = "";
            return;
            
        }
        itemNameText.text = data.name;
        itemDescriptionText.text = data.description;
    }


    

}
