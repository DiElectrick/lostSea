using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RaftController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float acceleration = 5f;
    [SerializeField] float deceleration = 1.0f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float speedForMaxRotation = 5f;

    [Header("Rowing Sync")]
    [SerializeField] float rowCycleTime = 1.0f;
    float rowingTimer;

    [Header("Turning Settings")]
    [SerializeField] float turnAcceleration = 5f;    // насколько быстро набирается угловая скорость
    [SerializeField] float turnDeceleration = 2f;    // насколько быстро сбрасывается угловая скорость после отпускания
    [SerializeField] float maxTurnSpeed = 50f;       // максимальная угловая скорость (градусы/с)


    Rigidbody rb;
    Vector2 moveInput;
    InputAction moveAction;

    float cycleProgress;
    float currentTurnSpeed;        // текущая угловая скорость (для инерции)

    Vector3 decelerationForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>().normalized;

        rowlingCount();

    }

    private void FixedUpdate()
    {
        Vector3 moveDir = rb.linearVelocity.normalized;
        float angle = Vector3.SignedAngle(transform.forward, moveDir, transform.up);
        float sinAngle = Mathf.Sin(angle * Mathf.Deg2Rad);

        decelerationForce = -moveDir * (acceleration * ((rb.linearVelocity.magnitude / maxSpeed) + 0.5f * Mathf.Abs(sinAngle)));
        decelerationForce += Vector3.Cross(moveDir, Vector3.up).normalized * sinAngle * 0.9f;


        rb.AddForce(decelerationForce, ForceMode.Acceleration);


        if (Mathf.Abs(moveInput.y) > 0.01f)
        {
            Vector3 force = transform.forward * (moveInput.y * acceleration);
            rb.AddForce(force, ForceMode.Acceleration);

        }

   
        rb.AddTorque(0,- turnDeceleration * (rb.angularVelocity.y * Mathf.Rad2Deg / maxTurnSpeed),0,ForceMode.Acceleration);

        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            rb.AddTorque(0, (turnAcceleration * moveInput.x * Mathf.Min(1f,speedForMaxRotation/rb.linearVelocity.magnitude)), 0, ForceMode.Acceleration);
        }

        /*
        // Управление угловой скоростью (инерция поворота)
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            currentTurnSpeed = Mathf.MoveTowards(
                currentTurnSpeed,
                moveInput.x * maxTurnSpeed * Mathf.Min(1f, speedForMaxRotation / rb.linearVelocity.magnitude),
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
        */

    }

    void rowlingCount()
    {
        rowingTimer += Time.deltaTime;
        if (rowingTimer >= rowCycleTime)
            rowingTimer = 0f;

        cycleProgress = rowingTimer / rowCycleTime;
    }
    public float GetRowingProgress()
    {
        return cycleProgress;
    }

    private void OnDrawGizmosSelected()
    {
        // Рассчитываем конечную точку луча
        Vector3 endPosition = transform.position + new Vector3(moveInput.x, 0, moveInput.y) * 2;

        // Задаем цвет для следующих гизмо
        Gizmos.color = Color.cyan;

        // Рисуем луч от текущей позиции вперед
        Gizmos.DrawRay(transform.position, new Vector3(moveInput.x, 0, moveInput.y) * 2);
        Gizmos.DrawRay(transform.position, transform.forward * 3);

        // Рисуем сферу в конечной точке луча
        Gizmos.DrawWireSphere(endPosition, 0.1f);

        Gizmos.color = Color.red;
        // Gizmos.DrawRay(transform.position, rb.linearVelocity * 5f);
    }
}
