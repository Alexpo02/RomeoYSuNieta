using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [SerializeField]
    private CameraPoint[] cameraPoints;

    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float rotateSpeed = 3f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
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
        {
            targetPosition = cameraPoints[0].transform.position;
            targetRotation = cameraPoints[0].transform.rotation;
            transform.rotation = targetRotation;
        }
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

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotateSpeed * 60f * Time.deltaTime // grados por segundo
            );

            bool posReached = Vector3.Distance(transform.position, targetPosition) < 0.01f;
            bool rotReached = Quaternion.Angle(transform.rotation, targetRotation) < 0.1f;

            if (posReached && rotReached)
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
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
                targetRotation = point.transform.rotation;
                isMoving = true;
                return;
            }
        }

        Debug.LogWarning($"No se encontró CameraPoint con roomIndex {roomIndex}");
    }
}
