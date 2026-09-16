using UnityEngine;
using UnityEngine.UI;

namespace YesChef.UI
{
    public class WorldProgressBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private GameObject visualContainer;

        private void Awake()
        {
            if (visualContainer == null) visualContainer = gameObject;
            SetVisible(false);
        }

        public void SetProgress(float progress)
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = Mathf.Clamp01(progress);
            }
        }

        public void SetVisible(bool visible)
        {
            if (visualContainer != null && visualContainer.activeSelf != visible)
            {
                visualContainer.SetActive(visible);
            }
        }
    }
}