using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    [SerializeField]
    private int targetRoomIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController.Instance.MoveToRoom(targetRoomIndex);
        }
    }
}
