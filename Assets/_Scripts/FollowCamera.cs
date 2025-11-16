using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class FollowCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject followTarget;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private bool cameraLag;
    [SerializeField] private float followSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, transform.position.z) + offset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (cameraLag)
        {
            Vector3 targetPosition = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, transform.position.z) + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, transform.position.z) + offset;
        }
    }

    // Teleports the camera instantly to the target position
    public void TeleportToTarget()
    {
        transform.position = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, transform.position.z) + offset;
    }
}
