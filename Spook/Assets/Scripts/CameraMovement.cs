using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform CamPosition;
    private float HeightOffset = 0.6F;

    [Header("Rotation Settings")]
    [SerializeField] private Transform Orientation;
    [SerializeField] private float XSens = 10;
    [SerializeField] private float YSens = 10;
    

    private float XRotation;
    private float YRotation;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        // Handle camera movement
        Vector3 newPosition = CamPosition.position;
        newPosition.y += HeightOffset;
        transform.position = newPosition;
    }

    private void Update()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        MouseInput();
        transform.rotation = Quaternion.Euler(XRotation, YRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, YRotation, 0);
    }

    private void MouseInput()
    {
        float Sensitivity = 50;
        float XAxis = Input.GetAxisRaw("Mouse X") * Time.deltaTime * XSens * Sensitivity;
        float YAxis = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * YSens * Sensitivity;

        YRotation += XAxis;
        XRotation -= YAxis;
        XRotation = Mathf.Clamp(XRotation, -90f, 90f);
    }
}
