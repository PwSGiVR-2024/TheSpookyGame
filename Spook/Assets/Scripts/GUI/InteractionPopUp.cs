using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionPopUp : MonoBehaviour
{
    [Header("Script Imports")]
    public GameObject Interactable;
    public GameObject Useable;
    public PlayerPickDrop InteractDetect;
    public InventoryManagement InventoryManage;
    public FlashlightController Flashlight;

    public List<DoorController> Doors = new List<DoorController>();

    [Header("Text Object references")]
    [SerializeField] private TextMeshProUGUI Key;
    [SerializeField] private TextMeshProUGUI UseableKey;
    [SerializeField] private TextMeshProUGUI Info;
    [SerializeField] private TextMeshProUGUI UseInfo;
    [SerializeField] private TextMeshProUGUI WarningText;


    private void Awake()
    {
        WarningText.text = null;
    }

    private void Update()
    {
        InteractionDetection();
        Useage();
        OnInteraction();
        FlashlightBar();
    }

    private void InteractionDetection()
    {        
        if (InteractDetect.IsInteractable)
        {
            foreach (DoorController door in Doors)
            {
                if (InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] == "Key" && InteractDetect.IsDoor && door.IsLocked) Info.text = "to Unlock";
                else if (InteractDetect.IsDoor && !door.IsLocked && !door.IsOpen) Info.text = "to Open";
                else if (InteractDetect.IsDoor && !door.IsLocked && door.IsOpen) Info.text = "to Close";
            }
            Interactable.SetActive(true);
            Key.text = InteractDetect.InteractKey.ToString();
            Info.text = "to Interact";

            if (InteractDetect.IsPickable && InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] == "Nothing") Info.text = "to Pick up";

        }
        else if (InteractDetect.GrabbedObjects.Count > 0 && InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] != "Nothing")
        {
            Interactable.SetActive(true);
            Key.text = InteractDetect.DropKey.ToString();
            Info.text = "to Drop";
        }
        else Interactable.SetActive(false);
        
    }


    private void OnInteraction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (InteractDetect.GrabbedObjects.Count > 0 && InteractDetect.SlotInUse && Input.GetKeyDown(InteractDetect.InteractKey) && InteractDetect.IsDoor == false)
        {
            WarningText.text = "This slot is already in use!";
            StartCoroutine(ClearText(1));
        }

        else if (Physics.Raycast(ray, out hit))
        {
            DoorController hitDoor = hit.collider.GetComponent<DoorController>();
            Debug.Log(hitDoor);
            if (hitDoor != null && hitDoor.IsLocked && InteractDetect.PickUpDistance == 2)
            {
                WarningText.text = "Locked!";
                StartCoroutine(ClearText(1.5F));
            }

        }
        else
        {
            WarningText.text = " ";
        }
    }
    private void Useage()
    {
        if (InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] == "Flashlight" && Flashlight.Charge < Flashlight.MaxCharge && InventoryManage.Inventory.Contains("Battery"))
        {
            Useable.SetActive(true);
            int batteryIndex = 0;
            for (int i = 0; i < InventoryManage.Inventory.Count; i++)
            {
                if (InventoryManage.Inventory[i].StartsWith("Battery"))
                {
                    batteryIndex = i;
                    break;
                    
                }
            }
            if (batteryIndex != -1)
            {
                UseableKey.text = InteractDetect.UseBattery.ToString();
                UseInfo.text = "to Refill";
            }
            else
            {
                UseableKey.text = "";
                UseInfo.text = "";
            }
        }
        else Useable.SetActive(false);
    }
    private void FlashlightBar()
    {
        if (Flashlight.FlashlightInHand) Flashlight.ChargeBar.SetActive(true);
        else Flashlight.ChargeBar.SetActive(false);
    }
    private IEnumerator ClearText(float delay)
    {
        yield return new WaitForSeconds(delay);
        WarningText.text = " ";
    }
}
