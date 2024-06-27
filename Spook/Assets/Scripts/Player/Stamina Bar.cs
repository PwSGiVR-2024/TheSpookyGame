using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [Header("Script Imports")]
    [SerializeField] private Stamina_System SSystem;

    [Header ("Bar values")]
    public int Maximum;
    private int CurrentValue;
    public int Current { get => CurrentValue; set => CurrentValue = value; }

    public Image mask;
    

    private void Update()
    {
        GetCurrentFill();
    }
    void GetCurrentFill()
    {
        float fillAmount = SSystem.Stamina / SSystem.MaxStamina;
        mask.fillAmount = fillAmount;
    }
}
