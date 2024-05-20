using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnterCheck : MonoBehaviour
{    
    [HideInInspector] public bool isPlayerNextToDoor = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNextToDoor = true;
            Debug.Log("Player is next to the door");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the door");
        }
    }
}
