using UnityEngine;

public class FurniturePuzzle : MonoBehaviour
{
    [Header("Zonas correctas (solo las 3 con mueble asignado)")]
    [SerializeField]
    private DropZone[] correctZones;

    [Header("Recompensa")]
    [SerializeField]
    private GameObject key;

    [Header("Cajón a abrir al resolver")]
    [SerializeField]
    private Drawer drawer; // ← nuevo

    [Header("Quest (opcional)")]
    [SerializeField]
    private QuestCollectable questCollectable;

    private bool solved = false;

    public void CheckPuzzle()
    {
        if (solved)
            return;

        foreach (var zone in correctZones)
        {
            if (!zone.IsOccupied || zone.PlacedItem == null)
                return;
        }

        solved = true;
        foreach (var zone in correctZones)
        {
            zone.Lock();
            zone.PlacedItem?.Lock();
        }

        Debug.Log("[FurniturePuzzle] ¡Puzzle resuelto!");
        key?.SetActive(true);
        drawer?.Unlock(); // ← nuevo
        questCollectable?.NotifyCollected();
    }
}
