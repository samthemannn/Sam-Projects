// Door class (Door.cs)
using System; // Required for Console

namespace PicoPark
{
    public class Door : PuzzleComponent
    {
        // A door is typically "open" when IsActive is true, and "closed" when false.
        public bool IsOpen => IsActive;

        public Door(string id, Vector2D position) : base(id, position)
        {
            // By default, a door starts closed (IsActive = false from PuzzleComponent)
        }

        public override void Interact(Player player)
        {
            // Players typically don't interact with doors directly to open them;
            // switches or other mechanisms do.
            // However, you could implement a "locked door" message or sound here if desired.
            if (IsOpen)
            {
                Console.WriteLine($"Door {Id} is already open. {player.Id} passes through.");
            }
            else
            {
                Console.WriteLine($"Door {Id} is closed. {player.Id} cannot pass.");
            }
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active); // This sets the IsActive property
            if (IsOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        public void Open()
        {
            // IsActive should be true here
            if (!base.IsActive) base.SetActive(true); // Ensure consistency
            Console.WriteLine($"Door {Id} is now Open.");
            // In a real game, this would trigger an animation and change its collision state.
        }

        public void Close()
        {
            // IsActive should be false here
            if (base.IsActive) base.SetActive(false); // Ensure consistency
            Console.WriteLine($"Door {Id} is now Closed.");
            // In a real game, this would trigger an animation and change its collision state.
        }
    }
}
