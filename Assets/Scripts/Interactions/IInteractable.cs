namespace YesChef.Interactions
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        bool CanInteract(Player.PlayerInventory inventory);
        void Interact(Player.PlayerInventory inventory);
    }
}