using UnityEngine;

public class IsometricCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Цель, за которой следует камера (персонаж)

    [Header("Camera Settings")]
    public float distance = 10f; // Дистанция от камеры до цели
    public float height = 7f; // Высота камеры над целью
    public float rotationAngle = 45f; // Угол поворота камеры по оси Y
    public float tiltAngle = 30f; // Угол наклона камеры вниз

    [Header("Smoothing Settings")]
    public float smoothSpeed = 5f; // Скорость плавного следования
    public bool smoothFollow = true; // Включить плавное следование

    [Header("Input Settings")]
    public bool enableZoom = true;
    public float zoomSpeed = 2f;
    public float minDistance = 5f;
    public float maxDistance = 20f;

    private Vector3 cameraOffset;

    void Start()
    {
        // Проверяем наличие цели
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

        // Обработка зума
        if (enableZoom)
        {
            HandleZoom();
        }

        // Обновление позиции камеры
        UpdateCameraPosition();
    }

    void CalculateCameraOffset()
    {
        // Создаем смещение камеры на основе углов и дистанции
        Quaternion rotation = Quaternion.Euler(tiltAngle, rotationAngle, 0f);
        cameraOffset = rotation * Vector3.back * distance;
        cameraOffset.y += height;
    }

    void UpdateCameraPosition()
    {
        // Целевая позиция камеры
        Vector3 targetPosition = target.position + cameraOffset;

        if (smoothFollow)
        {
            // Плавное перемещение камеры
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            // Мгновенное перемещение
            transform.position = targetPosition;
        }

        // Камера всегда смотрит на цель
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

    // Методы для изменения настроек во время выполнения
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

    // Визуализация в редакторе (только для отладки)
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