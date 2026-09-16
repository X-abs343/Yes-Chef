using System;
using UnityEngine;
using YesChef.Core;
using YesChef.Interactions;

namespace YesChef.Player
{
    [RequireComponent(typeof(PlayerInventory))]
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Detection Parameters")]
        [SerializeField] private Transform interactionCenter;
        [SerializeField] private float checkRadius = 1.0f;
        [SerializeField] private float forwardOffset = 0.8f;
        [SerializeField] private LayerMask interactableLayer;

        private PlayerInventory _inventory;
        private IInteractable _focusedInteractable;

        public event Action OnInteractionFailed;

        private void Awake()
        {
            _inventory = GetComponent<PlayerInventory>();
            if (interactionCenter == null) interactionCenter = transform;
        }

        private void Start()
        {
            if (InputReader.Instance != null)
            {
                InputReader.Instance.OnInteractEvent += TriggerInteraction;
            }
        }

        private void OnDestroy()
        {
            if (InputReader.Instance != null)
            {
                InputReader.Instance.OnInteractEvent -= TriggerInteraction;
            }
        }

        private void Update()
        {
            DetectInteractable();
        }

        private void DetectInteractable()
        {
            Vector3 center = interactionCenter.position + interactionCenter.forward * forwardOffset;
            Collider[] hits = Physics.OverlapSphere(center, checkRadius, interactableLayer);

            _focusedInteractable = null;
            float closestDist = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IInteractable>(out var interactable))
                {
                    float dist = Vector3.Distance(center, hit.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        _focusedInteractable = interactable;
                    }
                }
            }
        }

        private void TriggerInteraction()
        {
            if (_focusedInteractable != null)
            {
                if (_focusedInteractable.CanInteract(_inventory))
                {
                    _focusedInteractable.Interact(_inventory);
                }
                else
                {
                    OnInteractionFailed?.Invoke();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Transform t = interactionCenter != null ? interactionCenter : transform;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(t.position + t.forward * forwardOffset, checkRadius);
        }
    }
}