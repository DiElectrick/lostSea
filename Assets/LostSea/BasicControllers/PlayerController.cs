using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Vector3 movement;
    private Rigidbody rb;
    private Animator animator; // опционально

    InputAction moveAction;

    void Start()
    {
        // Получаем компонент Rigidbody
        rb = GetComponent<Rigidbody>();

        // Опционально: получаем аниматор если есть
        animator = GetComponent<Animator>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {

        // Создаем вектор движения и нормализуем его
        movement = new Vector3(moveAction.ReadValue<Vector2>().x, 0f, moveAction.ReadValue<Vector2>().y).normalized;

        movement = Quaternion.AngleAxis(45, Vector3.up) * movement;

        // Опционально: управление анимациями
        if (animator != null)
        {
            bool isMoving = movement.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);
        }
    }

    void FixedUpdate()
    {
        // Применяем движение в FixedUpdate для физики
        MoveCharacter();
        RotateCharacter();
    }

    void MoveCharacter()
    {
        if (movement.magnitude > 0.1f)
        {
            // Перемещаем персонажа
            Vector3 moveVelocity = movement * moveSpeed;
            rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
            
        }
        else
        {
            // Останавливаем горизонтальное движение
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    void RotateCharacter()
    {
        if (movement.magnitude > 0.1f)
        {
            // Плавно поворачиваем персонаж в сторону движения
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    // Опционально: визуализация вектора движения в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, movement * 2f);
    }
}