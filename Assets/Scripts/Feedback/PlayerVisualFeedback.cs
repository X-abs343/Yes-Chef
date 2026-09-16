using System.Collections;
using UnityEngine;
using YesChef.Player;

namespace YesChef.Feedback
{
    public class PlayerVisualFeedback : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private Transform visualsRoot;
        [SerializeField] private PlayerController controller;

        [Header("Locomotion Bobbing")]
        [SerializeField] private float bobFrequency = 14f;
        [SerializeField] private float bobHeight = 0.08f;

        [Header("Stop Squash & Stretch")]
        [SerializeField] private Vector3 squashScale = new Vector3(1.15f, 0.85f, 1.15f);
        [SerializeField] private float recoverSpeed = 10f;

        private Vector3 _originalScale;
        private Vector3 _originalLocalPos;
        private Quaternion _originalLocalRot;
        private Coroutine _activeRoutine;

        private void Awake()
        {
            if (visualsRoot != null)
            {
                _originalScale = visualsRoot.localScale;
                _originalLocalPos = visualsRoot.localPosition;
                _originalLocalRot = visualsRoot.localRotation;
            }

            if (controller != null)
            {
                controller.OnStoppedMoving += TriggerStopSquash;
            }
        }

        private void OnDestroy()
        {
            if (controller != null)
            {
                controller.OnStoppedMoving -= TriggerStopSquash;
            }
        }

        private void Update()
        {
            if (visualsRoot == null) return;

            // Handle Walk Bob
            if (controller != null && controller.IsMoving)
            {
                float verticalOffset = Mathf.Sin(Time.time * bobFrequency) * bobHeight;
                visualsRoot.localPosition = _originalLocalPos + new Vector3(0f, Mathf.Abs(verticalOffset), 0f);
            }
            else if (_activeRoutine == null)
            {
                visualsRoot.localPosition = Vector3.Lerp(visualsRoot.localPosition, _originalLocalPos, Time.deltaTime * recoverSpeed);
                visualsRoot.localScale = Vector3.Lerp(visualsRoot.localScale, _originalScale, Time.deltaTime * recoverSpeed);
                visualsRoot.localRotation = Quaternion.Slerp(visualsRoot.localRotation, _originalLocalRot, Time.deltaTime * recoverSpeed);
            }
        }

        private void TriggerStopSquash()
        {
            if (_activeRoutine != null) StopCoroutine(_activeRoutine);
            _activeRoutine = StartCoroutine(SquashAndRecoverRoutine());
        }

        private IEnumerator SquashAndRecoverRoutine()
        {
            visualsRoot.localScale = squashScale;
            // Slight forward tilt on stop
            visualsRoot.localRotation = _originalLocalRot * Quaternion.Euler(6f, 0f, 0f);

            while (Vector3.Distance(visualsRoot.localScale, _originalScale) > 0.01f)
            {
                visualsRoot.localScale = Vector3.Lerp(visualsRoot.localScale, _originalScale, Time.deltaTime * recoverSpeed);
                visualsRoot.localRotation = Quaternion.Slerp(visualsRoot.localRotation, _originalLocalRot, Time.deltaTime * recoverSpeed);
                yield return null;
            }

            visualsRoot.localScale = _originalScale;
            visualsRoot.localRotation = _originalLocalRot;
            _activeRoutine = null;
        }

        public void PlayCelebrationJump()
        {
            if (_activeRoutine != null) StopCoroutine(_activeRoutine);
            _activeRoutine = StartCoroutine(CelebrationRoutine());
        }

        private IEnumerator CelebrationRoutine()
        {
            float duration = 0.5f;
            float elapsed = 0f;
            float jumpHeight = 0.6f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Parabolic jump arc
                float y = Mathf.Sin(t * Mathf.PI) * jumpHeight;
                visualsRoot.localPosition = _originalLocalPos + new Vector3(0, y, 0);

                // 360 Spin around Y
                float angle = Mathf.Lerp(0f, 360f, t);
                visualsRoot.localRotation = _originalLocalRot * Quaternion.Euler(0, angle, 0);

                yield return null;
            }

            visualsRoot.localPosition = _originalLocalPos;
            visualsRoot.localRotation = _originalLocalRot;
            _activeRoutine = null;
        }

        public void PlaySadTilt()
        {
            if (_activeRoutine != null) StopCoroutine(_activeRoutine);
            _activeRoutine = StartCoroutine(SadTiltRoutine());
        }

        public void PlayStopSquashManual()
        {
            TriggerStopSquash();
        }

        private IEnumerator SadTiltRoutine()
        {
            float duration = 0.45f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Dip down and return
                float tilt = Mathf.Sin(t * Mathf.PI) * 18f;
                visualsRoot.localRotation = _originalLocalRot * Quaternion.Euler(tilt, 0, 0);
                yield return null;
            }

            visualsRoot.localRotation = _originalLocalRot;
            _activeRoutine = null;
        }
    }
}