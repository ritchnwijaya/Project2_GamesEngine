using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Item info box")]
    public GameObject itemInfoBox; 
    public TextMeshProUGUI itemNameText; 
    public TextMeshProUGUI itemDescriptionText;

    [Header("Screen Transitions")]
    public GameObject fadeIn;
    public GameObject fadeOut; 

    [Header("Yes No Prompt")]
    public YesNoPrompt yesNoPrompt; 

    [Header("Player Stats")]
    public Text moneyText;

    [Header("Shop")]
    public ShopListingManager shopListingManager; 

    [Header("Stamina")]
    public Sprite[] staminaUI;
    public Image StaminaUIImage;
    public int staminaCount;

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
        PlayerStats.RestoreStamina();
        RenderInventory();
        AssignSlotIndexes();
        RenderPlayerStats();
        DisplayItemInfo(null);
        //add UIManager to the list of objects TimeManager will notify when time updates
        TimeManager.Instance.RegisterTracker(this);
    }

    public void FadeOutScreen()
    {
        fadeOut.SetActive(true);
    }

    public void FadeInScreen()
    {
        fadeIn.SetActive(true); 
    }

    public void OnFadeInComplete()
    {
        fadeIn.SetActive(false); 
    }

    public void ResetFadeDefaults()
    {
        fadeOut.SetActive(false);
        fadeIn.SetActive(true);
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
            itemInfoBox.SetActive(false); 
            return;
            
        }
        itemInfoBox.SetActive(true); 
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

    public void RenderPlayerStats()
    {
        moneyText.text = PlayerStats.Money + PlayerStats.CURRENCY; 
        staminaCount = PlayerStats.Stamina;
        ChangeStaminaUI();
    }
    
    public void OpenShop(List<ItemData> shopItems)
    {
        shopListingManager.gameObject.SetActive(true);
        shopListingManager.RenderShop(shopItems); 
    }

    public void ChangeStaminaUI()
    {
        if (staminaCount <= 45) StaminaUIImage.sprite = staminaUI[3]; // exhausted
        else if (staminaCount <= 80) StaminaUIImage.sprite = staminaUI[2]; // tired
        else if (staminaCount <= 115) StaminaUIImage.sprite = staminaUI[1]; // active
        else if (staminaCount <= 150) StaminaUIImage.sprite = staminaUI[0]; // energised

    }
}
