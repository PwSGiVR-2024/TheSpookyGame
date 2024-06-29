using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickDrop : MonoBehaviour
{
    public delegate void InteractionDelegate(GameObject gameObjectReference);
    public static event InteractionDelegate InteractionEvent;

    [Header("Transforms and Mask")]
    [SerializeField] private Transform CameraLocation;
    [SerializeField] private Transform ObjectGrabpoint;
    [SerializeField] private Transform ObjectDroppoint;
    [SerializeField] private LayerMask InteractLayer;

    [Header("Script Imports")]
    public InventoryManagement InventoryManage;
    private ObjectGrabbable GrabbedObject;
    public Player_MovementController PMovement;

    [Header("Controls")]
    public KeyCode InteractKey = KeyCode.E;
    public KeyCode DropKey = KeyCode.G;
    public KeyCode UseBattery = KeyCode.F;

    [Header("Variables etc")]
    public float PickUpDistance;
    private bool Interactable, Pickable, Door, Held, SlotUsed;
    private string DoorName;

    public bool IsInteractable { get => Interactable; set => Interactable = value; }
    public bool IsPickable { get => Pickable; set => Pickable = value; }
    public bool IsDoor { get => Door; set => Door = value; }
    public bool IsHeld { get => Held; set => Held = value; }
    public bool SlotInUse { get => SlotUsed; set => SlotUsed = value; }

    [Header("List")]
    private List<ObjectGrabbable> Grabbed = new();
    public List<ObjectGrabbable> GrabbedObjects { get => Grabbed; set => Grabbed = value; }

    [Header("Sounds")]
    [SerializeField] private AudioSource PickUpDefault;

    public enum InteractableObject
    {
        Pickable,
        Door,
        None
    }

    public InteractableObject CurrentObjectType;

    private void Awake()
    {
        SlotUsed = false;
        Held = false;
    }

    private void Update()
    {
        Debug.DrawRay(CameraLocation.position, CameraLocation.forward * PickUpDistance, Color.green);
        LookDetection();
        Interaction();
    }

    void Interaction()
    {
        if (Input.GetKeyDown(InteractKey))
        {
            if (Physics.Raycast(CameraLocation.position, CameraLocation.forward, out RaycastHit hit, PickUpDistance, InteractLayer))
            {
                InteractionEvent?.Invoke(hit.collider.gameObject);
                TryPickUpObject(hit);
            }
        }
        else if (Input.GetKeyDown(DropKey) && Grabbed.Count > 0 && InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] != "Nothing")
        {
            TryDropObject();
        }
    }

    void TryPickUpObject(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent(out ObjectGrabbable grabbedObject))
        {
            // Check if the slot is already occupied
            if (InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] != "Nothing")
            {
                SlotUsed = true;
                return;
            }
            else SlotUsed = false;

            PickUpObject(grabbedObject);
        }
    }

    void PickUpObject(ObjectGrabbable grabbedObject)
    {
        PickUpDefault.Play();
        grabbedObject.Grab(ObjectGrabpoint);
        Grabbed.Add(grabbedObject);
        InventoryManage.AddItemToList(grabbedObject.ObjectName);
    }

    void TryDropObject()
    {
        string itemName = InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID];
        ObjectGrabbable grabbedObject = Grabbed.Find(obj => obj.ObjectName == itemName);

        if (grabbedObject != null)
        {
            DropObject(grabbedObject);
        }
    }

    void DropObject(ObjectGrabbable grabbedObject)
    {
        grabbedObject.Drop(ObjectDroppoint);
        InventoryManage.RemoveItemFromList(grabbedObject.ObjectName);
        SetItemVisibility(grabbedObject.ObjectName, isVisible: true);
        Grabbed.Remove(grabbedObject);
    }

    void SetItemVisibility(string itemName, bool isVisible)
    {
        GameObject itemObject = GameObject.Find(itemName);

        if (itemObject != null)
        {
            if (itemObject.TryGetComponent<ItemVisibilityController>(out var visibilityController)) visibilityController.SetVisibility(isVisible);
            else Debug.LogError($"Item '{itemName}' is missing the ItemVisibilityController script.");
        }
    }

    void LookDetection()
    {
        if (Physics.Raycast(CameraLocation.position, CameraLocation.forward, out RaycastHit hit, PickUpDistance, InteractLayer))
        {
            Interactable = true;

            if (hit.transform.CompareTag("Pickable"))
            {
                Pickable = true;
                Door = false;
                CurrentObjectType = InteractableObject.Pickable;
            }
            else if (hit.transform.CompareTag("Door"))
            {
                Pickable = false;
                Door = true;
                CurrentObjectType = InteractableObject.Door;
                DoorName = hit.transform.name;
            }
            else
            {
                Pickable = false;
                Door = false;
                CurrentObjectType = InteractableObject.None;
            }
        }
        else
        {
            Interactable = false;
            Pickable = false;
            Door = false;
            CurrentObjectType = InteractableObject.None;
        }
    }
}
