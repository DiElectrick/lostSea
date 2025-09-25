using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 10f;

    [SerializeField] private float _interactionRange = 3f;
    [SerializeField] private KeyCode _interactionKey = KeyCode.E;
    [SerializeField] private Camera _playerCamera; // ������ �� ������ ������

    private IInteractable _currentInteractable;

    private Vector3 movement;
    private Rigidbody rb;
    private Animator animator; // �����������

    InputAction moveAction;

    [SerializeField] public Transform HandTransform;

    void Start()
    {
        // �������� ��������� Rigidbody
        rb = GetComponent<Rigidbody>();

        // �����������: �������� �������� ���� ����
        animator = GetComponent<Animator>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        FindInteractableTarget();
        HandleInteractionInput();
        // ������� ������ �������� � ����������� ���
        movement = new Vector3(moveAction.ReadValue<Vector2>().x, 0f, moveAction.ReadValue<Vector2>().y).normalized;

        movement = Quaternion.AngleAxis(45, Vector3.up) * movement;

        // �����������: ���������� ����������
        if (animator != null)
        {
            bool isMoving = movement.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);
        }
    }

    void FixedUpdate()
    {
        // ��������� �������� � FixedUpdate ��� ������
        MoveCharacter();
        RotateCharacter();
    }

    // ����� ������� ����� ������� � ������� Raycast
    private void FindInteractableTarget()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        IInteractable newInteractable = null;

        if (Physics.Raycast(ray, out hit, _interactionRange))
        {
            // �������� �������� ���������, ����������� IInteractable, � �������, � ������� ����� ���
            newInteractable = hit.collider.GetComponent<IInteractable>();
        }

        // ���� ���� ����������
        if (_currentInteractable != newInteractable)
        {
            // ������� ��������� �� ������ ����
            if (_currentInteractable != null)
            {
               // _currentInteractable.OnEndHover();
            }

            // ������������� ����� ����
            _currentInteractable = newInteractable;

            // ������������ ����� ����
            if (_currentInteractable != null)
            {
               // _currentInteractable.OnStartHover();
                // ����� �����, ��������, ������� UI-���������: _currentInteractable.GetInteractionText()
            }
        }
    }

    // ��������� ������� �������
    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(_interactionKey) && _currentInteractable != null)
        {
            // �������� ����� Interact � �������� �������, ��������� ������ �� ������ ������
            _currentInteractable.Interact(gameObject);
        }
    }


void MoveCharacter()
    {
        if (movement.magnitude > 0.1f)
        {
            // ���������� ���������
            Vector3 moveVelocity = movement * moveSpeed;
            rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
            
        }
        else
        {
            // ������������� �������������� ��������
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    void RotateCharacter()
    {
        if (movement.magnitude > 0.1f)
        {
            // ������ ������������ �������� � ������� ��������
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    // �����������: ������������ ������� �������� � ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, movement * 2f);
    }
}