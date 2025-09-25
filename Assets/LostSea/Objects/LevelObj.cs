using UnityEngine;

public class LevelObj : MonoBehaviour, IInteractable
{
    bool _enabled = false;
    public void Interact(GameObject player)
    {
        _enabled = true;
    }

    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    
}
