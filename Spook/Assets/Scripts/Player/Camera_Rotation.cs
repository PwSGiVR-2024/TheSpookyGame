using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Rotation : MonoBehaviour
{
    [Header("Mouse Settings")]
    private readonly float XSens = 10;
    private readonly float YSens = 10;

    [Header("Script Imports")]
    public Transform Orientation;
    public Transform RayPoint;
    public Transform GrabPoint;
    public PlayerPickDrop InteractDetect;
    

    private int MaxRayDistance = 3;
    private float XRotation;
    private float YRotation;

    private void Awake()
    {;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        CameraRotation();
        CastFromCamera();
        CastFromHand();
    }

    private void CameraRotation()
    {
        MouseInput();
        transform.rotation = Quaternion.Euler(XRotation, YRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, YRotation, 0);
    }
    private void MouseInput()
    {
        float currentSensitivity = 50;//FindObjectOfType<ButtonFunctionManager>().CurrentMouseSensitivity
        //Camera Rotation - Get Mouse X,Y input 
        float XAxis = Input.GetAxisRaw("Mouse X") * Time.deltaTime * XSens * currentSensitivity;
        float YAxis = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * YSens * currentSensitivity;

        YRotation += XAxis;

        XRotation -= YAxis;
        XRotation = Mathf.Clamp(XRotation, -90F, 90F);
    }

    private void CastFromCamera()
    {
        Ray CameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(CameraRay, out hit, MaxRayDistance))
        {
            return;
        }
    }

    private void CastFromHand()
    {
        Ray HandRay = new Ray(GrabPoint.position, RayPoint.position - GrabPoint.position);
        RaycastHit HandHit;

        if (Physics.Raycast(HandRay, out HandHit, Mathf.Infinity))
        {
            Vector3 directionToRayPoint = (RayPoint.position - GrabPoint.position).normalized;
            GrabPoint.forward = directionToRayPoint;
        }
    }
}