using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [SerializeField]
    private CameraPoint[] cameraPoints;

    [SerializeField]
    private float moveSpeed = 3f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (cameraPoints.Length > 0)
            targetPosition = cameraPoints[0].transform.position;
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    public void MoveToRoom(int roomIndex)
    {
        foreach (CameraPoint point in cameraPoints)
        {
            if (point.RoomIndex == roomIndex)
            {
                targetPosition = point.transform.position;
                isMoving = true;
                return;
            }
        }

        Debug.LogWarning($"No se encontró CameraPoint con roomIndex {roomIndex}");
    }
}
