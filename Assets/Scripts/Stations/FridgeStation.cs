using UnityEngine;
using YesChef.Ingredients;
using YesChef.Player;

namespace YesChef.Stations
{
    public class FridgeStation : BaseStation
    {
        [Header("Registry")]
        [SerializeField] private GameObject ingredientPrefab;
        [SerializeField] private IngredientData[] availableIngredients;

        private int _currentIndex = 0;

        public override string InteractionPrompt => $"Take {availableIngredients[_currentIndex].DisplayName} (Press E)";

        public override bool CanInteract(PlayerInventory inventory)
        {
            return !inventory.HasItem && availableIngredients != null && availableIngredients.Length > 0;
        }

        public override void Interact(PlayerInventory inventory)
        {
            if (!CanInteract(inventory)) return;

            IngredientData chosenData = availableIngredients[_currentIndex];
            // Advance cycle for next interaction
            _currentIndex = (_currentIndex + 1) % availableIngredients.Length;

            GameObject spawnedObj = Instantiate(ingredientPrefab);
            IngredientInstance instance = spawnedObj.GetComponent<IngredientInstance>();
            instance.Initialize(chosenData, chosenData.Type == IngredientType.Cheese ? IngredientState.Prepared : IngredientState.Raw);

            inventory.TryHoldItem(spawnedObj);
            PlayBumpFeedback();
        }
    }
}