using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        public bool open;
        public float smooth = 1;
        float DoorOpenAngle = -90;
        float DoorCloseAngle = 0;
        [SerializeField] AudioClip DoorOpen;
        [SerializeField] AudioClip DoorClose;

        PlayerPickDrop InteractDetect;
        InventoryManagement InventoryManage;

        public GameObject RequiredKey;
        public bool IsLocked;
        private string key;

        private void Awake()
        {
            key = RequiredKey.name;
        }

        private void OnEnable()
        {
            PlayerPickDrop.InteractionEvent += Interact;
        }



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
        }

        public void Interact(GameObject gameObjectReference)
        {
            if (gameObjectReference == gameObject)
            {
                InteractDoor();
            }
        }

        public void OpenDoor()
        {
            open = !open;
            if (open)
            {
                AudioManager.Instance.PlaySound(DoorOpen);
            }
            else
            {
                AudioManager.Instance.PlaySound(DoorClose);
            }
        }

        private void InteractDoor()
        {
            if (!IsLocked)
            {
                StartCoroutine(ToggleDoor());
            }
            else
            {
                UnlockDoor();
                
            }
        }

        void UnlockDoor()
        {
            if(IsLocked && InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] != RequiredKey.name) Debug.Log("Locked");
            else if(IsLocked && InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID] == RequiredKey.name) { Debug.Log("Unlocked"); }
        }

        private IEnumerator ToggleDoor()
        {
            OpenDoor();
            float clipLength = open ? DoorOpen.length : DoorClose.length;
            yield return new WaitForSeconds(clipLength);
        }
        private void OnDisable()
        {
            PlayerPickDrop.InteractionEvent -= Interact;
        }
    }

}
