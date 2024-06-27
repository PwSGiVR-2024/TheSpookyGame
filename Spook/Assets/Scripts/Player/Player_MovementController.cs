using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public static class ListExtensions
{
    private static readonly System.Random RNG = new();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = RNG.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}


public class Player_MovementController : MonoBehaviour
{    
    [HideInInspector] public Rigidbody Rigidbody { get; set; }
    public Transform orientation;
    public Transform cameraTransform;
    private Vector3 initialCameraPosition;
    private Vector3 MovementDirection;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource WoodFootsteps;
    [SerializeField] private AudioSource TilesFootsteps;
    [SerializeField] private AudioSource RugFootsteps;

    [Header("Sound Effects")]
    [SerializeField] private float DefaultFootstepsVolume = 0.7F;
    private float FootstepsVolume;
    private float SoundSpeed;
    private readonly float SoundInterval = 0.125F;
    private float LastStep;


    [Header("Movement Settings")]
    [SerializeField] private float WalkSpeed = 5;
    [SerializeField] private float CrouchedSpeed;
    [SerializeField] private float SprintingSpeed;
    [SerializeField] private float GroundDrag;
    [HideInInspector] public float MoveSpeed;


    [Header("Camera Bobbing Settings")]
    [SerializeField] private float BobbingSpeed;
    [SerializeField] private float BobbingAmount;
    [SerializeField] private float BobbingMidpoint;
    private float BobbingTimer = 0;

    

    // Crouching Settings - Available only in code - Don't change you dumbass!
    private float DefaultYScale;
    private const float CrouchForce = 5;
    private readonly float CrouchingYScale = 0.75F;

    [Header("KeyBinds")]
    [SerializeField] private KeyCode SprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode CrouchKey = KeyCode.LeftControl;

    [Header("GroundCheck")]
    [SerializeField] private float PlayerHeight = 1.8F;
    public LayerMask IsGround;
    private bool Grounded;

    [Header("Slope Handling")]
    [SerializeField] private float MaxAngle;
    private RaycastHit SlopeHit;

    [Header("Function Checks")]
    public MovementStates State;
    private bool SprintEnabled;

    [Header("Post-Processing")]
    public Image SpeedLines;
    public PostProcessVolume PostProcessing;
    public Vignette Vignette;


    [Header("Vignette Settings")]
    [SerializeField] private float VignetteCrouched;
    [SerializeField] private float FadeDuration = 1.5F;
    [SerializeField] private float FadeTimer = 0;

    // Movement input variables
    private float HInput;
    private float VInput;


    public float MovementSpeed { get => MoveSpeed; set => MoveSpeed = value; }
    public float WalkingSpeed { get => WalkSpeed; set => WalkSpeed = value; }
    public bool SprintingEnabled { get => SprintEnabled; set => SprintEnabled = value; }

    // Sound Effect stuff
    private WalkableSurface Surface;
    private Material CurrentMaterial;
    private string StandingOn;

    private Dictionary<WalkableSurface, AudioSource> SurfaceAudioMap;
    private Dictionary<WalkableSurface, List<AudioClip>> SurfaceStepSounds;


    private void SoundLoadOnAwake()
    {
        SurfaceAudioMap = new Dictionary<WalkableSurface, AudioSource>()
        {
            { WalkableSurface.Wood, WoodFootsteps },
            { WalkableSurface.Tiles, TilesFootsteps },
            { WalkableSurface.Rug, RugFootsteps },
        };

        SurfaceStepSounds = new Dictionary<WalkableSurface, List<AudioClip>>()
        {
            { WalkableSurface.Wood, new List<AudioClip>() },
            { WalkableSurface.Tiles, new List<AudioClip>() },
            { WalkableSurface.Rug, new List<AudioClip>() },
        };

        LoadFootStepSounds(WalkableSurface.Wood, "Wood");
        LoadFootStepSounds(WalkableSurface.Tiles, "Tiles");
        LoadFootStepSounds(WalkableSurface.Rug, "Rug");
    }
    private void LoadFootStepSounds(WalkableSurface surface, string folder)
    {
        for (int i = 1; i <= 4; i++)
        {
            string soundPath = $"Sounds/Player/Footsteps/{folder}/Footsteps_{folder}_Walk_{i:00}";
            AudioClip footstep = Resources.Load<AudioClip>(soundPath);

            if (footstep != null)
            {
                SurfaceStepSounds[surface].Add(footstep);
            }
            else
            {
                Debug.LogError($"Audio not found: {soundPath}");
            }
        }
    }

    //Checks

