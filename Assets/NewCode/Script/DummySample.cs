using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySample : MonoBehaviour
{
    private Rigidbody2D rb;
    Vector2 collisionNormal;
    Collision2D collision;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public float forceMagnitude = 10f; // Adjust this value as needed

    private void OnCollisionEnter2D(Collision2D collisions)
    {
        // Check if the collision involves the circle
        if (collision.collider.CompareTag("Ground"))
        {
            collision = collisions;
            // Print the touch position
            Vector2 touchPosition = collision.GetContact(0).point;
            Debug.Log("Touch Position: " + touchPosition);

            // Calculate the angle of collision
            collisionNormal = collision.GetContact(0).normal;
            float angle = Vector2.Angle(Vector2.up, collisionNormal);

            // Check the direction of the collision to get the correct angle
            if (Vector3.Cross(Vector3.forward, collisionNormal).z > 0)
            {
                angle = 360f - angle;
            }

            Debug.Log("Angle of Collision: " + angle);

            // Apply force to the ground object
            
        }
    }
    void FixedUpdate()
    {
        Rigidbody2D groundRigidbody = collision.collider.GetComponent<Rigidbody2D>();
        if (groundRigidbody != null)
        {
            Vector2 forceDirection = -collisionNormal.normalized; // Apply force opposite to collision normal
                                                                  // groundRigidbody.AddForceAtPosition(forceDirection * forceMagnitude, touchPosition);
        }
    }
}
