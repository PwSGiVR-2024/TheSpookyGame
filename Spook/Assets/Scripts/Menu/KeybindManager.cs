using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindManager : MonoBehaviour
{
    public Dictionary<string, KeyCode> Keys = new();

    public TextMeshProUGUI Sprint, Sneak, Interact, UseItem, DropItem, UseFlash, MovementModeInfo;
    private GameObject CurrentKey;
    GameManager gameManager;
    [SerializeField] Button MovementMode;
    [SerializeField] GameObject PopUp, Warning;
    [Header("Text Fields")]

    [HideInInspector] public string[] MovementModes = { "WASD", "Arrow Keys" };
    [HideInInspector] public int CurrentMovementModeID = 0;

    void Awake()
    {
        gameManager = GameManager.Instance;
        PopUp.SetActive(false);
        Warning.SetActive(false);

        Keys.Clear();

        AddKeys();

        Sprint.text = Keys["Sprint"].ToString();
        Sneak.text = Keys["Sneak"].ToString();
        Interact.text = Keys["Interaction"].ToString();
        UseItem.text = Keys["Use Item"].ToString();
        DropItem.text = Keys["Drop Item"].ToString();
        UseFlash.text = Keys["Use Flash"].ToString();

        MovementMode.onClick.AddListener(SetMovementMode);
    }

    public void AddKeys()
    {
        Keys.Add("Sneak", gameManager.DefaultKeyBinds.SneakKey);
        Keys.Add("Sprint", gameManager.DefaultKeyBinds.SprintKey);
        Keys.Add("Interaction", gameManager.DefaultKeyBinds.InteractKey);
        Keys.Add("Use Item", gameManager.DefaultKeyBinds.UseKey);
        Keys.Add("Drop Item", gameManager.DefaultKeyBinds.DropKey);
        Keys.Add("Use Flash", gameManager.DefaultKeyBinds.FlashlightKey);
    }

    void SetMovementMode()
    {
        CurrentMovementModeID = (CurrentMovementModeID + 1) % MovementModes.Length;
        MovementModeInfo.text = MovementModes[CurrentMovementModeID];

        gameManager.HasPendingChanges = true;
    }

    public void ChangeKey(GameObject Clicked)
    {
        Warning.SetActive(false);

        CurrentKey = Clicked;
        if (CurrentKey.name != "Movement Key")
        {
            // Check if the key is already assigned
            string FunctionName = CurrentKey.name;
            KeyCode CurrentKeyCode = Keys[FunctionName];
            PopUp.SetActive(true);

            // Set up the pop-up text
            TMP_Text popUpText = PopUp.GetComponentInChildren<TMP_Text>();
            popUpText.text = $"Alert: Keybind Modification for '{FunctionName}'\n   Currently Assigned Key: {CurrentKeyCode}";

        }
        else
        {
            PopUp.SetActive(false); // Movement Key doesn't need reassignment
            CurrentKey = null;
        }
    }


    void UpdateKey(string FunctionName, KeyCode NewKeyCode)
    {
        if (Keys.ContainsKey(FunctionName))
        {
            string conflictingFunction = Keys.FirstOrDefault(x => x.Value == NewKeyCode && x.Key != FunctionName).Key;

            if (!string.IsNullOrEmpty(conflictingFunction))
            {
                ShowWarningMessage();
                return;
            }

            PopUp.SetActive(false);
            Keys[FunctionName] = NewKeyCode;

            switch (FunctionName)
            {
                case "Sprint":
                    Sprint.text = NewKeyCode.ToString();
                    break;
                case "Sneak":
                    Sneak.text = NewKeyCode.ToString();
                    break;
                case "Interaction":
                    Interact.text = NewKeyCode.ToString();
                    break;
                case "Use Item":
                    UseItem.text = NewKeyCode.ToString();
                    break;
                case "Drop Item":
                    DropItem.text = NewKeyCode.ToString();
                    break;
                case "Use Flash":
                    UseFlash.text = NewKeyCode.ToString();
                    break;
            }
            GameManager.Instance.HasPendingChanges = true;
        }
        else return;
    }


    void ShowWarningMessage()
    {
        Warning.SetActive(true);
        StartCoroutine(HideWarningMessage());
    }

    IEnumerator HideWarningMessage()
    {
        yield return new WaitForSeconds(1.5F);
        Warning.SetActive(false);
    }

    void OnGUI()
    {
        if (CurrentKey != null && CurrentKey.name != "Movement Key")
        {
            Event CurrEvent = Event.current;
            if (CurrEvent.isKey)
            {
                string FunctionName = CurrentKey.name;
                KeyCode KeyPressed = CurrEvent.keyCode;

                // Check if the pressed key is already assigned to any function
                bool AlreadyAssigned = Keys.ContainsValue(KeyPressed);
                if (!AlreadyAssigned)
                {
                    UpdateKey(FunctionName, KeyPressed);
                    CurrentKey = null;
                    PopUp.SetActive(false); // Hide pop-up as an unused key is pressed
                }
                
                else
                {
                    Debug.LogWarning($"Key '{KeyPressed}' is already assigned to a function.");
                    ShowWarningMessage();
                }
            }
            else if (CurrEvent.isMouse && CurrEvent.button >= 0 && CurrEvent.button < 3)
            {
                string FunctionName = CurrentKey.name;
                KeyCode NewKeyCode = KeyCode.Mouse0 + CurrEvent.button; // Mouse0 is the left mouse button, Mouse1 is right, Mouse2 is middle

                // Check if the new key is already assigned to any function
                bool AlreadyAssigned = Keys.ContainsValue(NewKeyCode);
                if (!AlreadyAssigned)
                {
                    UpdateKey(FunctionName, NewKeyCode);
                    CurrentKey = null;
                    PopUp.SetActive(false); // Hide pop-up as an unused key is pressed
                }
                else
                {
                    Debug.LogWarning($"Key '{NewKeyCode}' is already assigned to a function.");
                    ShowWarningMessage();
                }
            }
        }
    }

}
