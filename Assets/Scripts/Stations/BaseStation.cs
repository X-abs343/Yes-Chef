using System.Collections;
using UnityEngine;
using YesChef.Interactions;
using YesChef.Player;

namespace YesChef.Stations
{
    [RequireComponent(typeof(Collider))]
    public abstract class BaseStation : MonoBehaviour, IInteractable
    {
        [Header("Base Visuals")]
        [SerializeField] protected Transform stationVisualRoot;

        public abstract string InteractionPrompt { get; }
        public abstract bool CanInteract(PlayerInventory inventory);
        public abstract void Interact(PlayerInventory inventory);

        private Vector3 _cachedBaseScale;
        private Coroutine _bumpCoroutine;

        protected virtual void Awake()
        {
            if (stationVisualRoot == null) stationVisualRoot = transform;
            _cachedBaseScale = stationVisualRoot.localScale;
        }

        /// <summary>
        /// Micro-bounce feedback on the station visual root using relative delta scaling.
        /// </summary>
        protected void PlayBumpFeedback()
        {
            if (_bumpCoroutine != null)
            {
                StopCoroutine(_bumpCoroutine);
            }
            _bumpCoroutine = StartCoroutine(BumpRoutine());
        }

        private IEnumerator BumpRoutine()
        {
            // Multiplicative squish preserves original non-uniform dimensions
            Vector3 targetScale = new Vector3(
                _cachedBaseScale.x * 1.08f,
                _cachedBaseScale.y * 0.92f,
                _cachedBaseScale.z * 1.08f
            );

            float elapsed = 0f;
            float duration = 0.15f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                stationVisualRoot.localScale = Vector3.Lerp(targetScale, _cachedBaseScale, t);
                yield return null;
            }

            stationVisualRoot.localScale = _cachedBaseScale;
            _bumpCoroutine = null;
        }
    }
}