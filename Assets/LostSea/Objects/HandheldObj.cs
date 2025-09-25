using UnityEngine;

public class HandheldObj : MonoBehaviour, IInteractable
{

    private bool _isHeld = false;
    private Transform _originalParent;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _originalParent = transform.parent;
    }
    public void Interact(GameObject player)
    {
        if (!_isHeld)
        {
            // Берем ящик
            PickUp(player);
        }
        else
        {
            // Кладем ящик
            Drop();
        }
        _isHeld = !_isHeld;
    }

    private void PickUp(GameObject player) // На объекте игрока должна быть пустая GameObject-метка "рука"
    {
        // Выключаем физику, чтобы ящик не падал и не толкал игрока
        _rb.isKinematic = false;
        // Делаем ящик дочерним по отношению к "руке" игрока
        transform.SetParent(player.GetComponent<PlayerController>().HandTransform);
        // Помещаем ящик в центр "руки"
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = player.GetComponent<Rigidbody>();
        joint.enableCollision = true;


    }


    private void Drop()
    {
        // Включаем физику обратно
        _rb.isKinematic = false;
        FixedJoint joint = GetComponent<FixedJoint>();
        if (joint != null) Destroy(joint);
        // Возвращаем ящик в исходную иерархию
        transform.SetParent(_originalParent);
    }

}
