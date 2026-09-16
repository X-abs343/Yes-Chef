using System;
using UnityEngine;

namespace YesChef.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Mount Points")]
        [SerializeField] private Transform holdPoint;

        public event Action<GameObject> OnItemPickedUp;
        public event Action OnItemCleared;

        public GameObject CurrentHeldItem { get; private set; }
        public bool HasItem => CurrentHeldItem != null;

        public bool TryHoldItem(GameObject item)
        {
            if (HasItem || item == null) return false;

            CurrentHeldItem = item;
            CurrentHeldItem.transform.SetParent(holdPoint);
            CurrentHeldItem.transform.localPosition = Vector3.zero;
            CurrentHeldItem.transform.localRotation = Quaternion.identity;

            // Disable physics while held
            if (CurrentHeldItem.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
            }
            if (CurrentHeldItem.TryGetComponent<Collider>(out var col))
            {
                col.enabled = false;
            }

            OnItemPickedUp?.Invoke(CurrentHeldItem);
            return true;
        }

        public GameObject ClearHeldItem()
        {
            if (!HasItem) return null;

            GameObject released = CurrentHeldItem;
            CurrentHeldItem = null;
            released.transform.SetParent(null);

            OnItemCleared?.Invoke();
            return released;
        }

        public void DestroyHeldItem()
        {
            if (!HasItem) return;

            GameObject toDestroy = CurrentHeldItem;
            CurrentHeldItem = null;
            Destroy(toDestroy);

            OnItemCleared?.Invoke();
        }
    }
}