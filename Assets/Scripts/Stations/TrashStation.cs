using UnityEngine;
using YesChef.Feedback;
using YesChef.Player;

namespace YesChef.Stations
{
    public class TrashStation : BaseStation
    {
        public override string InteractionPrompt => "Throw away item";

        public override bool CanInteract(PlayerInventory inventory)
        {
            return inventory.HasItem;
        }

        public override void Interact(PlayerInventory inventory)
        {
            if (!CanInteract(inventory)) return;

            inventory.DestroyHeldItem();
            PlayBumpFeedback();

            // Fire sad head-drop feedback on player
            if (inventory.TryGetComponent<PlayerVisualFeedback>(out var juice))
            {
                juice.PlaySadTilt();
            }
        }
    }
}