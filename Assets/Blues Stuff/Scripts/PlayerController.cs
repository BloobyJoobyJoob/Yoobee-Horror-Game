using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    #region Vars

    public static PlayerController ClientManager = null;
    public static PlayerController TeammateManager = null;

    public CinemachineVirtualCamera Camera;

    public float SprintSpeed;
    public float SneakSpeed;
    public float WalkSpeed;

    public float BackwardsSpeedMultiplier = 0.5f;
    public float SidewaysSpeedMultiplier = 0.8f;

    private CharacterController controller;

    float Speed;

    Vector2 inputMovement;
    Vector3 gravityMovement;
    Vector3 walkingMovement;
    bool sprint;
    bool sneak;

    CinemachinePOV cameraPOV;

    #endregion

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cameraPOV = Camera.GetCinemachineComponent<CinemachinePOV>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (ClientManager == null)
            {
                ClientManager = this;
            }
            else
            {
                Debug.LogError("Blue you smelly fart");
                return;
            }
        }
        else
        {
            if (TeammateManager == null)
            {
                TeammateManager = this;
            }
            else
            {
                Debug.LogError("Blue you smelly fart");
                return;
            }
        }
    }

    private void Update()
    {
        Move();
    }
    void Move()
    {
        Vector2 walkSpeed;

        Speed = sneak ? SneakSpeed : sprint ? SprintSpeed : WalkSpeed;

        walkSpeed.y = inputMovement.y < 0 ? inputMovement.y * Speed * BackwardsSpeedMultiplier : Speed * inputMovement.y;
        walkSpeed.x = inputMovement.x * Speed;

        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + cameraPOV.m_HorizontalAxis.Value, transform.rotation.z);

        walkingMovement = (transform.forward * walkSpeed.y) + (transform.right * walkSpeed.x);

        if (controller.isGrounded)
        {
            gravityMovement = Vector3.zero;
        }
        else
        {
            gravityMovement += Physics.gravity * Time.deltaTime;
        }

        controller.Move((walkingMovement + gravityMovement) * Time.deltaTime);
    }

    #region GetInput
    public void OnWalk(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        sprint = context.action.IsPressed();
    }
    public void OnSneak(InputAction.CallbackContext context)
    {
        sneak = context.action.IsPressed();
    }
    #endregion
}