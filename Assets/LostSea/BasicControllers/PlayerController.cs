using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 10f;

    [SerializeField] private float _interactionRange = 3f;
    [SerializeField] private KeyCode _interactionKey = KeyCode.E;
    [SerializeField] private Camera _playerCamera; // Ссылка на камеру игрока

    private IInteractable _currentInteractable;

    private Vector3 movement;
    private Rigidbody rb;
    private Animator animator; // опционально

    InputAction moveAction;

    [SerializeField] public Transform HandTransform;

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
        FindInteractableTarget();
        HandleInteractionInput();
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

    // Поиск объекта перед игроком с помощью Raycast
    private void FindInteractableTarget()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        IInteractable newInteractable = null;

        if (Physics.Raycast(ray, out hit, _interactionRange))
        {
            // Пытаемся получить компонент, реализующий IInteractable, у объекта, в который попал луч
            newInteractable = hit.collider.GetComponent<IInteractable>();
        }

        // Если цель поменялась
        if (_currentInteractable != newInteractable)
        {
            // Убираем подсветку со старой цели
            if (_currentInteractable != null)
            {
               // _currentInteractable.OnEndHover();
            }

            // Устанавливаем новую цель
            _currentInteractable = newInteractable;

            // Подсвечиваем новую цель
            if (_currentInteractable != null)
            {
               // _currentInteractable.OnStartHover();
                // Здесь можно, например, вывести UI-подсказку: _currentInteractable.GetInteractionText()
            }
        }
    }

    // Обработка нажатия клавиши
    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(_interactionKey) && _currentInteractable != null)
        {
            // Вызываем метод Interact у текущего объекта, передавая ссылку на самого игрока
            _currentInteractable.Interact(gameObject);
        }
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