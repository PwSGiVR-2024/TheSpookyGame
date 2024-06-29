using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Transactions;

public class Pause_Menu : MonoBehaviour
{
    [Header("Game Object Imports")]
    public GameObject PauseMenu;
    public GameObject GameOverlay;
    public GameObject OptionsMenu;

    [Header("Controls")]
    public KeyCode MenuKey = KeyCode.Escape; // Default to Escape key if not set in Inspector

    private bool IsPaused;
    [SerializeField] private Button MainMenu;
    [SerializeField] private Button Resume;
    [SerializeField] private Button Options;
    [SerializeField] MenuTransitionManager transitionManager;

    private void Awake()
    {
        GameOverlay.SetActive(true);
        PauseMenu.SetActive(false);
        IsPaused = false;
        MainMenu.onClick.AddListener(ReturnToMenu);
        Resume.onClick.AddListener(Continue);
        Options.onClick.AddListener(Settings);
    }

    void Update()
    {
        if (Input.GetKeyDown(MenuKey))
        {
            if (IsPaused)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Settings()
    {
        OptionsMenu.SetActive(true);
        PauseMenu.SetActive(false);
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
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

}
