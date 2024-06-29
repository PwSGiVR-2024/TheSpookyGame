using System.Transactions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button NewGame;
    [SerializeField] Button ContinueGame;
    [SerializeField] Button GameSettings;

    [Header("Exit")]
    [SerializeField] Button ExitGame;
    [SerializeField] Button ConfirmExit;
    [SerializeField] Button DeclineExit;


    [Header("Pop-Ups")]
    [SerializeField] GameObject ExitPopUp;
    public GameObject Menu;
    public GameObject Settings;

    GameManager gameManager;
    AudioManager audioManager;
    [SerializeField] MenuTransitionManager transitionManager;
    private void Awake()
    {       
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;

        Settings.SetActive(false);
        ExitPopUp.SetActive(false);
        Menu.SetActive(true);
      
        NewGame.onClick.AddListener(StartNewGame);
        ContinueGame.onClick.AddListener(ContinueTheGame);
        GameSettings.onClick.AddListener(GoToSettings);
        ExitGame.onClick.AddListener(QuitGame);
    }

    private void Start()
    {
        audioManager.SetMasterVolume(1);
        audioManager.SetMusicVolume(0.75F);
        audioManager.SetSFXVolume(0.75F);
        gameManager.SetResolutionOnStart(72);
        gameManager.SetControlModeOnStart("WASD");
    }


    public void Debugger(string name)
    {
        Debug.Log(name + " button has been pressed");
    }
    public void StartNewGame()
    {
        Debugger("New Game");
        SceneManager.LoadScene("Main Level", LoadSceneMode.Single);
    }

    public void ContinueTheGame()
    {
        Debugger("Continue");
    }
    public void GoToSettings()
    {
        Settings.SetActive(true);
        Menu.SetActive(false);
        Debugger("Settings");
        
    }
    public void QuitGame()
    {
        Debugger("Quit Game");
        QuitNotification();
    }

    public void QuitNotification()
    {
        Debug.Log("Pop-Up Active");
        ExitPopUp.SetActive(true);

        ConfirmExit.onClick.AddListener(ConfirmQuit);
        DeclineExit.onClick.AddListener(DeclineQuit);

    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }
    public void DeclineQuit()
    {
        ExitPopUp.SetActive(false);
        ConfirmExit.onClick.RemoveListener(ConfirmQuit);
        DeclineExit.onClick.RemoveListener(DeclineQuit);
    }
}
