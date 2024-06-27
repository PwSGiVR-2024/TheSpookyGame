using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        public bool open;
        public float smooth = 1.0f;
        float DoorOpenAngle = -90.0f;
        float DoorCloseAngle = 0.0f;
        public AudioSource asource;
        public AudioClip openDoor, closeDoor;
        public PlayerPickDrop InteractDetect;
        public bool IsLocked;
        private bool isAnimating;

        // Use this for initialization
        void Start()
        {
            asource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (open)
            {
                var target = Quaternion.Euler(0, DoorOpenAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
            }
            else
            {
                var target1 = Quaternion.Euler(0, DoorCloseAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target1, Time.deltaTime * 5 * smooth);
            }

            if (InteractDetect.IsDoor && Input.GetKeyDown(InteractDetect.InteractKey))
            {
                Interact();
            }
        }

        public void OpenDoor()
        {
            open = !open;
            asource.clip = open ? openDoor : closeDoor;
            asource.Play();
        }

        private void Interact()
        {
            if (isAnimating) return;

            if (!IsLocked)
            {
                StartCoroutine(ToggleDoor());
            }
            else
            {
                TryUnlockDoor();
            }
        }

        private void TryUnlockDoor()
        {
            if (InteractDetect.IsDoor && Input.GetKeyDown(InteractDetect.InteractKey) && IsLocked)
            {
                // Add logic here to check for the key and unlock the door if needed.
                // Assuming InventoryManagement is available and set up correctly.
                // if (InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] == RequiredKey.name)
                // {
                //     IsLocked = false;
                //     DoorUnlock.Play();  // You would need to set this audio source up if necessary.
                // }
            }
        }

        private IEnumerator ToggleDoor()
        {
            isAnimating = true;
            OpenDoor();
            yield return new WaitForSeconds(asource.clip.length);
            isAnimating = false;
        }
    }
}
