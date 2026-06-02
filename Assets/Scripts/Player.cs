using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    private float runSpeed = 10f;

    private bool isRunning;

    private Rigidbody rb;
    private Vector2 movement;

    private PlayerControllerState currentState = PlayerControllerState.Iddle;

    private enum PlayerControllerState
    {
        Iddle,
        Moving,
        Running,
    }

    private void RunningState()
    {
        if (movement == Vector2.zero)
        {
            currentState = PlayerControllerState.Iddle;
            return;
        }

        if (!isRunning)
        {
            currentState = PlayerControllerState.Moving;
            return;
        }

        Move(runSpeed);
    }

    private void IddleState()
    {
        if (movement != Vector2.zero)
        {
            currentState = isRunning ? PlayerControllerState.Running : PlayerControllerState.Moving;
        }

        Move(0);
    }

    private void MovingState()
    {
        if (movement == Vector2.zero)
        {
            currentState = PlayerControllerState.Iddle;
            return;
        }

        if (isRunning)
        {
            currentState = PlayerControllerState.Running;
            return;
        }

        Move(speed);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
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
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case PlayerControllerState.Iddle:
                IddleState();
                break;
            case PlayerControllerState.Moving:
                MovingState();
                break;
            case PlayerControllerState.Running:
                RunningState();
                break;
        }
    }
}