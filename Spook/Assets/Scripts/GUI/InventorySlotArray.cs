using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySlotArray : MonoBehaviour
{
    [Header("Script Imports")]
    public InventoryManagement InventoryManage;

    [Header("GameObjects")]
    public GameObject InventorySlot;
    public GameObject SelectedSlot;
    public GameObject ImageSlot;

    [Header("Lists")]
    private List<string> ISlots = new();
    private List<string> SSlots = new();
    private List<string> ImSlots = new ();

    public List<string> ItemSlots { get => ISlots; set => ISlots = value; }
    public List<string> SelectSlots { get => SSlots; set => SSlots = value; }
    public List<string> ImageSlots { get => ImSlots; set => ImSlots = value; }

    private int ItemSlotCount;
    private GameObject[] InventorySlotsArray;
    private const float Spacing = 35;

    private void Awake()
    {
        ItemSlotCount = InventoryManage.ItemSlotCount;
        InventorySlotsArray = new GameObject[ItemSlotCount];

        // Create the first slot
        CreateSlot(SelectedSlot, "Item Slot Selected", SSlots);

        // Create the remaining slots
        for (int i = 1; i < ItemSlotCount -1; i++)
        {
            GameObject Slot = CreateSlot(SelectedSlot, "Item Slot Selected", SSlots, i+1);
            InventorySlotsArray[i] = Slot;
        }
    }

    private GameObject CreateSlot(GameObject slotPrefab, string baseName, List<string> slotList, int index = 1)
    {
        GameObject newSlot;

        if (index == 1)
        {
            // Special case for the first slot
            newSlot = Instantiate(slotPrefab, transform);
            newSlot.name = baseName + " " + (slotList.Count + 1);
        }
        else
        {
            // Duplicate the first slot
            newSlot = Instantiate(SelectedSlot, transform);
            newSlot.name = baseName + " " + index;
        }

        slotList.Add(newSlot.name);
        return newSlot;
    }
}
