// PuzzleComponent class (PuzzleComponent.cs)
namespace PicoPark
{
    public abstract class PuzzleComponent : GameObject
    {
        public bool IsActive { get; protected set; }

        protected PuzzleComponent(string id, Vector2D position) : base(id, position)
        {
            IsActive = false; // Default to inactive
        }

        // Abstract method for interaction, to be implemented by concrete puzzle components.
        // The player interacting is passed as a parameter if needed by the component.
        public abstract void Interact(Player player);

        // Some components might be activated/deactivated by other components (e.g., a door by a switch)
        public virtual void SetActive(bool active)
        {
            IsActive = active;
            // Optional: Add logging or event invocation here if needed
            // Console.WriteLine($"{Id} active state set to {IsActive}");
        }
    }
}
