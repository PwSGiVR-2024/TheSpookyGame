using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class InventoryManagement : MonoBehaviour
{
    public event Action OnInventoryUpdate;
    public event Action OnSlotChange;

    [Header ("Inventory")]
    public List<string> Inventory = new();
    private List<string> OGSlot = new();
    private List<string> CurrentlyHeld = new();
    
    public List<string> OGSlotName { get => OGSlot; set => OGSlot = value; }
    public List<string> CurrentlyHolding { get => CurrentlyHeld; set => CurrentlyHeld = value; }

    [Header ("Script Imports")]
    private ObjectGrabbable GrabbedObject;
    private FlashlightController Flashlight;
    public PlayerPickDrop InteractDetect;
    public InventorySlotArray InventoryArray;

    [Header("Texts")]
    public TextMeshProUGUI InventoryContents;
    public TextMeshProUGUI InventoryState;

    [Header("Inventory Values")]
    public int ItemSlotCount = 4;


    private int SlotID = 0;
    private string ItemDisplayName;
    public int CurrentSlotID { get => SlotID; set => SlotID = value; }


    private void Awake()
    {
        SetUpInventory();       
    }

    private void Update()
    {
        SlotSwap();       
    }

    private void Start()
    {
        UpdateDisplayAndState();
        SwitchSlot(1);
        SwitchSlot(-1);
    }

    private void UpdateDisplayAndState()
    {
        UpdateDisplay();
        UpdateInventoryStateText();
        OnInventoryUpdate?.Invoke();
    }

    // Setting up the inventory, creating 4 slots for items
    private void SetUpInventory()
    {
        for (int i = 0; i < ItemSlotCount; i++)
        {
            Inventory.Add("Item Slot " + i);
            OGSlot.Add("Item Slot " + i);
            CurrentlyHeld.Add("Nothing");
        }
        UpdateInventoryStateText();
    }


    // Handle slot switching using the scroll wheel
    private void SlotSwap()
    {
        if (Input.mouseScrollDelta.y > 0) SwitchSlot(1);
        else if (Input.mouseScrollDelta.y < 0) SwitchSlot(-1);
    }

    // Update the displayed text after changing variable inside of the list
    public void UpdateDisplay()
    {
        if (InventoryContents != null)
        {
            InventoryContents.text = "Inventory:\n";

            foreach (var Item in Inventory)
            {
                ItemDisplayName = Item.ToString();
                InventoryContents.text += "- " + Item + "\n";
            }
        }
    }


    // Update the InventoryState text with the current slot and currently holding item
    private void UpdateInventoryStateText()
    {
        if (InventoryState != null) InventoryState.text = "Current Slot: \n" + OGSlot[SlotID] + "\n \n Currently holding: \n" + CurrentlyHeld[SlotID];
    }


    // Add an item to the list by replacing the previous, default text
    public void AddItemToList(string Item)
    {
        int EmptySlotID = Inventory.FindIndex(InventorySlot => InventorySlot.Contains("Item Slot"));

        if (EmptySlotID != -1)
        {
            Inventory[SlotID] = Item;
            CurrentlyHeld[SlotID] = Item; // Update the slot with currently held item
        }


        // Update the displayed text
        UpdateDisplayAndState();
    }


    // Remove an item from the inventory and replace it with the default text
    public void RemoveItemFromList(string Item)
    {
        int ItemID = Inventory.FindIndex(InventorySlot => InventorySlot.Contains(Item));

        if (ItemID != -1)
        {
            Inventory[SlotID] = OGSlot[SlotID];
            CurrentlyHeld[SlotID] = "Nothing"; // Update to "Nothing" if no items are being currently held in this slot
        }

        // Update the displayed text
        UpdateDisplayAndState();
    }

    // Switch slots based on the scroll input
    private void SwitchSlot(int direction)
    {
        // Calculate the new slot index, and if it is within bounds
        SlotID += direction;
        if (SlotID < 0) SlotID = Inventory.Count - 1;
        else if (SlotID >= Inventory.Count) SlotID = 0;
        OnSlotChange?.Invoke();
        // Update the displayed text
        UpdateState();
        UpdateDisplayAndState();
    }


    // Set item visible only if held, and its slot has been selected
    private void SetItemActive(string ItemName, bool IsActive, bool IsSelectedSlot)
    {
        GameObject itemObject = GameObject.Find(ItemName);
        if (itemObject != null)
        {
            if (itemObject.TryGetComponent<ItemVisibilityController>(out var visibilityController)) visibilityController.SetVisibility(IsActive && IsSelectedSlot);
            else Debug.LogError($"Item '{ItemName}' is missing the ItemVisibilityController script.");
        }
    }
    private void SetSlotActive(string SlotName, bool IsSelectedSlot)
    {
        GameObject SlotSelected = GameObject.Find(SlotName);
        if (SlotSelected != null)
        {
            if (SlotSelected.TryGetComponent<ItemVisibilityController>(out var visibilityController))
            {
                // Only set visibility if the slot is selected
                visibilityController.SetVisibility(IsSelectedSlot);
            }
            else
            {
                Debug.LogError($"Slot '{SlotName}' is missing the SlotVisibilityController script.");
            }
        }
    }

    // Update the state of inventory and currently held/hidden items
    private void UpdateState()
    {
        for (int i = 0; i < Inventory.Count; i++)
        {
            string SlotName = OGSlot[i];
            SetSlotActive(SlotName, IsSelectedSlot: i == SlotID);
            string ItemName = Inventory[i];
            SetItemActive(ItemName, IsActive: i == SlotID, IsSelectedSlot: i == SlotID);
        }

    }
}
