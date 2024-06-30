using System.Collections.Generic;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class EndingScript : MonoBehaviour
{
    public Button MenuButton;
    public Button QuitButton;

    void Awake()
    {
        MenuButton.onClick.AddListener(ReturnMenu);
        QuitButton.onClick.AddListener(QuitGame);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ReturnMenu() { SceneManager.LoadScene("Main Menu"); }
    void QuitGame() { Application.Quit(); }
}
