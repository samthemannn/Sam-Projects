// Level class (Level.cs)
using System;
using System.Collections.Generic;
using System.Linq; // Required for FirstOrDefault

namespace PicoPark
{
    public class Level
    {
        public List<Player> Players { get; private set; }
        public List<PuzzleComponent> PuzzleComponents { get; private set; }
        private Dictionary<string, PuzzleComponent> _puzzleComponentMap; // For easy lookup by ID

        public Vector2D StartPositionP1 { get; set; } = new Vector2D(1, 1); // Default start positions
        public Vector2D StartPositionP2 { get; set; } = new Vector2D(2, 1);
        public Vector2D GoalPosition { get; set; } = new Vector2D(10, 1); // Example goal
        public float InteractionRadius { get; set; } = 1.5f; // How close player needs to be to interact

        public bool IsLevelComplete { get; private set; }

        public Level()
        {
            Players = new List<Player>();
            PuzzleComponents = new List<PuzzleComponent>();
            _puzzleComponentMap = new Dictionary<string, PuzzleComponent>();
            IsLevelComplete = false;
        }

        public void AddPlayer(Player player)
        {
            if (player != null && !Players.Contains(player))
            {
                Players.Add(player);
                player.OnInteractAttempt += HandlePlayerInteractionAttempt; // Subscribe to interaction event
                Console.WriteLine($"Player {player.Id} added to level at {player.Position}.");
            }
        }

        public void AddPuzzleComponent(PuzzleComponent component)
        {
            if (component != null && !_puzzleComponentMap.ContainsKey(component.Id))
            {
                PuzzleComponents.Add(component);
                _puzzleComponentMap.Add(component.Id, component);
                Console.WriteLine($"PuzzleComponent {component.Id} ({component.GetType().Name}) added to level at {component.Position}.");

            }
        }

        public void Initialize()
        {
            Console.WriteLine("Initializing Level...");
            // Link switches to their target components
            foreach (var component in PuzzleComponents)
            {
                if (component is Switch switchComponent)
                {
                    if (!string.IsNullOrEmpty(switchComponent.TargetComponentId) && _puzzleComponentMap.TryGetValue(switchComponent.TargetComponentId, out PuzzleComponent target))
                    {
                        switchComponent.LinkComponent(target);
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Switch {switchComponent.Id} has an invalid or missing TargetComponentId: {switchComponent.TargetComponentId}");
                    }
                }
            }
            Console.WriteLine("Level Initialized.");
        }

        private void HandlePlayerInteractionAttempt(Player player)
        {
            Console.WriteLine($"Handling interaction attempt for {player.Id} at {player.Position}.");
            // Find nearby interactable puzzle components
            foreach (var component in PuzzleComponents)
            {
                // Using a simple distance check. In a real engine, this might use colliders/triggers.
                float distance = Vector2D.Subtract(player.Position, component.Position).Length(); // Assuming Vector2D has Length()
                if (distance <= InteractionRadius)
                {
                    // Don't interact with Doors directly in this way, they are controlled by switches
                    if (component is Door) continue;

                    Console.WriteLine($"{player.Id} is close enough to {component.Id} ({component.GetType().Name}). Interacting.");
                    component.Interact(player);
                    // Typically, a player interacts with one component at a time.
                    // Could add logic to find the closest or a specific one.
                    return;
                }
            }
            Console.WriteLine($"{player.Id} tried to interact, but no components were in range or eligible.");
        }


        // Conceptual update loop for the level
        public void Update(float deltaTime)
        {
            if (IsLevelComplete) return;

            // Simulate player input processing (in a real game, this comes from an input manager)
            // For now, we'll just call conceptual updates or checks

            // Update puzzle components (e.g., pressure switches)
            foreach (var component in PuzzleComponents)
            {
                if (component is Switch switchComponent && switchComponent.Type == Switch.SwitchType.Pressure)
                {
                    bool playerOnSwitch = false;
                    foreach (var player in Players)
                    {
                        // Simple bounding box check for pressure plate
                        // Assuming switch is 1x1 unit centered at its position, and player pivot is at their base center.
                        // Player's Y should be compared against the switch's Y, assuming platformer view.
                        // More precise collision would use bounding boxes.
                        if (player.Position.X >= switchComponent.Position.X - 0.5f &&
                            player.Position.X <= switchComponent.Position.X + 0.5f &&
                            Math.Abs(player.Position.Y - switchComponent.Position.Y) < 0.5f) // Check Y proximity
                        {
                            playerOnSwitch = true;
                            break;
                        }
                    }
                    switchComponent.SetPressedState(playerOnSwitch);
                }
            }

            // Check for level completion
            CheckLevelCompletion();
        }

        private void CheckLevelCompletion()
        {
            // Example: All players must reach the goal position
            bool allPlayersAtGoal = true;
            if (!Players.Any()) allPlayersAtGoal = false; // No players, no win

            foreach (var player in Players)
            {
                // Simple distance check for goal
                if (Vector2D.Subtract(player.Position, GoalPosition).Length() > 1.0f) // 1.0f is tolerance
                {
                    allPlayersAtGoal = false;
                    break;
                }
            }

            if (allPlayersAtGoal && Players.Any())
            {
                IsLevelComplete = true;
                Console.WriteLine("Level Complete! All players reached the goal.");
                // Trigger next level load or win screen
            }
        }

        public void PrintLevelState()
        {
            Console.WriteLine("\n--- Level State ---");
            foreach (var player in Players)
            {
                Console.WriteLine($"Player: {player.Id} at {player.Position}, OnGround: {player.IsOnGround}");
            }
            foreach (var component in PuzzleComponents)
            {
                string state = component.IsActive ? "Active" : "Inactive";
                if (component is Door door) state = door.IsOpen ? "Open" : "Closed";
                Console.WriteLine($"Component: {component.Id} ({component.GetType().Name}) at {component.Position}, State: {state}");
                if (component is Switch sw)
                {
                    Console.WriteLine($"  -> Switch Type: {sw.Type}, Target: {sw.TargetComponentId}");
                }
            }
            Console.WriteLine($"Level Complete: {IsLevelComplete}");
            Console.WriteLine("---------------------\n");
        }
    }
}