    public bool HasPlayerMoved(float MovementThreshold = 0.2F)
    {
        return Mathf.Abs(Rigidbody.velocity.magnitude) > MovementThreshold;
    }
    private IEnumerator FadeSpeedLines(float targetAlpha, float duration)
    {
        float startAlpha = SpeedLines.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            SpeedLines.color = new Color(SpeedLines.color.r, SpeedLines.color.g, SpeedLines.color.b, newAlpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SpeedLines.color = new Color(SpeedLines.color.r, SpeedLines.color.g, SpeedLines.color.b, targetAlpha);
    }
    // Available Movement States

    public enum WalkableSurface
    {
        Wood,
        Tiles,
        Rug
    }
    private void SurfaceCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, PlayerHeight * 0.5f + 0.2f, IsGround))
        {
            if (hit.transform.TryGetComponent<Renderer>(out var hitRenderer))
            {
                CurrentMaterial = hitRenderer.material;
                StandingOn = CurrentMaterial.ToString().Split(' ')[0];
            }
        }
    }
    private void UpdateCurrentSurface()
    {
        if (StandingOn != null)
        {
            if (StandingOn == "Planks")
            {
                Surface = WalkableSurface.Wood;
            }
            else if (StandingOn == "Tiles")
            {
                Surface = WalkableSurface.Tiles;
            }
            else if (StandingOn == "Rug")
            {
                Surface = WalkableSurface.Rug;
            }
            else return;

            UpdateSurfaceStepSounds();
        }
    }
    private void UpdateSurfaceStepSounds()
    {
        List<AudioClip> StepSounds = SurfaceStepSounds[Surface];

        // Randomly shuffle the step sounds for the current material
        StepSounds.Shuffle();
    }


    public enum MovementStates
    {
        Standing,
        Walking,
        Sprinting,
        Crouching,
    }
    private void UpdateState()
    {
        float speed = Rigidbody.velocity.magnitude;

        if (Grounded && Input.GetKey(CrouchKey))
        {
            State = MovementStates.Crouching;
            FadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(FadeTimer / FadeDuration);
            Vignette.intensity.value = Mathf.Lerp(0, VignetteCrouched, t);
        }
        else if (speed == 0 && Grounded)
        {
            State = MovementStates.Standing;
        }
        else if (Grounded && SprintEnabled && Input.GetKey(SprintKey))
        {
            State = MovementStates.Sprinting;
        }
        else if (Grounded)
        {
            State = MovementStates.Walking;
            Vignette.intensity.value = 0;

        }
        if (State != MovementStates.Crouching)
        {
            FadeTimer = 0;
        }
    }
    private void ExecuteState()
    {
        switch (State)
        {
            case MovementStates.Standing:
                SpeedLines.color = new Color(SpeedLines.color.r, SpeedLines.color.g, SpeedLines.color.b, 0);
                break;
            case MovementStates.Walking:
                SpeedLines.color = new Color(SpeedLines.color.r, SpeedLines.color.g, SpeedLines.color.b, 0);
                MoveSpeed = WalkSpeed;
                SoundSpeed = 4.5F * SoundInterval;
                break;
            case MovementStates.Sprinting:
                StartCoroutine(FadeSpeedLines(0.6F, FadeDuration));
                MoveSpeed = SprintingSpeed;
                SoundSpeed = 3 * SoundInterval;
                break;
            case MovementStates.Crouching:
                SpeedLines.color = new Color(SpeedLines.color.r, SpeedLines.color.g, SpeedLines.color.b, 0);
                MoveSpeed = CrouchedSpeed;
                SoundSpeed = 6.5F * SoundInterval;
                break;
        }
    }

    // Load shit on awake
    private void Awake()
    {
        DefaultYScale = transform.localScale.y;
        IsGround = LayerMask.GetMask("IsGround");
        Rigidbody = GetComponent<Rigidbody>();
        _ = GetComponent<Renderer>();

        SoundLoadOnAwake();

        Rigidbody.freezeRotation = true;
        Rigidbody.useGravity = true;
        SprintEnabled = true;
    }
    private void Start()
    {
        if (PostProcessing && !Vignette)
        {
            PostProcessing.profile.TryGetSettings(out Vignette);
        }
        Vignette.intensity.value = 0;


        initialCameraPosition = cameraTransform.localPosition;
    }

    private void Update()
    {
        GroundedDrag();
        PlayerInput();
        SpeedControl();
        UpdateState();
        UpdateCurrentSurface();
        SurfaceCheck();
        ApplyBobbingEffect();
    }
    private void FixedUpdate()
    {
        PlayerMovement();
        ExecuteState();
    }

    private void GroundedDrag()
    {
        Grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, IsGround);
        if (Grounded) Rigidbody.drag = GroundDrag;
        else Rigidbody.drag = 0;
    }

    // Player's input for the movement
    private void PlayerInput()
    {
        HInput = Input.GetAxisRaw("Horizontal");
        VInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(CrouchKey) && Grounded && !Input.GetKeyDown(SprintKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, CrouchingYScale, transform.localScale.z);
            Rigidbody.AddForce(Vector3.down * CrouchForce, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(CrouchKey) && Grounded)
        {
            transform.localScale = new Vector3(transform.localScale.x, DefaultYScale, transform.localScale.z);
        }

    }
    private void PlayerMovement()
    {
        MovementDirection = orientation.forward * VInput + orientation.right * HInput;
        Rigidbody.AddForce(10 * MoveSpeed * MovementDirection.normalized, ForceMode.Force);

        if (OnSlope())
        {
            Rigidbody.useGravity = false;
            Rigidbody.AddForce(6 * MoveSpeed * SlopeMoveDirection(), ForceMode.Force);
            if (Rigidbody.velocity.y > 0) Rigidbody.AddForce(Vector3.down * 24, ForceMode.Force);

            if (Rigidbody.velocity.y < 0) Rigidbody.AddForce(Vector3.up * 24, ForceMode.Force);
        }
        else Rigidbody.useGravity = true;
        PlayFootStepSounds(Surface);
    }

    // Playing a footstep sound, stereo effect to imitate legs
    private void PlayFootStepSounds(WalkableSurface Surface)
    {
        List<AudioClip> StepSounds = SurfaceStepSounds[Surface];
        if (StepSounds == null || StepSounds.Count == 0) return;

        if ((HInput != 0 || VInput != 0) && Time.time - LastStep > SoundSpeed)
        {
            int RandomID = Random.Range(0, StepSounds.Count);
            AudioSource CurrentAudio = SurfaceAudioMap[Surface];
            CurrentAudio.clip = StepSounds[RandomID];

            // Adjust the FootstepsVolume based on crouching state
            if (State == MovementStates.Crouching) FootstepsVolume = DefaultFootstepsVolume - 0.2F;
            else if (State == MovementStates.Sprinting) FootstepsVolume = DefaultFootstepsVolume + 0.3F;
            else if (State != MovementStates.Crouching || State != MovementStates.Sprinting) FootstepsVolume = DefaultFootstepsVolume;

            CurrentAudio.volume = FootstepsVolume;
            CurrentAudio.Play();

            LastStep = Time.time;
            if (Mathf.FloorToInt(Time.time / SoundSpeed) % 2 == 0) CurrentAudio.panStereo = -0.2F; // Left
            else CurrentAudio.panStereo = 0.2F; // Right
        }
    }

    // Reduced stopping time after releasing movement
    private void SpeedControl()
    {
        if (OnSlope())
        {
            if (Rigidbody.velocity.magnitude > MoveSpeed)
            {
                Rigidbody.velocity = Rigidbody.velocity.normalized * MoveSpeed;
            }
        }
        else
        {
            Vector3 FlatVelocity = new(Rigidbody.velocity.x, 0, Rigidbody.velocity.z);

            if (FlatVelocity.magnitude > 0.1F)
            {
                Rigidbody.drag = GroundDrag;
            }
            else
            {
                Rigidbody.drag = GroundDrag * 3;
            }
            if (FlatVelocity.magnitude > MoveSpeed)
            {
                Vector3 LimitedVelocity = FlatVelocity.normalized * MoveSpeed;
                Rigidbody.velocity = new Vector3(LimitedVelocity.x, Rigidbody.velocity.y, LimitedVelocity.z);
            }
        }
    }

    // Slope calculations allowing for movement on angled surfaces
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out SlopeHit, PlayerHeight * 0.5F + 0.3F))
        {
            float Angle = Vector3.Angle(Vector3.up, SlopeHit.normal);
            return Angle < MaxAngle && Angle != 0;
        }
        return false;
    }
    private Vector3 SlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(MovementDirection, SlopeHit.normal).normalized;
    }

    private void ApplyBobbingEffect()
    {
        if (State == MovementStates.Walking || State == MovementStates.Sprinting)
        {
            float waveslice = Mathf.Sin(BobbingTimer);
            BobbingTimer += BobbingSpeed * MoveSpeed * Time.deltaTime;
            if (BobbingTimer > Mathf.PI * 2)
            {
                BobbingTimer -= Mathf.PI * 2;
            }

            if (waveslice != 0)
            {
                float translateChange = waveslice * BobbingAmount;
                float totalAxes = Mathf.Abs(HInput) + Mathf.Abs(VInput);

                totalAxes = Mathf.Clamp(totalAxes, 0, 1);
                translateChange = totalAxes * translateChange;

                Vector3 localPosition = initialCameraPosition;
                localPosition.y = initialCameraPosition.y + translateChange;
                cameraTransform.localPosition = localPosition;
            }
        }
        else
        {
            BobbingTimer = 0;
            cameraTransform.localPosition = initialCameraPosition;
        }
    }
}