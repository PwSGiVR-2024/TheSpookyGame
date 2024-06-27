using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Charge_Bar : MonoBehaviour
{   
    [Header("Script Imports")]
    [SerializeField] private FlashlightController Flashlight;

    [Header("Bar values")]
    public int Maximum;
    private int CurrentValue;
    public int Current { get => CurrentValue; set => CurrentValue = value; }
    public Image FMask;

    
    private void Update()
    {
        GetCurrentFill();
    }
    void GetCurrentFill()
    {
        float fillAmount = Flashlight.Charge / Flashlight.MaxCharge;
        FMask.fillAmount = fillAmount;
    }
}
