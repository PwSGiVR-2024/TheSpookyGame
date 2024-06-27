using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    public Transform CamPosition;
    public float heightOffset;

    private void FixedUpdate()
    {
        Vector3 newPosition = CamPosition.position;
        newPosition.y += heightOffset;
        transform.position = newPosition;
    }
}
