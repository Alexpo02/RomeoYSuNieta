using UnityEngine;

public class Book : MonoBehaviour, IInteractuable
{
    public void GetInteractionText()
    {
        //throw new System.NotImplementedException();
    }

    public void HideText()
    {
        //throw new System.NotImplementedException();
    }

    public void Interact()
    {
        Debug.Log("Objeto destruido");
        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
