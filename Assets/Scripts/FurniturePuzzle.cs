using UnityEngine;

public class FurniturePuzzle : MonoBehaviour
{
    [Header("Zonas correctas (solo las 3 con mueble asignado)")]
    [SerializeField]
    private DropZone[] correctZones;

    [Header("Quest (opcional)")]
    [SerializeField]
    private QuestCollectable questCollectable;

    [Header("Recompensa")]
    [SerializeField]
    private GameObject key;

    private bool solved = false;

    // DropZone llama a este método desde OnItemPlaced
    public void CheckPuzzle()
    {
        if (solved)
            return;

        foreach (var zone in correctZones)
        {
            // Zona vacía → todavía no
            if (!zone.IsOccupied)
                return;

            // Objeto incorrecto → todavía no
            // (DropZone ya filtra por acceptedItemName, pero doble comprobación)
            if (zone.PlacedItem == null)
                return;
        }

        // ¡Todo correcto! Bloquear todas las zonas
        solved = true;
        foreach (var zone in correctZones)
        {
            zone.Lock();
            zone.PlacedItem?.Lock();
        }

        Debug.Log("[FurniturePuzzle] ¡Puzzle resuelto!");
        key?.SetActive(true);
        questCollectable?.NotifyCollected();
    }
}
