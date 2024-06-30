using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class OnEnterEnd : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
 
        if (other.CompareTag("Player"))
        {
            // Load the next scene
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("EndScreen"); 
        }
    }
}
