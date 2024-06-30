using System.Collections.Generic;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button Return;
    [SerializeField] Button ScreenMode;
    [SerializeField] Button ConfirmSettings;
    [SerializeField] Button ResetSettings;
    [SerializeField] Button AntiAlias;

    [Header("Sliders")]
    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;
    public Slider MouseSensSlider;

    [Header("Input Fields")]
    [SerializeField] TMP_InputField MasterVolume;
    [SerializeField] TMP_InputField MusicVolume;
    [SerializeField] TMP_InputField SFXVolume;
    [SerializeField] TMP_InputField MouseSensitivity;

    [Header("CheckBoxes")]
    [SerializeField] Toggle VSync;
    [SerializeField] Toggle Bloom;
    [SerializeField] Toggle Particles;

    [Header ("Text Fields")]
    [SerializeField] TextMeshProUGUI MasterVolumeText;
    [SerializeField] TextMeshProUGUI MusicVolumeText;
    [SerializeField] TextMeshProUGUI SFXVolumeText;
    [SerializeField] TextMeshProUGUI MouseSensitivityText;
    [SerializeField] TextMeshProUGUI ScreenModeInfo;
    [SerializeField] TextMeshProUGUI AntiAliasInfo;

    [Header("Dropdowns")]
    [SerializeField] public TMP_Dropdown ScreenResolutionList;

    [Header("Pop-Up")]
    [SerializeField] GameObject UnsavedPopUp;
    [SerializeField] Button Yes;
    [SerializeField] Button No;

    [Header("Value Fields")]
    [HideInInspector] public float MouseSensValue;
    [HideInInspector] public float MasterVolumeValue;
    [HideInInspector] public float MusicVolumeValue;
    [HideInInspector] public float SFXVolumeValue;
    [HideInInspector] public int ResolutionIDValue;
    [HideInInspector] public int ScreenModeValue;

    [Header("Game Objects")]
    [SerializeField] GameObject Previous;
    [SerializeField] GameObject Settings;

    public string[] AntiAliasTypes = { "None", "x2", "x4", "x8" };
    [HideInInspector] public int CurrentAAID = 0;
    public string[] ScreenModes = { "Fullscreen", "Borderless", "Windowed" };
    [HideInInspector] public int CurrentScreenModeID = 0;


    MenuManager menuManager;
    GameManager gameManager;
    AudioManager audioManager;
    KeybindManager keybindManager;

    private void Awake()
    {
        audioManager = AudioManager.Instance;
        gameManager = GameManager.Instance;

        menuManager = GetComponent<MenuManager>();
        keybindManager = FindObjectOfType<KeybindManager>();


        AddListeners();
        SetDefaultValues();
        ResolutionValues();
    }

    private void AddListeners()
    {
        Return.onClick.AddListener(ReturnToPrevious);
        ConfirmSettings.onClick.AddListener(ConfirmCurrentSettings);
        ResetSettings.onClick.AddListener(RestoreDefaults);
        ScreenMode.onClick.AddListener(SetScreenMode);
        AntiAlias.onClick.AddListener(ToggleAA);
        Yes.onClick.AddListener(ReturnWithoutSave);
        No.onClick.AddListener(ReturnToOptions);

        // Master volume Settings
        MasterVolumeSlider.onValueChanged.AddListener(UpdateMasterText);
        MasterVolumeText.text = Mathf.Round(MasterVolumeSlider.value).ToString();
        MasterVolume.onEndEdit.AddListener(UpdateMasterValue);

        // Music volume Settings
        MusicVolumeSlider.onValueChanged.AddListener(UpdateMusicText);
        MusicVolumeText.text = Mathf.Round(MusicVolumeSlider.value).ToString();
        MusicVolume.onEndEdit.AddListener(UpdateMusicValue);

        // SFX volume Settings
        SFXVolumeSlider.onValueChanged.AddListener(UpdateSFXText);
        SFXVolumeText.text = Mathf.Round(SFXVolumeSlider.value).ToString();
        SFXVolume.onEndEdit.AddListener(UpdateSFXValue);

        // Mouse Sensitivity Settings
        MouseSensSlider.onValueChanged.AddListener(UpdateSensText);
        MouseSensitivityText.text = Mathf.Round(SFXVolumeSlider.value).ToString();
        MouseSensitivity.onEndEdit.AddListener(UpdateMouseSensitivity);

        // Checkbox Settings
        VSync.onValueChanged.AddListener(ToggleVSync);
        Bloom.onValueChanged.AddListener(SetBloom);
        Particles.onValueChanged.AddListener(SetParticles);

        ScreenResolutionList.onValueChanged.AddListener(OnResolutionChanged);


    }


    // Setting the default values on start of program
    private void SetValues(Slider Slider, TextMeshProUGUI Text, float value)
    {
        Slider.value = value;
        Text.text = Mathf.Round(value).ToString();
    }

    private void SetDefaultAudioValues()
    {
        SetValues(MasterVolumeSlider, MasterVolumeText, gameManager.DefaultOptions.MasterVolume);
        SetValues(MusicVolumeSlider, MusicVolumeText, gameManager.DefaultOptions.MusicVolume);
        SetValues(SFXVolumeSlider, SFXVolumeText, gameManager.DefaultOptions.SFXVolume);
    }

    private void SetDefaultMouseSensitivity()
    {   SetValues(MouseSensSlider, MouseSensitivityText, gameManager.DefaultOptions.MouseSensitivity);  }

    private void SetDefaultScreenMode()
    {
        ScreenModeInfo.text = gameManager.DefaultOptions.ScreenMode;
    }

    private void SetDefaultValues()
    {
        SetDefaultAudioValues();
        SetDefaultMouseSensitivity();
        SetDefaultScreenMode();

        // Setting the default values on sliders
        audioManager.SetMasterVolume(gameManager.DefaultOptions.MasterVolume / 100);
        audioManager.SetMusicVolume(gameManager.DefaultOptions.MusicVolume / 100);
        audioManager.SetSFXVolume(gameManager.DefaultOptions.SFXVolume / 100);
    }

    //Anti - Alias
    public void ToggleAA()
    {
        CurrentAAID = (CurrentAAID + 1) % AntiAliasTypes.Length;
        QualitySettings.antiAliasing = CurrentAAID switch
        {
            1 => 2,
            2 => 4,
            3 => 8,
            _ => 0,
        };
        AntiAliasInfo.text = AntiAliasTypes[CurrentAAID];


        gameManager.HasPendingChanges = true;
        Debug.Log(QualitySettings.antiAliasing);
    }

    // Vertical Sync
    public void ToggleVSync(bool OnOff)
    {
        QualitySettings.vSyncCount = OnOff ? 1 : 0;
        Debug.Log(OnOff);
        gameManager.HasPendingChanges = true;
    }

    public void SetBloom(bool NewValue)
    {
        gameManager.HasPendingChanges = true;
    }
    public void SetParticles(bool NewValue)
    {
        gameManager.HasPendingChanges = true;
    }



    //  Updating the text in InputField
    public void UpdateMasterText(float value)
    {
        MasterVolume.text = Mathf.Round(value).ToString();
        audioManager.SetMasterVolume(value / 100);
        gameManager.HasPendingChanges = true;
    }
    public void UpdateMusicText(float value)
    {
        MusicVolume.text = Mathf.Round(value).ToString();
        audioManager.SetMusicVolume(value / 100);
        gameManager.HasPendingChanges = true;
    }
    public void UpdateSFXText(float value)
    {
        SFXVolume.text = Mathf.Round(value).ToString();
        audioManager.SetSFXVolume(value / 100);
        gameManager.HasPendingChanges = true;
    }
    public void UpdateSensText(float value)
    {
        MouseSensitivity.text = Mathf.Round(value).ToString();
        gameManager.HasPendingChanges = true;
    }


    //  Updating the slider value based on either movement or the InputField input
    public void UpdateMasterValue(string inputText)
    {
        if (float.TryParse(inputText, out float volumeValue))
        {
            volumeValue = Mathf.Clamp(volumeValue, 0, 100);
            MasterVolumeSlider.value = volumeValue;
            MasterVolume.text = Mathf.Round(volumeValue).ToString();
            audioManager.SetMasterVolume(volumeValue / 100);
            gameManager.HasPendingChanges = true;
        }
    }
    public void UpdateMusicValue(string inputText)
    {
        if (float.TryParse(inputText, out float volumeValue))
        {
            volumeValue = Mathf.Clamp(volumeValue, 0, 100);
            MusicVolumeSlider.value = volumeValue;
            MusicVolume.text = Mathf.Round(volumeValue).ToString();
            audioManager.SetMusicVolume(volumeValue / 100);
            gameManager.HasPendingChanges = true;
        }
    }
    public void UpdateSFXValue(string inputText)
    {
        if (float.TryParse(inputText, out float volumeValue))
        {
            volumeValue = Mathf.Clamp(volumeValue, 0, 100);
            SFXVolumeSlider.value = volumeValue;
            SFXVolume.text = Mathf.Round(volumeValue).ToString();
            audioManager.SetSFXVolume(volumeValue / 100);
            gameManager.HasPendingChanges = true;
        }
    }
    public void UpdateMouseSensitivity(string inputText)
    {
        if (float.TryParse(inputText, out float SensitivityValue))
        {
            SensitivityValue = Mathf.Clamp(SensitivityValue, 10, 100);
            MouseSensSlider.value = SensitivityValue;
            MouseSensitivityText.text = Mathf.Round(SensitivityValue).ToString();
            gameManager.HasPendingChanges = true;
        }
    }

    // Get all the possible resolution values, and add them to a list, then input those into the dropdown
    public void ResolutionValues()
    {
        gameManager.Resolutions = Screen.resolutions;
        ScreenResolutionList.ClearOptions();
        List<string> Options = new List<string>();
        int CurrentResID = 72;

        for (int i = 0; i < gameManager.Resolutions.Length; i++)
        {
            int roundedRefreshRate = Mathf.CeilToInt(gameManager.Resolutions[i].refreshRate);

            string option = gameManager.Resolutions[i].width + "x" + gameManager.Resolutions[i].height + " | " + roundedRefreshRate + " Hz";
            Options.Add(option);           
        }

        ScreenResolutionList.AddOptions(Options);
        ScreenResolutionList.value = CurrentResID;
        ScreenResolutionList.RefreshShownValue();
    }
    public void OnResolutionChanged(int resolutionIndex)
    {
        Resolution SelectedRes = gameManager.Resolutions[resolutionIndex];
        Screen.SetResolution(width: SelectedRes.width, height: SelectedRes.height, fullscreen: Screen.fullScreen, preferredRefreshRate: SelectedRes.refreshRate);
    }


    //  Setting the screen mode to either Fullscreen, or windowed
    public void SetScreenMode()
    {
        CurrentScreenModeID = (CurrentScreenModeID + 1) % ScreenModes.Length;

        ScreenModeInfo.text = ScreenModes[CurrentScreenModeID];
       
        gameManager.HasPendingChanges = true;
    }

    //  Confirm, or reset the changes made in settings
    public void ConfirmCurrentSettings()
    {
        if (gameManager.HasPendingChanges)
        {
            int SelectedResID = ScreenResolutionList.value;
            OnResolutionChanged(SelectedResID);

            gameManager.CurrentOptions.MasterVolume = MasterVolumeSlider.value;
            gameManager.CurrentOptions.MusicVolume = MusicVolumeSlider.value;
            gameManager.CurrentOptions.SFXVolume = SFXVolumeSlider.value;
            gameManager.CurrentOptions.MouseSensitivity = MouseSensSlider.value;
            gameManager.CurrentOptions.ResolutionID = SelectedResID;
            gameManager.CurrentOptions.ScreenMode = ScreenModes[CurrentScreenModeID];

            gameManager.CurrentKeyBinds.MovementType = keybindManager.MovementModes[keybindManager.CurrentMovementModeID];
            gameManager.CurrentKeyBinds.SprintKey = keybindManager.Keys["Sprint"];
            gameManager.CurrentKeyBinds.SneakKey = keybindManager.Keys["Sneak"];
            gameManager.CurrentKeyBinds.InteractKey = keybindManager.Keys["Interaction"];
            gameManager.CurrentKeyBinds.UseKey = keybindManager.Keys["Use Item"];
            gameManager.CurrentKeyBinds.DropKey = keybindManager.Keys["Drop Item"];
            gameManager.CurrentKeyBinds.FlashlightKey = keybindManager.Keys["Use Flash"];

            MasterVolumeValue = gameManager.CurrentOptions.MasterVolume;
            MusicVolumeValue = gameManager.CurrentOptions.MusicVolume;
            SFXVolumeValue = gameManager.CurrentOptions.SFXVolume;
            MouseSensValue = gameManager.CurrentOptions.MouseSensitivity;
            ResolutionIDValue = gameManager.CurrentOptions.ResolutionID;
            ScreenModeValue = gameManager.CurrentOptions.ScreenMode[CurrentScreenModeID];

            gameManager.SetScreenMode(ScreenModes[CurrentScreenModeID]);
            gameManager.SetMovementMode(keybindManager.MovementModes[keybindManager.CurrentMovementModeID]);


            // Set HasPendingChanges flag to false
            gameManager.HasPendingChanges = false;
        }
        else if (!gameManager.HasPendingChanges) return;     
    }

    //  Returning to previous Menu, or the options, based on the Player's choice
    public void ReturnToPrevious()
    {
        if (gameManager.HasPendingChanges)
        {
            UnsavedPopUp.SetActive(true);
            Yes.onClick.AddListener(ReturnWithoutSave);
            No.onClick.AddListener(ReturnToOptions);
        }
        else if (!gameManager.HasPendingChanges)
        {
            Previous.SetActive(true);
            Settings.SetActive(false);
        } 

    }
    public void ReturnToOptions()
    {
        UnsavedPopUp.SetActive(false);

        Yes.onClick.RemoveListener(ReturnWithoutSave);
        No.onClick.RemoveListener(ReturnToOptions);
    }

    public void ReturnWithoutSave()
    {
        RestoreDefaults();
        UnsavedPopUp.SetActive(false);

        Yes.onClick.RemoveListener(ReturnWithoutSave);
        No.onClick.RemoveListener(ReturnToOptions);

        Previous.SetActive(true);
        Settings.SetActive(false);
    }
    public void RestoreDefaults()
    {
        // Resetting the sliders and other options
        MouseSensSlider.value = gameManager.DefaultOptions.MouseSensitivity;
        MasterVolumeSlider.value = gameManager.DefaultOptions.MasterVolume;
        MusicVolumeSlider.value = gameManager.DefaultOptions.MusicVolume;
        SFXVolumeSlider.value = gameManager.DefaultOptions.SFXVolume;
        ScreenResolutionList.value = gameManager.DefaultOptions.ResolutionID; //ID

        // Resetting the keybinds
        gameManager.CurrentKeyBinds.MovementType = gameManager.DefaultKeyBinds.MovementType;
        gameManager.CurrentKeyBinds.SprintKey = gameManager.DefaultKeyBinds.SprintKey;
        gameManager.CurrentKeyBinds.SneakKey = gameManager.DefaultKeyBinds.SneakKey;
        gameManager.CurrentKeyBinds.InteractKey = gameManager.DefaultKeyBinds.InteractKey;
        gameManager.CurrentKeyBinds.UseKey = gameManager.DefaultKeyBinds.UseKey;
        gameManager.CurrentKeyBinds.DropKey = gameManager.DefaultKeyBinds.DropKey;
        gameManager.CurrentKeyBinds.FlashlightKey = gameManager.DefaultKeyBinds.FlashlightKey;

        // Updating the text of keybinds to their default values        
        keybindManager.MovementModeInfo.text = gameManager.DefaultKeyBinds.MovementType.ToString();
        keybindManager.Sprint.text = gameManager.DefaultKeyBinds.SprintKey.ToString();
        keybindManager.Sneak.text = gameManager.DefaultKeyBinds.SneakKey.ToString();
        keybindManager.Interact.text = gameManager.DefaultKeyBinds.InteractKey.ToString();
        keybindManager.UseItem.text = gameManager.DefaultKeyBinds.UseKey.ToString();
        keybindManager.DropItem.text = gameManager.DefaultKeyBinds.DropKey.ToString();
        keybindManager.UseFlash.text = gameManager.DefaultKeyBinds.FlashlightKey.ToString();

        gameManager.HasPendingChanges = false;
    }
}
