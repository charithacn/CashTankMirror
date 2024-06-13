using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // The thing the camera is following
    public Transform target;
    public Camera camera_;

    void Start()
    {
        transform.position = target.position;
    }

    void Update()
    {
        // Glide camera over to target
        transform.position = new Vector3(transform.position.x + ((target.position.x - transform.position.x) / 2),
            transform.position.y + ((target.position.y - transform.position.y) / 2), -10);

        camera_.orthographicSize = Mathf.Max(7, (transform.position.y / 5) + 1.5f);

        // Camera constrains
        if (transform.position.y < -1)
            transform.position = new Vector3(transform.position.x, -1, transform.position.z);
    }
}
