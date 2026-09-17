using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Ingredients;
using YesChef.Orders;

namespace YesChef.UI
{
    public class OrderTicketUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject contentRoot;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Transform iconsContainer;
        [SerializeField] private Image iconTemplate;

        [Header("Icon Colors")]
        [SerializeField] private Color vegColor = new Color(0.18f, 0.8f, 0.44f);
        [SerializeField] private Color cheeseColor = new Color(0.95f, 0.77f, 0.06f);
        [SerializeField] private Color meatColor = new Color(0.85f, 0.25f, 0.2f);
        [SerializeField] private Color fulfilledColor = new Color(0.3f, 0.3f, 0.3f, 0.35f);

        private List<Image> _spawnedIcons = new List<Image>();

        private void Awake()
        {
            if (iconTemplate != null) iconTemplate.gameObject.SetActive(false);
            SetVisible(false);
        }

        public void SetVisible(bool visible)
        {
            if (contentRoot != null) contentRoot.SetActive(visible);
        }

        public void SetupOrder(CustomerOrder order)
        {
            ClearIcons();
            if (order == null) return;

            SetVisible(true);

            for (int i = 0; i < order.RequiredIngredients.Count; i++)
            {
                Image icon = Instantiate(iconTemplate, iconsContainer);
                icon.gameObject.SetActive(true);
                icon.color = GetTypeColor(order.RequiredIngredients[i].Type);
                _spawnedIcons.Add(icon);
            }

            UpdateDisplay(order);
        }

        public void UpdateDisplay(CustomerOrder order)
        {
            if (order == null) return;

            // Timer formatted as elapsed seconds
            if (timerText != null)
            {
                int seconds = Mathf.FloorToInt(order.ElapsedSeconds);
                timerText.text = $"{seconds}s";
                timerText.color = seconds > 25 ? new Color(1f, 0.3f, 0.3f) : Color.white;
            }

            // Correctly match fulfilled items by TYPE, not by simple index
            List<IngredientType> remainingFulfilled = new List<IngredientType>();
            foreach (var f in order.FulfilledIngredients)
            {
                remainingFulfilled.Add(f.Type);
            }

            for (int i = 0; i < order.RequiredIngredients.Count && i < _spawnedIcons.Count; i++)
            {
                IngredientType reqType = order.RequiredIngredients[i].Type;

                if (remainingFulfilled.Contains(reqType))
                {
                    _spawnedIcons[i].color = fulfilledColor;
                    remainingFulfilled.Remove(reqType); // Consume this fulfill match
                }
                else
                {
                    _spawnedIcons[i].color = GetTypeColor(reqType);
                }
            }
        }

        private Color GetTypeColor(IngredientType type)
        {
            switch (type)
            {
                case IngredientType.Vegetables: return vegColor;
                case IngredientType.Cheese: return cheeseColor;
                case IngredientType.Meat: return meatColor;
                default: return Color.white;
            }
        }

        private void ClearIcons()
        {
            foreach (var icon in _spawnedIcons)
            {
                if (icon != null) Destroy(icon.gameObject);
            }
            _spawnedIcons.Clear();
        }
    }
}