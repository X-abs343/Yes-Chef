using System.Collections;
using UnityEngine;
using YesChef.Ingredients;
using YesChef.Player;

namespace YesChef.Stations
{
    public class StoveStation : BaseStation
    {
        [System.Serializable]
        public class StoveSlot
        {
            public Transform mountPoint;
            public IngredientInstance currentItem;
            public float progress;
            public bool isCooking;
            public Coroutine cookRoutine;
            public UI.WorldProgressBar progressBar;
        }

        [Header("Stove Config")]
        [SerializeField] private StoveSlot[] slots = new StoveSlot[2];
        [SerializeField] private float cookDuration = 6.0f;

        public StoveSlot[] Slots => slots;
        public override string InteractionPrompt => "Stove: Cook / Take Meat";

        public override bool CanInteract(PlayerInventory inventory)
        {
            // Can take cooked meat
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].currentItem != null && slots[i].currentItem.IsPrepared && !inventory.HasItem)
                    return true;
            }

            // Can place raw meat
            if (inventory.HasItem)
            {
                var inst = inventory.CurrentHeldItem.GetComponent<IngredientInstance>();
                if (inst != null && inst.Data.Type == IngredientType.Meat && !inst.IsPrepared)
                {
                    return HasEmptySlot();
                }
            }

            return false;
        }

        public override void Interact(PlayerInventory inventory)
        {
            // Priority 1: Pick up finished meat
            if (!inventory.HasItem)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].currentItem != null && slots[i].currentItem.IsPrepared)
                    {
                        inventory.TryHoldItem(slots[i].currentItem.gameObject);
                        slots[i].currentItem = null;
                        slots[i].progress = 0f;
                        PlayBumpFeedback();
                        return;
                    }
                }
            }

            // Priority 2: Deposit raw meat into an empty slot
            if (inventory.HasItem)
            {
                var inst = inventory.CurrentHeldItem.GetComponent<IngredientInstance>();
                if (inst != null && inst.Data.Type == IngredientType.Meat && !inst.IsPrepared)
                {
                    int slotIndex = GetFirstEmptySlotIndex();
                    if (slotIndex != -1)
                    {
                        GameObject item = inventory.ClearHeldItem();
                        var meat = item.GetComponent<IngredientInstance>();
                        slots[slotIndex].currentItem = meat;
                        meat.transform.SetParent(slots[slotIndex].mountPoint);
                        meat.transform.localPosition = Vector3.zero;
                        meat.transform.localRotation = Quaternion.identity;

                        slots[slotIndex].cookRoutine = StartCoroutine(CookRoutine(slots[slotIndex]));
                        PlayBumpFeedback();
                    }
                }
            }
        }

        private IEnumerator CookRoutine(StoveSlot slot)
        {
            slot.isCooking = true;
            slot.progressBar?.SetVisible(true);
            slot.progress = 0f;

            Vector3 startLocal = slot.mountPoint.localPosition;

            while (slot.progress < 1.0f)
            {
                slot.progress += Time.deltaTime / cookDuration;
                slot.progressBar?.SetProgress(slot.progress);
                // Sizzle vibration bounce
                slot.mountPoint.localPosition = startLocal + new Vector3(0, Mathf.Sin(Time.time * 25f) * 0.02f, 0);
                yield return null;
            }

            slot.mountPoint.localPosition = startLocal;
            slot.progress = 1.0f;
            slot.isCooking = false;
            slot.progressBar?.SetVisible(false);
            slot.currentItem.SetPrepared();
            PlayBumpFeedback();
        }

        private bool HasEmptySlot() => GetFirstEmptySlotIndex() != -1;

        private int GetFirstEmptySlotIndex()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].currentItem == null) return i;
            }
            return -1;
        }
    }
}