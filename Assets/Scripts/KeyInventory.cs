using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton que registra qué llaves ha recogido el jugador.
/// </summary>
public class KeyInventory : MonoBehaviour
{
    public static KeyInventory Instance { get; private set; }

    private readonly HashSet<string> collectedKeys = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddKey(string keyId)
    {
        collectedKeys.Add(keyId);
        Debug.Log($"[KeyInventory] Llave recogida: {keyId}");
    }

    public bool HasKey(string keyId) => collectedKeys.Contains(keyId);
}
