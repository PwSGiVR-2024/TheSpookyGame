using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Script Imports")]
    public PlayerPickDrop InteractDetect;
    public InventoryManagement InventoryManage;

    [Header("Door Settings")]
    [SerializeField] private Animator DoorAnimator;
    private bool Open;
    public GameObject RequiredKey;
    public bool IsLocked;
    private string key;

    public string Key { get; private set; }

    [Header("Sounds")]
    [SerializeField] private AudioSource DoorOpen;
    [SerializeField] private AudioSource DoorClose;
    [SerializeField] private AudioSource DoorUnlock;

    private bool isAnimating;
    public bool IsOpen { get => Open; set => Open = value; }
    private enum DoorAnimationType
    {
        Open,
        Close
    }

    private void Awake()
    {
        key = RequiredKey.name;
    }
    private void Start()
    {
        PlayerPickDrop.InteractionEvent += Interact;
    }

    private void Interact(GameObject GameObjectReference)
    {
        if (GameObjectReference == transform.gameObject && !isAnimating)
        {
            DoorMechanics();
            TryUnlockDoor();
        }
    }

    private void DoorMechanics()
    {
        if (!IsOpen && !IsLocked)
            StartCoroutine(AnimateDoor(DoorAnimationType.Open, DoorOpen));
        else if (!IsLocked)
            StartCoroutine(AnimateDoor(DoorAnimationType.Close, DoorClose));
    }
   
    private void TryUnlockDoor()
    {
        if (InteractDetect.IsDoor && Input.GetKeyDown(InteractDetect.InteractKey) && IsLocked)
        {
            if (InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] == RequiredKey.name)
            {
                IsLocked = false;
                DoorUnlock.Play();
            }
            else return;
        }
    }

    private IEnumerator AnimateDoor(DoorAnimationType DoorAnimation, AudioSource Sound)
    {
        if (!isAnimating)
        {
            isAnimating = true;
            Sound.Play();
            DoorAnimator.Play(DoorAnimation.ToString());
            yield return new WaitForSeconds(DoorAnimator.GetCurrentAnimatorClipInfo(0).Length);
            isAnimating = false;

            // Optionally add logic to handle door state after the animation
            if (DoorAnimation == DoorAnimationType.Open)
            {
                IsOpen = true;
            }
            else if (DoorAnimation == DoorAnimationType.Close)
            {
                IsOpen = false;
            }
        }
    }

    private void OnDisable()
    {
        PlayerPickDrop.InteractionEvent -= Interact;
    }
}
