using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PScript : MonoBehaviour
{
    [SerializeField]
    private InputActionReference playerMovementAction;
    public float speed = 10f;
    public float rotationSpeed = 100f;
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        Vector2 rightStickValue = playerMovementAction.action.ReadValue<Vector2>();
        rb.AddForce(transform.up * rightStickValue.y * speed);

        // Rotate the vehicle
        rb.angularVelocity = -rightStickValue.x * rotationSpeed;
    }
}
