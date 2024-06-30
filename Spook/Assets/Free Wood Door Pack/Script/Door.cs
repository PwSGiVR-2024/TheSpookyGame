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

        public GameObject RequiredKey;
        public bool IsLocked;
        private string key;

        private InventoryManagement InventoryManage;

        private void Awake()
        {
            key = RequiredKey.name;
            // Ensure InventoryManage is assigned, either through the inspector or by finding the component in the scene
            InventoryManage = FindObjectOfType<InventoryManagement>();
            if (InventoryManage == null)
            {
                Debug.LogError("InventoryManage is not assigned in Door script.");
            }
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
            if (InventoryManage == null)
            {
                Debug.LogError("InventoryManage is not set.");
                return;
            }

            string currentItem = InventoryManage.CurrentlyHolding[InventoryManage.CurrentSlotID];
            if (IsLocked && currentItem != RequiredKey.name)
            {
                Debug.Log("Locked: The key does not match.");
            }
            else if (IsLocked && currentItem == RequiredKey.name)
            {
                Debug.Log("Unlocked");
                IsLocked = false;
                OpenDoor(); // Optionally open the door immediately after unlocking
            }
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
