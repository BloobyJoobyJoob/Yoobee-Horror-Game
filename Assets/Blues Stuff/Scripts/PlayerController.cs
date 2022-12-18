using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    #region Vars

    public static PlayerController ClientManager = null;
    public static PlayerController TeammateManager = null;

    [Header("Movement")]

    public float SprintSpeed;
    public float SneakSpeed;
    public float WalkSpeed;

    public float BackwardsSpeedMultiplier = 0.5f;
    public float SidewaysSpeedMultiplier = 0.8f;

    [Header("Head Bobbing")]

    public CinemachineVirtualCamera Camera;

    public AnimationCurve BobHeight;
    public AnimationCurve BobSpeed;
    public AnimationCurve XBobAmount;
    public AnimationCurve YBobAmount;
    public AnimationCurve DutchBobAmount;
    public AnimationCurve PanBobAmount;
    public AnimationCurve TiltBobAmount;

    public bool TiltBobOnSecondFootstep = false;

    public float BobBlendTime = 0.2f;

    private CharacterController controller;

    float speed = 0;
    float oldSpeed = 0;
    float smoothSpeed = 0;
    float bobValue = 0;

    Vector2 inputMovement;
    Vector3 gravityMovement;
    Vector3 walkingMovement;
    bool sprint;
    bool sneak;

    CinemachinePOV cameraPOV;
    CinemachineRecomposer cameraRecomposer;
    CinemachineCameraOffset cameraOffset;

    #endregion

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        cameraPOV = Camera.GetCinemachineComponent<CinemachinePOV>();
        cameraRecomposer = Camera.GetComponent<CinemachineRecomposer>();
        cameraOffset = Camera.GetComponent<CinemachineCameraOffset>();
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
        BobHead();
    }
    void Move()
    {
        Vector2 walkSpeed;

        float maxSpeed;

        maxSpeed = sneak ? SneakSpeed : sprint ? SprintSpeed : WalkSpeed;

        walkSpeed.y = inputMovement.y < 0 ? inputMovement.y * maxSpeed * BackwardsSpeedMultiplier : maxSpeed * inputMovement.y;
        walkSpeed.x = inputMovement.x * maxSpeed * SidewaysSpeedMultiplier;

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

        speed = walkingMovement.magnitude;

        if (speed != oldSpeed)
        {
            DOTween.To(() => smoothSpeed, x => smoothSpeed = x, speed, BobBlendTime);
        }

        oldSpeed = speed;
    }

    void BobHead()
    {
        if (smoothSpeed == 0) return;
        
        bobValue += BobSpeed.Evaluate(smoothSpeed / SprintSpeed) * Time.deltaTime;

        if (bobValue > 360)
        {
            bobValue -= 360;
        }

        float bobValueSin = Mathf.Sin(bobValue);
        float bobValueSinSlow = Mathf.Sin(bobValue * 0.5f);

        cameraOffset.m_Offset = new Vector3(BobSlow(XBobAmount), Bob(YBobAmount) + BobHeight.Evaluate(smoothSpeed / SprintSpeed), 0);

        cameraRecomposer.m_Dutch = BobSlow(DutchBobAmount);
        cameraRecomposer.m_Pan = BobSlow(PanBobAmount);

        cameraRecomposer.m_Tilt = TiltBobOnSecondFootstep ? BobSlow(TiltBobAmount) : Bob(TiltBobAmount);

        float BobSlow(AnimationCurve amp)
        {
            return bobValueSinSlow * amp.Evaluate(smoothSpeed / SprintSpeed);
        }
        float Bob(AnimationCurve amp)
        {
            return bobValueSin * amp.Evaluate(smoothSpeed / SprintSpeed);
        }
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