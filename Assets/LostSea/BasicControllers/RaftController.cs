using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RaftController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float acceleration = 5f;
    [SerializeField] float turnSpeed = 50f;
    [SerializeField] float waterDrag = 0.99f;
    [SerializeField] float speedForMaxRotation = 2f;

    [Header("Rowing Sync")]
    [SerializeField] float rowCycleTime = 1.0f;
    float rowingTimer;

    [Header("Turning Settings")]
    [SerializeField] float turnAcceleration = 5f;    // насколько быстро набирается угловая скорость
    [SerializeField] float turnDeceleration = 2f;    // насколько быстро сбрасывается угловая скорость после отпускания
    [SerializeField] float maxTurnSpeed = 50f;       // максимальная угловая скорость (градусы/с)
    [SerializeField] float directionAlignSpeed = 2f;


    Rigidbody rb;
    Vector2 moveInput;
    InputAction moveAction;
    
    float cycleProgress;
    float currentTurnSpeed;        // текущая угловая скорость (для инерции)
    
    Vector3 moveDirection;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>().normalized;


        rowingTimer += Time.deltaTime;
        if (rowingTimer >= rowCycleTime)
            rowingTimer = 0f;

        cycleProgress = rowingTimer / rowCycleTime;



        // плавное доворачивание направления движения
        moveDirection = Vector3.Slerp(
            moveDirection,
            transform.forward,
            Time.deltaTime * directionAlignSpeed
        );
    }

    private void FixedUpdate()
    {

        if (Mathf.Abs(moveInput.y) > 0.01f)
        {
            Vector3 force = moveDirection * (moveInput.y * acceleration);
            rb.AddForce(force, ForceMode.Acceleration);
        }

        // Управление угловой скоростью (инерция поворота)
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            currentTurnSpeed = Mathf.MoveTowards(
                currentTurnSpeed,
                moveInput.x * maxTurnSpeed * Mathf.Min(1f,speedForMaxRotation/rb.linearVelocity.magnitude),
                turnAcceleration * Time.fixedDeltaTime * maxTurnSpeed
            );
        }
        else
        {
            currentTurnSpeed = Mathf.MoveTowards(
                currentTurnSpeed,
                0f,
                turnDeceleration * Time.fixedDeltaTime * maxTurnSpeed
            );
        }

        // применяем вращение
        if (Mathf.Abs(currentTurnSpeed) > 0.01f)
        {
            float turn = currentTurnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }


        rb.linearVelocity *= waterDrag; 
    }

    public float GetRowingProgress()
    {
        return cycleProgress;
    }
}
