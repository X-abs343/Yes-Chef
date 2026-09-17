using System.Collections;
using TMPro;
using UnityEngine;

namespace YesChef.Feedback
{
    public class FloatingScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float floatDistance = 1.2f;
        [SerializeField] private float lifetime = 2.0f;

        private Vector3 _baseScale;

        public void Initialize(int score)
        {
            _baseScale = transform.localScale;

            if (scoreText != null)
            {
                if (score >= 0)
                {
                    scoreText.text = $"+{score}";
                    scoreText.color = new Color(0.2f, 1f, 0.4f);
                }
                else
                {
                    scoreText.text = $"{score}";
                    scoreText.color = new Color(1f, 0.25f, 0.25f);
                }
            }

            StartCoroutine(AnimateRoutine());
        }

        private IEnumerator AnimateRoutine()
        {
            Vector3 startPos = transform.localPosition;
            Vector3 endPos = startPos + new Vector3(0, floatDistance, 0);
            float elapsed = 0f;

            // Initial pop scale
            transform.localScale = _baseScale * 0.7f;

            while (elapsed < lifetime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lifetime;

                // Move up
                transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

                // Pop-in bounce
                if (t < 0.2f)
                {
                    transform.localScale = Vector3.Lerp(_baseScale * 0.7f, _baseScale * 1.2f, t / 0.2f);
                }
                else if (t < 0.35f)
                {
                    transform.localScale = Vector3.Lerp(_baseScale * 1.2f, _baseScale, (t - 0.2f) / 0.15f);
                }

                // Fade out near end
                if (t > 0.6f && canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, (t - 0.6f) / 0.4f);
                }

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}