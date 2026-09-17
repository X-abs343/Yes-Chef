using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YesChef.Feedback;
using YesChef.Ingredients;
using YesChef.Orders;
using YesChef.Player;
using YesChef.UI;

namespace YesChef.Stations
{
    public class DeliveryWindow : BaseStation
    {
        [Header("Window Configuration")]
        [SerializeField] private int windowIndex;
        [SerializeField] private IngredientData[] possibleIngredients;
        [SerializeField] private float respawnDelay = 5.0f;

        [Header("UI & Feedback")]
        [SerializeField] private OrderTicketUI ticketUI;
        [SerializeField] private GameObject floatingScorePrefab;
        [SerializeField] private Transform scoreSpawnPoint;

        public static event Action<int> OnOrderDeliveredScore;

        public CustomerOrder ActiveOrder { get; private set; }
        public bool HasActiveOrder => ActiveOrder != null;

        public override string InteractionPrompt => HasActiveOrder ? "Deliver Prepared Ingredient" : "Waiting for Customer...";

        protected override void Awake()
        {
            base.Awake();
            if (scoreSpawnPoint == null) scoreSpawnPoint = transform;
        }

        private void Start()
        {
            SpawnNewOrder();
        }

        private void Update()
        {
            if (HasActiveOrder && ticketUI != null)
            {
                ticketUI.UpdateDisplay(ActiveOrder);
            }
        }

        public override bool CanInteract(PlayerInventory inventory)
        {
            if (!HasActiveOrder || !inventory.HasItem) return false;

            var inst = inventory.CurrentHeldItem.GetComponent<IngredientInstance>();
            if (inst == null || !inst.IsPrepared) return false;

            // Check if this item is actually needed
            return IsIngredientNeeded(inst.Data.Type);
        }

        public override void Interact(PlayerInventory inventory)
        {
            if (!CanInteract(inventory)) return;

            var inst = inventory.CurrentHeldItem.GetComponent<IngredientInstance>();
            if (ActiveOrder.TryFulfill(inst))
            {
                // Item matches: consume from hand
                inventory.DestroyHeldItem();
                PlayBumpFeedback();
                ticketUI.UpdateDisplay(ActiveOrder);

                if (ActiveOrder.IsComplete)
                {
                    CompleteOrder(inventory);
                }
            }
        }

        private void CompleteOrder(PlayerInventory inventory)
        {
            int earnedScore = ActiveOrder.CalculateFinalScore();
            OnOrderDeliveredScore?.Invoke(earnedScore);

            // Floating score popup
            if (floatingScorePrefab != null)
            {
                GameObject popup = Instantiate(floatingScorePrefab, scoreSpawnPoint.position, Quaternion.Euler(70f, 0f, 0f));
                if (popup.TryGetComponent<FloatingScoreUI>(out var scoreUI))
                {
                    scoreUI.Initialize(earnedScore);
                }
            }

            // Player victory 360 jump
            if (inventory.TryGetComponent<PlayerVisualFeedback>(out var playerJuice))
            {
                playerJuice.PlayCelebrationJump();
            }

            ActiveOrder = null;
            ticketUI?.SetVisible(false);

            StartCoroutine(RespawnOrderRoutine());
        }

        private IEnumerator RespawnOrderRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);
            SpawnNewOrder();
        }

        public void SpawnNewOrder()
        {
            if (possibleIngredients == null || possibleIngredients.Length == 0) return;

            // 50% 2 ingredients, 50% 3 ingredients
            int itemCount = UnityEngine.Random.value < 0.5f ? 2 : 3;
            List<IngredientData> list = new List<IngredientData>();

            for (int i = 0; i < itemCount; i++)
            {
                int randIdx = UnityEngine.Random.Range(0, possibleIngredients.Length);
                list.Add(possibleIngredients[randIdx]);
            }

            ActiveOrder = new CustomerOrder(list);
            ticketUI?.SetupOrder(ActiveOrder);
            PlayBumpFeedback();
        }

        private bool IsIngredientNeeded(IngredientType type)
        {
            if (ActiveOrder == null) return false;

            int needed = 0;
            int fulfilled = 0;

            foreach (var req in ActiveOrder.RequiredIngredients) if (req.Type == type) needed++;
            foreach (var ful in ActiveOrder.FulfilledIngredients) if (ful.Type == type) fulfilled++;

            return fulfilled < needed;
        }
    }
}