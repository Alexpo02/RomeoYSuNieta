using System;
using UnityEngine;

public class QuestCollectable : MonoBehaviour
{
    public string objetiveID; // Ej: "planta_roja", "piedra_mana"
    public QuestType objectiveType = QuestType.Collect;

    //[Header("Item asociado (opcional, para destruir si ya está en inventario)")]
    //public ItemData itemData; // ← AÑADE ESTO

    // Evento que QuestManager escucha
    public static event Action<QuestType, string> OnQuestItemCollected;

    public void NotifyCollected()
    {
        Debug.Log($"[QuestCollectable] Notificando: tipo={objectiveType}, id={objetiveID}");
        // Solo notifica cuando la misión está activa
        OnQuestItemCollected?.Invoke(objectiveType, objetiveID);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
