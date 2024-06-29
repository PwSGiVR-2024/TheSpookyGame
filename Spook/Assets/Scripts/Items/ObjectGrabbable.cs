using TMPro;
using UnityEditor;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{

    [Header("Script Imports")]
    public InventoryManagement InventoryManage;

    [Header("Reference Points")]
    public Transform RotationPoint;

    private Rigidbody Rigidbody;
    private Transform ObjectGrabpoint;
    private Transform ObjectDroppoint;
    private GameObject CollectibleItem;


    private bool Visible;
    private string Object;
    private static string HeldItem;
    public string ObjectName { get => Object; set => Object = value; }
    public bool IsVisible { get => Visible; set => Visible = value; }




    private void Awake()
    {
        Visible = true;
        Rigidbody = GetComponent<Rigidbody>();
        CollectibleItem = GameObject.FindWithTag("Pickable");
        GetComponent<Collider>().enabled = true;
    }
    private void LateUpdate()
    {
        MoveObjectToHeldPosition();
    }
    private void ObjectTransform()
    {
        string currentItem = InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID];
        Vector3 offset = Vector3.zero;

        switch (currentItem)
        {
            case "Flashlight":
                RotateObject(0, 90, 84);
                MoveObjectToFixedPosition(0,0,0);
                break;
            case "RedKey":
                RotateObject(0, -85, 90);
                MoveObjectToFixedPosition(0.05F, 0, -0.02F);
                break;
            case "BlackKey":
                RotateObject(0, -85, 90);
                MoveObjectToFixedPosition(0.05F, 0, -0.02F);
                break;
            case"BlueKey":
                RotateObject(0, -85, 90);
                MoveObjectToFixedPosition(0.05F, 0, -0.02F);
                break;
            case "GreenKey":
                RotateObject(0, -85, 90);
                MoveObjectToFixedPosition(0.05F, 0, -0.02F);
                break;

            case "Finish Key":
                RotateObject(0, -85, 90);
                MoveObjectToFixedPosition(0.05F, 0, -0.02F);
                break;
            default:
                RotateObject(0, -85, 0);
                MoveObjectToFixedPosition(offset.x, offset.y, offset.z);
                break;
        }
    }



    private void RotateObject(float x, float y, float z)
    {
        transform.rotation = Quaternion.identity;
        transform.rotation *= RotationPoint.rotation * Quaternion.Euler(x, y, z);
    }
    private void MoveObjectToFixedPosition(float x, float y, float z)
    {
        transform.position = RotationPoint.TransformPoint(new Vector3(x, y, z));
    }
    private void MoveObjectToHeldPosition()
    {
        // Holding item in a fixed position
        if (ObjectGrabpoint != null)
        {
            ObjectTransform();
            Rigidbody.MovePosition(ObjectGrabpoint.position);
        }
        // Releasing held item
        else if (ObjectDroppoint != null)
        {
            Rigidbody.MovePosition(ObjectDroppoint.position);
            ObjectDroppoint = null;
        }
    }


    public void SetObjectActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Grab(Transform ObjectGrabpoint)
    {
        this.ObjectGrabpoint = ObjectGrabpoint;
        Rigidbody.useGravity = false;
        GetComponent<Collider>().enabled = false;

        Object = gameObject.name;
        HeldItem = Object;

        // Parent the object to the grabbing point
        transform.parent = ObjectGrabpoint;
    }
    public void Drop(Transform ObjectDroppoint)
    {
        // Unparent the object
        transform.parent = null;

        this.ObjectDroppoint = ObjectDroppoint;
        ObjectGrabpoint = null;
        Rigidbody.useGravity = true;
        GetComponent<Collider>().enabled = true;

        // Reset the position and rotation of the dropped item to ObjectDroppoint
        transform.SetPositionAndRotation(ObjectDroppoint.position, ObjectDroppoint.rotation);
    }
}