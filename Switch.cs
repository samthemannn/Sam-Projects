// Switch class (Switch.cs)
using System; // Required for Console

namespace PicoPark
{
    public class Switch : PuzzleComponent
    {
        public string TargetComponentId { get; private set; }
        private PuzzleComponent _linkedComponent; // Internal reference, set by Level

        // Type of switch: Toggle (player steps on/off) or Pressure (active while player is on it)
        public enum SwitchType { Toggle, Pressure }
        public SwitchType Type { get; private set; }

        private bool _isCurrentlyPressed; // Used for pressure switches state

        public Switch(string id, Vector2D position, string targetComponentId, SwitchType type = SwitchType.Toggle) : base(id, position)
        {
            TargetComponentId = targetComponentId;
            Type = type;
            _isCurrentlyPressed = false;
        }

        // Called by Level.Update() for pressure switches
        public void SetPressedState(bool isPressed)
        {
            if (Type != SwitchType.Pressure) return;

            if (isPressed == _isCurrentlyPressed) return; // No change in state

            _isCurrentlyPressed = isPressed;
            IsActive = _isCurrentlyPressed; // A pressure switch's "active" state is its pressed state.

            if (_isCurrentlyPressed)
            {
                ActivateLinkedComponent();
                Console.WriteLine($"Pressure Switch {Id} is now pressed.");
            }
            else
            {
                DeactivateLinkedComponent();
                Console.WriteLine($"Pressure Switch {Id} is now released.");
            }
        }

        public override void Interact(Player player)
        {
            // Standard interaction, typically for Toggle switches
            if (Type == SwitchType.Toggle)
            {
                IsActive = !IsActive; // Toggle its own state
                Console.WriteLine($"Switch {Id} (Toggle) {(IsActive ? "activated" : "deactivated")} by {player.Id}.");
                if (_linkedComponent != null)
                {
                    _linkedComponent.SetActive(IsActive); // Toggle linked component's state
                }
            }
            else if (Type == SwitchType.Pressure)
            {
                // Pressure switches are typically not "interacted" with via a button press,
                // but by standing on them. OnPlayerEnter/Exit handles their logic.
                // However, an explicit Interact call could force its state for debugging or specific puzzles.
                Console.WriteLine($"Player {player.Id} interacted with Pressure Switch {Id}. State depends on presence.");
            }
        }

        private void ActivateLinkedComponent()
        {
            if (_linkedComponent != null && !_linkedComponent.IsActive)
            {
                _linkedComponent.SetActive(true);
                Console.WriteLine($"Switch {Id} activated linked component {_linkedComponent.Id}.");
            }
        }

        private void DeactivateLinkedComponent()
        {
            // For pressure switches, only deactivate if no players are on it.
            // This logic might be better handled in Level's update loop if multiple players can be on one switch.
            if (Type == SwitchType.Pressure && _isPlayerOnSwitch) return; // Still pressed

            if (_linkedComponent != null && _linkedComponent.IsActive)
            {
                _linkedComponent.SetActive(false);
                Console.WriteLine($"Switch {Id} deactivated linked component {_linkedComponent.Id}.");
            }
        }

        // Method for the Level to establish the link to the target component
        public void LinkComponent(PuzzleComponent component)
        {
            if (component != null && component.Id == TargetComponentId)
            {
                _linkedComponent = component;
                Console.WriteLine($"Switch {Id} successfully linked to component {component.Id}.");
                // Initialize linked component state based on switch's initial state if needed
                // (e.g., if switch starts active)
                if(IsActive)
                {
                     _linkedComponent.SetActive(true);
                }
            }
            else
            {
                Console.WriteLine($"Warning: Switch {Id} could not link to component with ID {TargetComponentId}. Provided component ID: {(component?.Id ?? "null")}");
            }
        }

        // Override SetActive if the switch itself can be activated/deactivated by something else
        // (e.g. a master switch)
        public override void SetActive(bool active)
        {
            base.SetActive(active); // Sets IsActive property
            Console.WriteLine($"Switch {Id} active state externally set to {IsActive}.");
            if (_linkedComponent != null)
            {
                _linkedComponent.SetActive(IsActive);
            }
        }
    }
}
