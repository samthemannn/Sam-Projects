// Player class (Player.cs)
using System; // Required for Action

namespace PicoPark
{
    public class Player : GameObject
    {
        public bool IsOnGround { get; set; }
        public float MoveSpeed { get; set; } = 5.0f; // Arbitrary speed value
        public float JumpForce { get; set; } = 10.0f; // Arbitrary jump force

        // Action to be invoked when player tries to interact.
        // The Level class will subscribe to this and check for nearby interactable objects.
        public Action<Player> OnInteractAttempt;

        public Player(string id, Vector2D position) : base(id, position)
        {
            IsOnGround = true; // Assume player starts on the ground
        }

        // Conceptual movement. Actual implementation will depend on the physics engine.
        public void Move(float direction)
        {
            // direction > 0 for right, direction < 0 for left
            // This would typically change velocity or directly manipulate position.
            // For now, let's simulate position change.
            Position.X += direction * MoveSpeed * 0.016f; // Assuming 60 FPS, so deltaTime is ~0.016
            Console.WriteLine($"{Id} moved to {Position.X}");
        }

        // Conceptual jump.
        public void Jump()
        {
            if (IsOnGround)
            {
                IsOnGround = false;
                // In a real engine, this would apply an upward force/velocity.
                Console.WriteLine($"{Id} jumped.");
                // Simulate landing after a short delay for conceptual purposes
                // In a real game, collision detection would set IsOnGround.
            }
        }

        // Called when player presses the interact button.
        public void Interact()
        {
            Console.WriteLine($"{Id} attempts to interact.");
            OnInteractAttempt?.Invoke(this);
        }
    }
}
