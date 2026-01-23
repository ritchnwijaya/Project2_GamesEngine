using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ITimeTracker 
{
    public static UIManager Instance {get; private set;}
    [Header("Status Bar")]
    //Tool equip slot on the status bar
    public Image toolEquipSlot;
    public Text toolQuantityText; 

    // Time UI
    public Text timeText;
    public Text dateText;

    [Header("Inventory System")]
    public GameObject inventoryPanel;
    //The tool equip slot UI on the Inventory panel
    public HandInventorySlot itemHandSlot;
    public HandInventorySlot toolHandSlot;

    public InventorySlot[] toolSlots;
    public InventorySlot[] itemSlots;

    public Text itemNameText;
    public Text itemDescriptionText;

    [Header("Yes No Prompt")]
    public YesNoPrompt yesNoPrompt; 

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
        //add UIManager to the list of objects TimeManager will notify when time updates
        TimeManager.Instance.RegisterTracker(this);
    }

    public void TriggerPrompt(string message, System.Action onYesCallback)
    {
        yesNoPrompt.gameObject.SetActive(true);

        yesNoPrompt.CreatePrompt(message, onYesCallback); 
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
        ItemSlotData[] inventoryToolSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Tool);
        ItemSlotData[] inventoryItemSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Item);

        RenderInventoryPanel(inventoryToolSlots, toolSlots);
        RenderInventoryPanel(inventoryItemSlots, itemSlots);

        //Render the equipped slots
        toolHandSlot.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool));
        itemHandSlot.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item));

        //Get Tool Equip from InventoryManager
        ItemData equippedTool = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Tool);
        toolQuantityText.text = "";

        //Check if there is an item to display
        if (equippedTool != null)
        {
            //Switch the thumbnail over
            toolEquipSlot.sprite = equippedTool.thumbnail;

            toolEquipSlot.gameObject.SetActive(true);
            int quantity = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool).quantity;
            if (quantity > 1)
            {
                toolQuantityText.text = quantity.ToString();
            }

            return;
        }

        toolEquipSlot.gameObject.SetActive(false);
    }

    void RenderInventoryPanel(ItemSlotData[] slots, InventorySlot[] uiSlots)
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

    //callback to handle UI for time
    public void ClockUpdate(GameTimeStamp timestamp)
    {
        // handles the time 
        int hours = timestamp.hour;
        int minutes = timestamp.minute;
        
        string prefix = "AM ";

        // convert hours to 12 hour clock
        if(hours>12)
        {
            prefix = "PM ";
            hours -= 12; 
        }

        timeText.text = prefix + hours + ":" + minutes.ToString("00");

        // hanlde the date 
        int day = timestamp.day;
        string season = timestamp.season.ToString();
        string dayOfTheWeek = timestamp.GetDayOfTheWeek().ToString();

        dateText.text = season + " " + day + " (" + dayOfTheWeek + ")";


    }
    

}
