using System;
using UnityEngine;
using YesChef.Core;

namespace YesChef.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 8.5f;
        [SerializeField] private float rotationSpeed = 16f;

        private Rigidbody _rb;
        private Vector3 _inputDirection;
        private bool _wasMoving;

        public event Action OnStartedMoving;
        public event Action OnStoppedMoving;
        public bool IsMoving => _inputDirection.sqrMagnitude > 0.01f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Vector2 input = InputReader.Instance != null ? InputReader.Instance.MoveComposite : Vector2.zero;
            _inputDirection = new Vector3(input.x, 0f, input.y).normalized;

            bool isCurrentlyMoving = IsMoving;
            if (isCurrentlyMoving && !_wasMoving)
            {
                OnStartedMoving?.Invoke();
            }
            else if (!isCurrentlyMoving && _wasMoving)
            {
                OnStoppedMoving?.Invoke();
            }
            _wasMoving = isCurrentlyMoving;
        }

        private void FixedUpdate()
        {
            Vector3 targetVelocity = _inputDirection * moveSpeed;
            Vector3 velocity = _rb.linearVelocity;
            _rb.linearVelocity = new Vector3(targetVelocity.x, velocity.y, targetVelocity.z);

            if (IsMoving)
            {
                Quaternion targetRot = Quaternion.LookRotation(_inputDirection, Vector3.up);
                _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }
}