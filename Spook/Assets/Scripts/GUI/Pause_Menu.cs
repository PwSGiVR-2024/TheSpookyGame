using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Pause_Menu : MonoBehaviour
{
    [Header ("Game Object Imports")]
    public GameObject PauseMenu;
    public GameObject GameOverlay;

    [Header("Controls")]
    public KeyCode MenuKey = KeyCode.L;

    private bool IsPaused;
    [SerializeField] private Button MainMenu;
    [SerializeField] private Button Resume;

    private void Awake()
    {
        
        GameOverlay.SetActive(true);
        PauseMenu.SetActive(false);
        IsPaused = false;
        MainMenu.onClick.AddListener(ReturnToMenu);
        Resume.onClick.AddListener(Continue);

    }
    void Update()
    {
        MenuSwap();
    }

    // Swapping from Game Screen to Pause Menu
    void MenuSwap ()
    {
        if (Input.GetKeyDown(MenuKey)  && !IsPaused)
        {
            Pause();
        }
        else if (Input.GetKeyDown(MenuKey) && IsPaused)
        {
            Continue();
        }
    }

    // Pause Function
    public void Pause()
    {
        IsPaused = true;
        GameOverlay.SetActive(false);
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    // Unpause Function
    public void Continue()
    {
        IsPaused = false;
        GameOverlay.SetActive(true);
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void ReturnToMenu()
    {
        Debug.Log("Returning to Main Menu");
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        Time.timeScale = 1;
    }
}
