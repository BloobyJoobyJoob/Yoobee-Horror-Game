using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public float SprintSpeed;
    public float SneakSpeed;
    public float WalkSpeed;

    private CharacterController controller;
    private PlayerInput input;

    Vector2 walk;
    float Speed;

    Vector2 movement;
    bool sprint;
    bool sneak;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Speed = sneak ? SneakSpeed : sprint ? SprintSpeed : WalkSpeed;

        if (movement.y > 0)
        {
            walk.y = 
        }
        else if (movement.y < 0)
        {

        }

        controller.Move(Speed * Time.deltaTime * transform.forward);
    }

    public void OnWalk(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        sprint = context.action.IsPressed();
    }
    public void OnSneak(InputAction.CallbackContext context)
    {
        sneak = context.action.IsPressed();
    }
}