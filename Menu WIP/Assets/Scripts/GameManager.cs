using UnityEngine;

[System.Serializable]
public class GameOptions
{
    public float MouseSensitivity;   
    public float MasterVolume;
    public float MusicVolume;
    public float SFXVolume;
    public int ResolutionID;
    public string ScreenMode;
}
[System.Serializable]
public class KeyBinds
{
    public string MovementType;
    public KeyCode SprintKey;
    public KeyCode SneakKey;
    public KeyCode InteractKey;
    public KeyCode UseKey;
    public KeyCode DropKey;
    public KeyCode FlashlightKey;
}


public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    OptionsManager options;
    KeybindManager keybinds;

    [HideInInspector] public Resolution[] Resolutions;
    public bool HasPendingChanges;

    public GameOptions CurrentOptions;
    public KeyBinds CurrentKeyBinds;


    [HideInInspector] public GameOptions OriginalOptions;    
    [HideInInspector] public GameOptions DefaultOptions;
   
    [HideInInspector] public KeyBinds OriginalKeyBinds;   
    [HideInInspector] public KeyBinds DefaultKeyBinds;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject GameManagerObject = new ("GameManager");
                instance = GameManagerObject.AddComponent<GameManager>();
                DontDestroyOnLoad(GameManagerObject);
            }
            return instance;
        }
    }
    private void InitializeDefaultValues()
    {
        DefaultKeyBinds = new KeyBinds
        {
            MovementType = "WASD",
            SprintKey = KeyCode.LeftShift,
            SneakKey = KeyCode.LeftControl,
            InteractKey = KeyCode.E,
            UseKey = KeyCode.F,
            DropKey = KeyCode.G,
            FlashlightKey = KeyCode.Mouse0
        };

        DefaultOptions = new GameOptions
        {
            MouseSensitivity = 50,
            MasterVolume = 100,
            MusicVolume = 75,
            SFXVolume = 75,
            ResolutionID = 72,
            ScreenMode = "Fullscreen"
        };
    }
    private void Awake()
    {
        InstanceCheck();
        InitializeDefaultValues();

        options = FindObjectOfType<OptionsManager>();
        keybinds = FindObjectOfType<KeybindManager>();

        Options();
        Keybinds();
    }
    public void Keybinds()
    {       
        OriginalKeyBinds = new KeyBinds
        {
            MovementType = "WASD",
            SprintKey = KeyCode.LeftShift,
            SneakKey = KeyCode.LeftControl,
            InteractKey = KeyCode.E,
            UseKey = KeyCode.F,
            DropKey = KeyCode.G,
            FlashlightKey = KeyCode.Mouse0
        };
        CurrentKeyBinds = OriginalKeyBinds;

        CurrentKeyBinds = new KeyBinds
        {
            MovementType = keybinds.MovementModes[keybinds.CurrentMovementModeID],
            SprintKey = keybinds.Keys["Sprint"],
            SneakKey = keybinds.Keys["Sneak"],
            InteractKey = keybinds.Keys["Interaction"],
            UseKey = keybinds.Keys["Use Item"],
            DropKey = keybinds.Keys["Drop Item"],
            FlashlightKey = keybinds.Keys["Use Flash"]
        };
    }
    public void Options()
    {       
        OriginalOptions = new GameOptions
        {
            MouseSensitivity = 50,
            MasterVolume = 100,
            MusicVolume = 75,
            SFXVolume = 75,
            ResolutionID = 72,
            ScreenMode = "Fullscreen"
        };

        CurrentOptions = OriginalOptions;

        CurrentOptions = new GameOptions
        {
            MouseSensitivity = options.MouseSensValue,
            MasterVolume = options.MasterVolumeValue,
            MusicVolume = options.MusicVolumeValue,
            SFXVolume = options.SFXVolumeValue,
            ResolutionID = options.ResolutionIDValue,
            ScreenMode = options.ScreenModes[options.CurrentScreenModeID],
        };
    }

    public void SetScreenMode(string mode)
    {
        if (!HasPendingChanges && mode != options.ScreenModes[options.CurrentScreenModeID])
        {
            HasPendingChanges = true;
            return;
        }
        else
        {
            switch (mode)
            {
                case "Fullscreen":
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case "Borderless":
                    Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                    break;
                case "Windowed":
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                default:
                    Debug.LogError("Unsupported screen mode: " + mode);
                    break;
            }
        }
    }
    public void SetMovementMode(string mode)
    {
        if (!HasPendingChanges && mode != keybinds.MovementModes[keybinds.CurrentMovementModeID])
        {
            HasPendingChanges = true;
            return;
        }
        else
        {
            switch (mode)
            {
                case "WASD":
                    Debug.Log("WASD");
                    break;
                case "Arrow Keys":
                    Debug.Log("Arrow Keys");
                    break;
                default:
                    Debug.LogError("Unavailable movement mode " + mode);
                    break;
            }
        }
    }

    public void SetResolutionOnStart(int ResID)
    {
        DefaultOptions.ResolutionID = ResID;
    }
    public void SetControlModeOnStart(string ModeName)
    {
        DefaultKeyBinds.MovementType = ModeName;
    }
    private void InstanceCheck()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

}
