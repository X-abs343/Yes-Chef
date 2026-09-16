using System.Collections;
using UnityEngine;
using YesChef.Ingredients;
using YesChef.Player;

namespace YesChef.Stations
{
    public class CuttingStation : BaseStation
    {
        [Header("Mount Point")]
        [SerializeField] private Transform foodSlot;
        [SerializeField] private float cutDuration = 2.0f;
        [SerializeField] private UI.WorldProgressBar progressBar;

        private IngredientInstance _currentIngredient;
        private bool _isChopping;
        private float _progress;

        public float Progress => _progress;
        public bool IsOccupied => _currentIngredient != null;
        public bool IsFinished => IsOccupied && _currentIngredient.IsPrepared;

        public override string InteractionPrompt => IsFinished ? "Take Chopped Veggie" : (_isChopping ? "Chopping..." : "Place Veggie");

        public override bool CanInteract(PlayerInventory inventory)
        {
            if (IsFinished && !inventory.HasItem) return true;

            if (!IsOccupied && inventory.HasItem)
            {
                var inst = inventory.CurrentHeldItem.GetComponent<IngredientInstance>();
                return inst != null && inst.Data.Type == IngredientType.Vegetables && !inst.IsPrepared;
            }

            return false;
        }

        public override void Interact(PlayerInventory inventory)
        {
            if (IsFinished && !inventory.HasItem)
            {
                // Retrieve prepared food
                inventory.TryHoldItem(_currentIngredient.gameObject);
                _currentIngredient = null;
                _progress = 0f;
                PlayBumpFeedback();
                return;
            }

            if (!IsOccupied && inventory.HasItem)
            {
                // Deposit vegetable
                GameObject item = inventory.ClearHeldItem();
                _currentIngredient = item.GetComponent<IngredientInstance>();
                _currentIngredient.transform.SetParent(foodSlot);
                _currentIngredient.transform.localPosition = Vector3.zero;
                _currentIngredient.transform.localRotation = Quaternion.identity;

                StartCoroutine(ChopRoutine());
                PlayBumpFeedback();
            }
        }

        private IEnumerator ChopRoutine()
        {
            _isChopping = true;
            progressBar?.SetVisible(true);
            _progress = 0f;

            Vector3 startPos = stationVisualRoot.localPosition;

            while (_progress < 1.0f)
            {
                _progress += Time.deltaTime / cutDuration;
                progressBar?.SetProgress(_progress);
                // Wobble juice while cutting
                stationVisualRoot.localPosition = startPos + new Vector3(Mathf.Sin(Time.time * 40f) * 0.04f, 0, 0);
                yield return null;
            }

            stationVisualRoot.localPosition = startPos;
            _progress = 1.0f;
            _isChopping = false;
            progressBar?.SetVisible(false);
            _currentIngredient.SetPrepared();
            PlayBumpFeedback();
        }
    }
}