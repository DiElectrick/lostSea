using UnityEngine;

public class DialogueObj : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        Debug.Log("Интеракция");
    }

}
