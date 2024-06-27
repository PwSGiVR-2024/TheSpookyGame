using System.Collections;
using UnityEngine;


public class Environment_DoorMechanics : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerPickDrop InteractionController;

    [Header("Door Settings")]
    [SerializeField] private Animator DoorAnimator;
    [SerializeField] private bool IsLocked;
    private bool InAnimation, IsOpen;

    [Header("Sounds")]
    [SerializeField] private AudioSource DoorOpen;
    [SerializeField] private AudioSource DoorClose;
    [SerializeField] private AudioSource DoorUnlock;




    private enum AnimationType
    {
        Open,
        Close
    }
    private void Awake()
    {
        PlayerPickDrop.InteractionEvent += Interact;
    }
    private void Interact(GameObject GameObjectReference)
    {
        if(GameObjectReference == transform.gameObject && !InAnimation)
        {
            DoorMechanics();
        }
    }

    private void DoorMechanics()
    {
        if (!IsOpen && !IsLocked)
            StartCoroutine(AnimateDoor(AnimationType.Open, DoorOpen));
        else if (!IsLocked)
            StartCoroutine(AnimateDoor(AnimationType.Close, DoorClose));
    }

    private IEnumerator AnimateDoor(AnimationType DoorAnimation, AudioSource Sound)
    {
        if (!InAnimation)
        {
            InAnimation = true;
            Sound.Play();
            DoorAnimator.Play(DoorAnimation.ToString());
            yield return new WaitForSeconds(DoorAnimator.GetCurrentAnimatorClipInfo(0).Length);
            InAnimation = false;

            if (DoorAnimation == AnimationType.Open)
            {
                IsOpen = true;
            }
            else if (DoorAnimation == AnimationType.Close)
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
