using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_InteractionController : MonoBehaviour
{
    // -- Events -- \\
    public delegate void InteractionDelegate(GameObject GameObjectReference);
    public static event InteractionDelegate InteractionEvent;

    // -- Main Code -- \\

    [Header("Keybinds")]
    public KeyCode InteractKey = KeyCode.E;

    [Header("Interaction Settings")]
    public float PickUpDistance = 3;

    [Header("Imports")]
    [SerializeField] private Transform CameraLocation;
    [SerializeField] private LayerMask InteractableLayer;

    // -- Object Types -- \\
    public enum InteractableType
    {
        Collectible,       
        Consumable,
        Door,
        None
    }

    void Awake()
    {
        InteractableLayer = LayerMask.GetMask("Interactable");
    }

    void Update()
    {
        Debug.DrawRay(CameraLocation.position, CameraLocation.forward * PickUpDistance, Color.green);
        Interaction();
    }

    void Interaction()
    {
        if (Input.GetKeyDown(InteractKey))
        {
            if (Physics.Raycast(CameraLocation.position, CameraLocation.forward, out RaycastHit hit, PickUpDistance))
            {
                Debug.Log("Hit");
                InteractionEvent?.Invoke(hit.collider.gameObject);
            }
        }
    }

}
