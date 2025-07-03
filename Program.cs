// Program.cs - Main entry point or game manager for simulation
using System;
using PicoPark; // Assuming all classes are in this namespace

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting PicoPark Basic Level Simulation...");

        // 1. Instantiate a Level
        Level level = new Level();
        level.StartPositionP1 = new Vector2D(1, 1);
        level.StartPositionP2 = new Vector2D(2, 1); // P2 starts slightly offset or different spot
        level.GoalPosition = new Vector2D(15, 1);   // Define a goal

        // 2. Create two Player objects
        Player player1 = new Player("P1", level.StartPositionP1);
        Player player2 = new Player("P2", level.StartPositionP2);
        level.AddPlayer(player1);
        level.AddPlayer(player2);

        // 3. Create puzzle components for the cooperative scenario
        // Scenario:
        // - P1 stands on pressureSwitch1 (pos 3,1) to open door1 (pos 5,1).
        // - P2 goes through door1, reaches toggleSwitch2 (pos 7,1).
        // - P2 activates toggleSwitch2 to open door2 (pos 9,1) allowing P1 to pass switch1 area.
        // - Both players proceed to goal.

        // Door 1 and Switch 1 (Pressure)
        Door door1 = new Door("door1", new Vector2D(5, 1));
        Switch pressureSwitch1 = new Switch("switch1", new Vector2D(3, 1), "door1", Switch.SwitchType.Pressure);

        level.AddPuzzleComponent(door1);
        level.AddPuzzleComponent(pressureSwitch1);

        // Door 2 and Switch 2 (Toggle)
        Door door2 = new Door("door2", new Vector2D(9, 1)); // P1 needs this to open to leave pressure switch area to reach goal
        Switch toggleSwitch2 = new Switch("switch2", new Vector2D(7, 1), "door2", Switch.SwitchType.Toggle);

        level.AddPuzzleComponent(door2);
        level.AddPuzzleComponent(toggleSwitch2);

        // Optional: A block puzzle if desired (not strictly part of this flow yet)
        // Block block1 = new Block("block1", new Vector2D(12, 1));
        // level.AddPuzzleComponent(block1);

        // 4. Initialize Level (links switches to doors)
        level.Initialize();
        level.PrintLevelState();

        // 5. Simulate player actions
        Console.WriteLine("\n--- Simulation Start ---");

        // P1 moves to pressureSwitch1
        Console.WriteLine("\nAction: P1 moves to pressureSwitch1 (3,1)");
        player1.Position = new Vector2D(3, 1); // Simulate movement
        level.Update(0.1f); // Update level to process switch
        level.PrintLevelState();
        // Expected: door1 is open

        if (!door1.IsOpen)
        {
            Console.WriteLine("Error: P1 on switch1, but door1 is not open!");
            return;
        }
        Console.WriteLine("P1 is on pressureSwitch1, door1 is open.");

        // P2 moves through door1 to toggleSwitch2
        Console.WriteLine("\nAction: P2 moves to toggleSwitch2 (7,1)");
        player2.Position = new Vector2D(7, 1); // Simulate movement past open door1
        level.Update(0.1f);
        level.PrintLevelState();

        Console.WriteLine("\nAction: P2 interacts with toggleSwitch2");
        player2.Interact(); // This will call HandlePlayerInteractionAttempt in Level
        level.Update(0.1f);
        level.PrintLevelState();
        // Expected: door2 is open

        if (!door2.IsOpen)
        {
            Console.WriteLine("Error: P2 interacted with switch2, but door2 is not open!");
            return;
        }
        Console.WriteLine("P2 activated toggleSwitch2, door2 is open.");

        // P1 moves off pressureSwitch1 (door1 should close) and towards goal
        Console.WriteLine("\nAction: P1 moves off pressureSwitch1 to (6,1) (past where door1 was)");
        player1.Position = new Vector2D(6, 1);
        level.Update(0.1f);
        level.PrintLevelState();
        // Expected: door1 is closed, door2 is open

        if (door1.IsOpen)
        {
            Console.WriteLine("Error: P1 moved off switch1, but door1 is still open!");
            //return; // This might be ok if P2 is also on it, but in this scenario, P2 is not.
        }
         if (!door2.IsOpen)
        {
            Console.WriteLine("Error: door2 should still be open!");
            return;
        }
        Console.WriteLine("P1 moved off pressureSwitch1 (door1 should be closed). P2 keeps door2 open.");

        // Both players move to Goal
        Console.WriteLine("\nAction: P1 moves to Goal (15,1)");
        player1.Position = level.GoalPosition;
        level.Update(0.1f);

        Console.WriteLine("\nAction: P2 moves to Goal (15,1)");
        player2.Position = level.GoalPosition;
        level.Update(0.1f); // This update should trigger level completion

        level.PrintLevelState();

        if (level.IsLevelComplete)
        {
            Console.WriteLine("\nSimulation Successful: Level Complete!");
        }
        else
        {
            Console.WriteLine("\nSimulation Incomplete: Players did not complete the level correctly.");
            Console.WriteLine($"P1 at {player1.Position}, P2 at {player2.Position}, Goal at {level.GoalPosition}");
        }

        Console.WriteLine("\n--- Simulation End ---");
    }
}
