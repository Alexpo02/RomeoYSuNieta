using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    [Range(0, 2)]
    private float walkAnimSpeed = 0.3f; // ajusta hasta que encaje

    [SerializeField]
    [Range(0, 2)]
    private float carryAnimSpeed = 0.3f;
    private Rigidbody rb;
    private Vector2 movement;
    private Animator animator;
    private PlayerInteractor interactor;
    private PlayerControllerState currentState = PlayerControllerState.Iddle;

    private enum PlayerControllerState
    {
        Iddle,
        Moving,
        IddlePickingObjects,
        MovingPickingObjects,
    }

    private void IddlePickingObjectsState()
    {
        animator.speed = 1f;
        bool isCarrying = interactor != null && interactor.HeldItem != null;
        if (!isCarrying)
        {
            currentState = PlayerControllerState.Iddle;
            return;
        }
        if (movement != Vector2.zero)
        {
            currentState = PlayerControllerState.MovingPickingObjects;
            return;
        }
        Move(0);
    }

    private void MovingPickingObjectsState()
    {
        bool isCarrying = interactor != null && interactor.HeldItem != null;
        if (!isCarrying)
        {
            currentState = PlayerControllerState.Moving;
            return;
        }
        if (movement == Vector2.zero)
        {
            currentState = PlayerControllerState.IddlePickingObjects;
            return;
        }
        Move(speed * 0.5f);
        animator.speed = (speed * 0.5f) * carryAnimSpeed * movement.magnitude; // 👈
    }

    private void IddleState()
    {
        animator.speed = 1f;
        bool isCarrying = interactor != null && interactor.HeldItem != null;
        if (movement != Vector2.zero)
        {
            currentState = isCarrying
                ? PlayerControllerState.MovingPickingObjects
                : PlayerControllerState.Moving;
            return;
        }
        Move(0);
    }

    private void MovingState()
    {
        bool isCarrying = interactor != null && interactor.HeldItem != null;
        if (isCarrying)
        {
            currentState = PlayerControllerState.MovingPickingObjects;
            return;
        }
        if (movement == Vector2.zero)
        {
            currentState = PlayerControllerState.Iddle;
            return;
        }
        Move(speed);
        animator.speed = speed * walkAnimSpeed * movement.magnitude;
    }

    private void Move(float currentSpeed)
    {
        Vector3 velocity = new Vector3(
            movement.x * currentSpeed,
            rb.linearVelocity.y,
            movement.y * currentSpeed
        );
        rb.linearVelocity = velocity;
        Vector3 direction = new Vector3(movement.x, 0f, movement.y);
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        interactor = GetComponent<PlayerInteractor>();
    }

    private void FixedUpdate()
    {
        bool isCarrying = interactor != null && interactor.HeldItem != null;
        animator.SetBool("IsCarrying", isCarrying);
        animator.SetBool("IsWalking", movement != Vector2.zero);
        switch (currentState)
        {
            case PlayerControllerState.Iddle:
                IddleState();
                break;
            case PlayerControllerState.Moving:
                MovingState();
                break;
            case PlayerControllerState.IddlePickingObjects:
                IddlePickingObjectsState();
                break;
            case PlayerControllerState.MovingPickingObjects:
                MovingPickingObjectsState();
                break;
        }
    }
}
