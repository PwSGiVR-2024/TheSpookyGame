using System.Collections;
using UnityEngine;
using static Player_MovementController;

public class Stamina_System : MonoBehaviour
{
    [Header("| Stamina Settings |")]
    private float CurrentStamina;
    [SerializeField] private float MaximumStamina = 100;
    [SerializeField] private float UsageRate = 10;
    [SerializeField] private float RegenerationRate = 5;

    private const float OutageSpeedReduction = 2;
    private const float StandingTimeRequired = 2;

    public float Stamina { get => CurrentStamina; set => CurrentStamina = value; }
    public float MaxStamina { get => MaximumStamina; set => MaximumStamina = value; }

    private Player_MovementController PMovement;
    private bool OutOfStamina, CanRegenerateStamina, IsRegenerationBoostActive, IsStaminaCooldownActive, IsStandingLongEnough;
    public StaminaStates CurrentState;
    public enum StaminaStates 
    {
        Normal,
        Exhausted,
        RegenBoost       
    }

    private void UpdateStates()
    {
        if (Stamina == 0 && CurrentState != StaminaStates.Exhausted)
        {
            CurrentState = StaminaStates.Exhausted;
        }
        else if (IsRegenerationBoostActive && CurrentState != StaminaStates.RegenBoost)
        {
            CurrentState = StaminaStates.RegenBoost;
        }
        else if (!IsRegenerationBoostActive && CurrentState != StaminaStates.Exhausted)
        {
            CurrentState = StaminaStates.Normal;
        }
    }

    private void ExecuteState()
    {
        switch (CurrentState)
        {
            case StaminaStates.Normal:
                break;
            case StaminaStates.Exhausted:
                StaminaOutage(2);
                break;
            case StaminaStates.RegenBoost:
                
                break;

        }
    }

    // Setting default values to some variables on start
    private void Awake()
    {
        PMovement = GetComponent<Player_MovementController>();
        Stamina = MaxStamina;
        IsRegenerationBoostActive = false;
        IsStaminaCooldownActive = false;
        CanRegenerateStamina = true;
        OutOfStamina = false;
        IsStandingLongEnough = false;
        PMovement.SprintingEnabled = true;        
    }

    // Update function making Stamina drain while Spprinting or Jumping, and enabling Stamina Regeneration while not Sprinting or Jumping
    private void Update()
    {
        UpdateStates();
        StaminaUsage();
        RegenerationMechanic();
    }
    private void FixedUpdate()
    {
        ExecuteState();
    }



    // Stamina Usage when using more intensive movement mechanics
    private void StaminaUsage()
    {
        float Speed = PMovement.Rigidbody.velocity.magnitude;

        // Stamina usage when sprinting or jumping
        if (Stamina > 0 && PMovement.State == Player_MovementController.MovementStates.Sprinting && Speed > 0.2F)
        {
            Stamina -= UsageRate * Time.deltaTime;
            Stamina = Mathf.Max(Stamina, 0);
        }
            
        // Stamina goes back to >= 50  -  Enable Additional movement options
        else if (Stamina >= 50 && OutOfStamina == true)
        {
            OutOfStamina = false;
            PMovement.SprintingEnabled = true;
        }
        // Coroutine for increased regeneration starting the countdown after standing for a moment
        if (PMovement.State == Player_MovementController.MovementStates.Standing || (PMovement.State == Player_MovementController.MovementStates.Crouching && Speed == 0)) StartCoroutine(StandingTimeCounter());
    }

    // Out of Stamina Consequences - Disabled mechanics
    private void StaminaOutage(float SpeedReduction)
    {
        OutOfStamina = true;
        PMovement.SprintingEnabled = false;
        PMovement.State = Player_MovementController.MovementStates.Walking;
        PMovement.MoveSpeed -= OutageSpeedReduction;
        StartCoroutine(RegenerationCooldownNoStamina(OutageSpeedReduction));
        CurrentState = StaminaStates.Normal;
    }


    // Stamina Regeneration function  -  Increased by 2x when standing still
    private void RegenerationMechanic()
    {
        // After reaching 0 Stamina, disable Regeneration until coroutine starts
        if (Stamina == 0)
        {
            CanRegenerateStamina = false;
            StartCoroutine(RegenerationCooldownNoStamina(2));
        }
        else if (CanRegenerateStamina == true) StaminaRegeneration();
    }   
    private void StaminaRegeneration()
    {
        if (Stamina < MaxStamina && PMovement.State != Player_MovementController.MovementStates.Sprinting) StartCoroutine(RegenerationBoost(1));
    }



    // Coroutine checking if Player is standing still, or crouching in place for X amount of time to activate double boost
    private IEnumerator StandingTimeCounter()
    {
        float StartTime = Time.time;
        float Speed = PMovement.Rigidbody.velocity.magnitude;

        // Check if player is fulfilling the quota
        while (PMovement.State == Player_MovementController.MovementStates.Standing || (PMovement.State == Player_MovementController.MovementStates.Crouching && Speed == 0))
        {
            if (Time.time - StartTime >= StandingTimeRequired)
            {
                IsStandingLongEnough = true;
                yield break;
            }
            yield return null;
        }
        // Player moved, returning false, resetting counter
        IsStandingLongEnough = false;
    }
    // Regeneration Cooldown after reaching 0 stamina
    private IEnumerator RegenerationCooldownNoStamina(float delay)
    {
        if (!IsStaminaCooldownActive)
        {
            // Checks that Stamina Cooldown is Active, if it is, wait for X seconds, then enable regeneration
            IsStaminaCooldownActive = true;
            yield return new WaitForSeconds(delay);
            IsStaminaCooldownActive = false;

            CanRegenerateStamina = true;
            StaminaRegeneration();

            PMovement.MoveSpeed = PMovement.WalkingSpeed;
        }
    }
    // Coroutine for regeneration
    private IEnumerator RegenerationBoost(float delay)
    {
        // Check if regen boost active, after delay AND making sure player is standing long enough in place, start
        yield return new WaitForSeconds(delay);
        
        // Stamina Regeneration increased after standing for some time
        if (IsStandingLongEnough && Stamina < MaxStamina)
        {
            IsRegenerationBoostActive = true;
            Stamina += RegenerationRate * Time.deltaTime * 2;
        }
        // Stamina Regeneration base amount while moving
        else
        {
            IsRegenerationBoostActive = false;
            Stamina += RegenerationRate * Time.deltaTime;
        }
        // Lock max amount of Stamina to regenerate
        Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);
        // Check if player is standing, if not return false
        if (PMovement.State != Player_MovementController.MovementStates.Standing) IsStandingLongEnough = false; 
    }

}
