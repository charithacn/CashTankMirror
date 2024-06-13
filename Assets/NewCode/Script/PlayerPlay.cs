using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerPlay : NetworkBehaviour
{
    // Start is called before the first frame update
    public Transform playerTransform;
    Rigidbody2D rb;
    public Vector2 livePosition;
    public Quaternion liveRotation;
    bool isGrounded;
    public LayerMask groundLayer;
    public float groundCheckRadius;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //rb.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        print(horizontalInput + verticalInput);
        Vector2 moveDirection = new Vector2(horizontalInput, verticalInput).normalized;
        Vector2 newPosition = rb.position + moveDirection * 5f * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        isGrounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);
        if(!isGrounded)
        {
            if(livePosition!=null)
            {
                transform.position = livePosition;
                transform.rotation = liveRotation;
            }
            
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Draw ground check circle in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
    public override void OnStopClient()
    {
        Debug.Log("player OnNetworkDespawn");
        int v = ClientManager.Clients.Count;
        Debug.Log("Count " + v);
    }

    public override void OnStartClient()
    {
        Debug.Log("palyr OnNetworkSpawn");

        ServerConnectServerRpc();
        StartCoroutine(WaitToClient());
        TestServerRPC();
    }
    [ServerRpc]
    public void TestServerRPC(NetworkConnection conn = null)
    {
        var clientId = conn.ClientId;
        print("Client id : " + clientId);
    }
    [ServerRpc]
    private void ServerConnectServerRpc()
    {
        print("player Server rpc connected");

    }
    [ObserversRpc]
    private void ClientConnectClientRpc()
    {
        print("player Client Connected!");
    }
    IEnumerator WaitToClient()
    {
        yield return new WaitForSeconds(2f);
        ClientConnectClientRpc();
    }

    public float rotationSpeed = .1f;
    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Exit");
        //transform.position = livePosition;
        //transform.rotation = liveRotation;
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        livePosition = transform.position;
        liveRotation = transform.rotation;
        Vector2 direction = collision.contacts[0].point - (Vector2)transform.position;
        float angle = Vector2.Angle(direction, transform.right);
        Debug.Log("Collision enter 2d");
        if (angle != 90f)
        {
            // Calculate the signed angle to determine the direction of rotation
            Quaternion targetRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90);

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            Debug.Log("Collision enter 2d using 90s");
            Vector2 collisionNormal = collision.contacts[0].normal;
            Vector2 forceDirection = Quaternion.Euler(0, 0, 180) * collisionNormal;

            // Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                //rb.AddForce(forceDirection.normalized * 1f, ForceMode2D.Impulse);
                //rb.AddForce(forceDirection * 10f, ForceMode2D.Impulse);
            }
        }

    }
}
