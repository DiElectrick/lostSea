using UnityEngine;

public class IsometricCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // ����, �� ������� ������� ������ (��������)

    [Header("Camera Settings")]
    public float distance = 10f; // ��������� �� ������ �� ����
    public float height = 7f; // ������ ������ ��� �����
    public float rotationAngle = 45f; // ���� �������� ������ �� ��� Y
    public float tiltAngle = 30f; // ���� ������� ������ ����

    [Header("Smoothing Settings")]
    public float smoothSpeed = 5f; // �������� �������� ����������
    public bool smoothFollow = true; // �������� ������� ����������

    [Header("Input Settings")]
    public bool enableZoom = true;
    public float zoomSpeed = 2f;
    public float minDistance = 5f;
    public float maxDistance = 20f;

    private Vector3 cameraOffset;

    void Start()
    {
        // ��������� ������� ����
        if (target == null)
        {
            Debug.LogWarning("IsometricCameraController: Target not assigned! Looking for Player tag.");
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        CalculateCameraOffset();
        ApplyCameraPosition();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ��������� ����
        if (enableZoom)
        {
            HandleZoom();
        }

        // ���������� ������� ������
        UpdateCameraPosition();
    }

    void CalculateCameraOffset()
    {
        // ������� �������� ������ �� ������ ����� � ���������
        Quaternion rotation = Quaternion.Euler(tiltAngle, rotationAngle, 0f);
        cameraOffset = rotation * Vector3.back * distance;
        cameraOffset.y += height;
    }

    void UpdateCameraPosition()
    {
        // ������� ������� ������
        Vector3 targetPosition = target.position + cameraOffset;

        if (smoothFollow)
        {
            // ������� ����������� ������
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            // ���������� �����������
            transform.position = targetPosition;
        }

        // ������ ������ ������� �� ����
        transform.LookAt(target.position);
    }

    void HandleZoom()
    {
        float scroll = 0;
        if (scroll != 0)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            CalculateCameraOffset();
        }
    }

    void ApplyCameraPosition()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + cameraOffset;
            transform.position = targetPosition;
            transform.LookAt(target.position);
        }
    }

    // ������ ��� ��������� �������� �� ����� ����������
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetCameraAngles(float newRotationAngle, float newTiltAngle)
    {
        rotationAngle = newRotationAngle;
        tiltAngle = newTiltAngle;
        CalculateCameraOffset();
    }

    public void SetDistance(float newDistance)
    {
        distance = Mathf.Clamp(newDistance, minDistance, maxDistance);
        CalculateCameraOffset();
    }

    // ������������ � ��������� (������ ��� �������)
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawWireSphere(target.position, 0.5f);
        }
    }
}