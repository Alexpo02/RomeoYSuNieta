using UnityEngine;

public class Key : MonoBehaviour, IInteractuable
{
    QuestCollectable questCollectable;

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
        questCollectable.NotifyCollected();
        Destroy(gameObject);
    }

    void Awake()
    {
        questCollectable = GetComponent<QuestCollectable>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
