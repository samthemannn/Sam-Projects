// Block class (Block.cs)
using System; // Required for Console

namespace PicoPark
{
    public class Block : PuzzleComponent
    {
        public Block(string id, Vector2D position) : base(id, position)
        {
        }

        public override void Interact(Player player)
        {
            // Conceptual: Allow player to "push" the block.
            // In a real engine, this would involve physics.
            // For now, we can simulate moving the block slightly in the direction the player is facing,
            // assuming we can determine that. For simplicity, let's assume the player's attempt to
            // interact means they are trying to push it.

            // A more robust implementation would check which side the player is on.
            // Let's assume if a player interacts, they push it slightly to the right for this example.
            // Or, better, the Level class could determine the direction based on player's previous movement or facing.

            // For this conceptual phase, we'll just log it.
            // The actual movement logic would be more complex and likely handled
            // in the Level or a physics system callback.

            Console.WriteLine($"{player.Id} interacted with Block {Id}. Block would be pushed.");

            // Example of how it *might* work if player's direction is known:
            // float pushDirection = DeterminePushDirection(player, this);
            // this.Position.X += pushDirection * 0.5f; // Push it a small amount
            // Console.WriteLine($"Block {Id} pushed to {this.Position.X}");
        }

        // Placeholder for a method that would determine push direction
        // private float DeterminePushDirection(Player player, Block block)
        // {
        //     // If player is to the left of the block and moving right, or just "facing right"
        //     // if (player.Position.X < block.Position.X) return 1.0f;
        //     // If player is to the right of the block and moving left, or just "facing left"
        //     // if (player.Position.X > block.Position.X) return -1.0f;
        //     return 0; // No push if directly above/below or no clear direction
        // }
    }
}
