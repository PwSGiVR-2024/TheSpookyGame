using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryScript : MonoBehaviour
{
    public float BatteryAmount;
    private void BatteryEnergyAmount()
    {
        BatteryAmount = Random.Range(35, 50);
        Debug.Log(BatteryAmount);
    }

    private void Awake()
    {
        BatteryEnergyAmount();
    }

}
