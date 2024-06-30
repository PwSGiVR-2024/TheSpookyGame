using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashlightController : MonoBehaviour
{
    GameManager gameManager;
    [Header("Script Imports")]
    public InventoryManagement InventoryManage;
    public PlayerPickDrop InteractDetect;
    public BatteryScript Battery;
    public GameObject LightRays;

    [Header("Flashlight Settings")]
    [SerializeField] private Transform GrabPoint;
    [SerializeField] private Light Lamp;
    public GameObject Flashlight, ChargeBar;

    public float MaxCharge, UsageRate;

    private float CurrentCharge;
    private bool FlashlightHeld;
    private bool IsFlashOn, IsCharged;

    [Header("Animations")]
    [SerializeField] private Animator OutOfBattery;

    [Header("Sounds")]
    [SerializeField] private AudioSource FlashClick;
    [SerializeField] private AudioSource FlashBuzz;

    [Header("Fill Object")]
    public Image fillObject;
    public Gradient chargeGradient;

    public float Charge { get => CurrentCharge; set => CurrentCharge = value; }
    public bool FlashlightInHand { get => FlashlightHeld; set => FlashlightHeld = value; }
    public bool IsFlashlightOn { get => IsFlashOn; set => IsFlashOn = value; }

    public bool RayVisibile
    {
        get { return LightRays.activeSelf; }
    }

    private void Awake()
    {
        gameManager = GameManager.Instance;

        // Ensure all required components are assigned or log an error if not
        if (LightRays == null)
        {
            Debug.LogError("LightRays is not assigned in the inspector.");
        }

        if (Lamp == null)
        {
            Debug.LogError("Lamp is not assigned in the inspector.");
        }

        if (InventoryManage == null)
        {
            InventoryManage = FindObjectOfType<InventoryManagement>();
            if (InventoryManage == null)
            {
                Debug.LogError("InventoryManage is not assigned and could not be found in the scene.");
            }
        }

        if (InteractDetect == null)
        {
            InteractDetect = FindObjectOfType<PlayerPickDrop>();
            if (InteractDetect == null)
            {
                Debug.LogError("InteractDetect is not assigned and could not be found in the scene.");
            }
        }

        if (Battery == null)
        {
            Battery = FindObjectOfType<BatteryScript>();
            if (Battery == null)
            {
                Debug.LogError("Battery is not assigned and could not be found in the scene.");
            }
        }

        LightRays?.SetActive(false);
        Charge = MaxCharge;
        Lamp.intensity = 0;
        IsFlashlightOn = false;
    }

    void Update()
    {
        FlashlightFunction();
        LightOnOff();
        ChargeFunctions();
        BatteryUse();
        AnimationOnBatteryOutage();
    }

    private void ChargeFunctions()
    {
        State();
    }

    private void BatteryUse()
    {
        if (Charge < MaxCharge && FlashlightInHand && Input.GetKeyDown(InteractDetect?.UseBattery ?? KeyCode.None))
        {
            // Find the index of the battery in the inventory
            int batteryIndex = -1;
            for (int i = 0; i < InventoryManage?.Inventory.Count; i++)
            {
                if (InventoryManage.Inventory[i].StartsWith("Battery"))
                {
                    batteryIndex = i;
                    break;
                }
            }

            if (batteryIndex != -1)
            {
                // Use the battery at the found index
                int originalSlotID = batteryIndex;

                InventoryManage.Inventory[batteryIndex] = "Nothing";
                InventoryManage.Inventory[originalSlotID] = InventoryManage.OGSlotName[originalSlotID];
                InventoryManage.CurrentlyHolding[originalSlotID] = "Nothing";

                float addedCharge = Battery.BatteryAmount;
                Charge = Mathf.Min(Charge + addedCharge, MaxCharge);

                // Stop the draining coroutine if it's running
                StopCoroutine(DrainFlashlightCoroutine());

                // If flashlight is on, restart draining coroutine
                if (IsFlashlightOn)
                {
                    IsFlashlightOn = false;  // Ensure it's off before restarting
                    StartDrainingFlashlight();
                }

                InventoryManage.UpdateDisplay();
            }
        }
    }

    private IEnumerator DrainFlashlightCoroutine()
    {
        while (IsFlashlightOn && Charge > 0)
        {
            Charge -= UsageRate * Time.deltaTime;
            Charge = Mathf.Max(Charge, 0);
            yield return null;
        }

        IsFlashlightOn = false;
    }

    private void StartDrainingFlashlight()
    {
        StartCoroutine(DrainFlashlightCoroutine());
    }

    private void State()
    {
        if (Charge == 0)
        {
            IsFlashlightOn = false;
            IsCharged = false;
        }
        else if (Charge > 0) IsCharged = true;
    }

    private void LightOnOff()
    {
        float chargePercentage = Charge / MaxCharge;

        if (IsFlashlightOn)
        {
            Lamp.intensity = Mathf.Lerp(0.8f, 1.0f, chargePercentage);
            LightRays?.SetActive(true);
        }
        else
        {
            FlashBuzz?.Stop();
            Lamp.intensity = 0;
        }

        Color fillColor = chargeGradient.Evaluate(chargePercentage);
        UpdateFillColor(fillColor);
    }

    private void UpdateFillColor(Color color)
    {
        if (fillObject != null)
        {
            fillObject.color = color;
        }
    }

    private void FlashlightStates()
    {
        if (InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] != "Flashlight" && InventoryManage.Inventory.Contains("Flashlight")) IsFlashlightOn = false;
        if (InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] == "Flashlight") FlashlightInHand = true;
        else FlashlightInHand = false;
    }

    private IEnumerator FlickerCoroutine(float flickerIntensity)
    {
        while (IsFlashlightOn && Charge <= 15)
        {
            float RandomValue = Random.Range(0.1f * flickerIntensity, flickerIntensity);
            Lamp.intensity = RandomValue;
            FlashBuzz.volume = (1.5F * RandomValue) * gameManager.CurrentOptions.SFXVolume;
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        }

        Lamp.intensity = IsFlashlightOn ? 1 : 0;
    }

    private void FlashlightFunction()
    {
        FlashlightStates();
        if (InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] == "Flashlight")
        {
            if (Input.GetMouseButtonDown(0) && !IsFlashlightOn && IsCharged)
            {
                FlashClick.Play();
                FlashBuzz.Play();
                FlashClick.volume = 1 * gameManager.CurrentOptions.SFXVolume;
                FlashBuzz.volume = 1.5F * gameManager.CurrentOptions.SFXVolume;
                IsFlashlightOn = true;
                StartDrainingFlashlight();
            }
            else if (Input.GetMouseButtonDown(0) && IsFlashlightOn)
            {
                FlashClick.Play();
                FlashBuzz.Stop();
                IsFlashlightOn = false;
            }
        }
        if (IsFlashlightOn && Charge <= 15)
        {
            StartCoroutine(FlickerCoroutine(0.6F));
        }
        else if (Charge == 0 || Charge > 15)
        {
            StopCoroutine(FlickerCoroutine(0.5f));
            Lamp.intensity = IsFlashlightOn ? 1 : 0;
        }
        else
        {
            StopCoroutine(DrainFlashlightCoroutine());
            return;
        }
    }

    private void AnimationOnBatteryOutage()
    {
        if (Charge == 0 && FlashlightInHand && Input.GetMouseButton(0)) OutOfBattery.Play("Out Of Battery");
        else OutOfBattery.StopPlayback();
    }
}
