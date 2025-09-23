using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Vector3 movement;
    private Rigidbody rb;
    private Animator animator; // �����������

    InputAction moveAction;

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