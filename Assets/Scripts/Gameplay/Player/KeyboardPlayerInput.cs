using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer.Gameplay
{
    public class KeyboardPlayerInput : APlayerInput
    {
        InputAction moveAction;
        InputAction jumpAction;
        InputAction dashAction;

        private void Awake()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            jumpAction = InputSystem.actions.FindAction("Jump");
            dashAction = InputSystem.actions.FindAction("Dash");
        }

        private void Update()
        {
            Horizontal = moveAction.ReadValue<Vector2>().x;
            Jump = jumpAction.WasPressedThisFrame();
            Dash = dashAction.WasPressedThisFrame();
        }
    }
}
