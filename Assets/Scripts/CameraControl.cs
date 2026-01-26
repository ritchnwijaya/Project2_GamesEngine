using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] float offsetZ = 8f;
    [SerializeField] float smoothing = 12f;   // höher = schneller folgt
    Transform playerPosition;

    void Start()
    {
        playerPosition = FindAnyObjectByType<PlayerMovement>().transform;
    }

    // LateUpdate -> Kamera folgt nachdem der Player sich bewegt hat
    void LateUpdate()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        Vector3 targetPosition = new Vector3(
            playerPosition.position.x,
            transform.position.y,
            playerPosition.position.z - offsetZ
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothing * Time.deltaTime
        );
    }
}
