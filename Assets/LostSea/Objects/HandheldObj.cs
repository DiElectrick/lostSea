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
            // ����� ����
            PickUp(player);
        }
        else
        {
            // ������ ����
            Drop();
        }
        _isHeld = !_isHeld;
    }

    private void PickUp(GameObject player) // �� ������� ������ ������ ���� ������ GameObject-����� "����"
    {
        // ��������� ������, ����� ���� �� ����� � �� ������ ������
        _rb.isKinematic = false;
        // ������ ���� �������� �� ��������� � "����" ������
        transform.SetParent(player.GetComponent<PlayerController>().HandTransform);
        // �������� ���� � ����� "����"
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = player.GetComponent<Rigidbody>();
        joint.enableCollision = true;


    }


    private void Drop()
    {
        // �������� ������ �������
        _rb.isKinematic = false;
        FixedJoint joint = GetComponent<FixedJoint>();
        if (joint != null) Destroy(joint);
        // ���������� ���� � �������� ��������
        transform.SetParent(_originalParent);
    }

}
