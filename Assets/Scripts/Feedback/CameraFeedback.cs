using System.Collections;
using UnityEngine;
using YesChef.Player;
using YesChef.Stations;

namespace YesChef.Feedback
{
    public class CameraFeedback : MonoBehaviour
    {
        [Header("Shake Configuration")]
        [SerializeField] private float defaultDuration = 0.18f;
        [SerializeField] private float defaultMagnitude = 0.12f;

        private Vector3 _originalPosition;
        private Coroutine _shakeRoutine;

        private void Awake()
        {
            _originalPosition = transform.localPosition;
        }

        private void OnEnable()
        {
            // Subscribe to interaction failure events
            PlayerInteraction playerInteraction = FindFirstObjectByType<PlayerInteraction>();
            if (playerInteraction != null)
            {
                playerInteraction.OnInteractionFailed += TriggerLightShake;
            }

            DeliveryWindow.OnOrderDeliveredScore += HandleScoreFeedback;
        }

        private void OnDisable()
        {
            PlayerInteraction playerInteraction = FindFirstObjectByType<PlayerInteraction>();
            if (playerInteraction != null)
            {
                playerInteraction.OnInteractionFailed -= TriggerLightShake;
            }

            DeliveryWindow.OnOrderDeliveredScore -= HandleScoreFeedback;
        }

        public void TriggerLightShake()
        {
            TriggerShake(defaultDuration, defaultMagnitude);
        }

        private void HandleScoreFeedback(int score)
        {
            // Shake if order took too long and docked negative points
            if (score < 0)
            {
                TriggerShake(0.25f, 0.2f);
            }
        }

        public void TriggerShake(float duration, float magnitude)
        {
            if (_shakeRoutine != null) StopCoroutine(_shakeRoutine);
            _shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
        }

        private IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = _originalPosition + new Vector3(x, y, 0f);
                yield return null;
            }

            transform.localPosition = _originalPosition;
            _shakeRoutine = null;
        }
    }
}