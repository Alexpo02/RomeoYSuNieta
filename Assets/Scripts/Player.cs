using System;
using System.Collections;
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
    private float walkAnimSpeed = 0.3f;

    [SerializeField]
    [Range(0, 2)]
    private float carryAnimSpeed = 0.3f;

    [SerializeField]
    private string interactAnimParam = "IsInteracting";

    private Rigidbody rb;
    private Vector2 movement;
    private Animator animator;
    private PlayerInteractor interactor;
    private PlayerControllerState currentState = PlayerControllerState.Iddle;
    private bool isBlocked = false;

    private enum PlayerControllerState
    {
        Iddle,
        Moving,
        IddlePickingObjects,
        MovingPickingObjects,
    }

    public bool CanPlayInteractAnimation =>
        currentState == PlayerControllerState.Iddle || currentState == PlayerControllerState.Moving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        interactor = GetComponent<PlayerInteractor>();
    }

    private void FixedUpdate()
    {
        if (isBlocked)
            return;

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
        animator.speed = (speed * 0.5f) * carryAnimSpeed * movement.magnitude;
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
        if (isBlocked)
        {
            movement = Vector2.zero;
            return;
        }
        movement = context.ReadValue<Vector2>();
    }

    public void SetMovementBlocked(bool blocked)
    {
        isBlocked = blocked;
        if (blocked)
        {
            movement = Vector2.zero;
            rb.linearVelocity = Vector3.zero;
            currentState = PlayerControllerState.Iddle;
            animator.SetFloat("Speed", 0f);
            animator.speed = 1f;
        }
    }

    public void PlayInteractAnimation(Action onComplete = null)
    {
        StartCoroutine(InteractRoutine(onComplete));
    }

    private IEnumerator InteractRoutine(Action onComplete)
    {
        SetMovementBlocked(true);

        animator.ResetTrigger(interactAnimParam);
        animator.SetTrigger(interactAnimParam);

        yield return null;

        // Esperar a que empiece la transición
        int timeout = 60;
        while (timeout > 0)
        {
            if (
                animator.IsInTransition(0)
                || animator.GetCurrentAnimatorStateInfo(0).IsName("interact")
            )
                break;
            timeout--;
            yield return null;
        }

        // Esperar a que termine la transición y entre en "interact"
        timeout = 60;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("interact") && timeout > 0)
        {
            timeout--;
            yield return null;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("interact"))
        {
            Debug.LogWarning(
                "[InteractRoutine] No entró en 'interact'. Revisa el nombre del estado en el Animator."
            );
            SetMovementBlocked(false);
            onComplete?.Invoke();
            yield break;
        }

        float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;

        // Al 50% de la animación ejecuta la acción (ajusta según tu clip)
        yield return new WaitForSeconds(clipLength * 0.5f);
        onComplete?.Invoke();

        yield return new WaitForSeconds(clipLength * 0.5f);
        SetMovementBlocked(false);
    }
}
