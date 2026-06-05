using UnityEngine;

/// <summary>
/// ScriptableObject que actúa como flag booleano compartido entre objetos de una sala.
/// Crea un asset por sala: RoomCondition_1, RoomCondition_2, RoomCondition_3.
/// </summary>
[CreateAssetMenu(fileName = "RoomCondition", menuName = "Rooms/Room Condition")]
public class RoomCondition : ScriptableObject
{
    private bool isCompleted;

    public bool IsCompleted => isCompleted;

    public void Complete()
    {
        isCompleted = true;
        Debug.Log($"[RoomCondition] '{name}' completada.");
    }

    public void Reset()
    {
        isCompleted = false;
    }

    // En el Editor, los ScriptableObjects retienen su valor al salir del Play Mode.
    // OnEnable se llama al entrar en Play Mode, reseteando el flag cada vez.
    private void OnEnable()
    {
        isCompleted = false;
    }
}
