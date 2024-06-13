using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyreController : MonoBehaviour
{
    public bool tyre_ground;
    public float groundCheckRadius;
    public LayerMask groundLayer;

    public float rotateGroundRadius;
    public bool rotate_tyre_ground;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tyre_ground = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);

        rotate_tyre_ground = Physics2D.OverlapCircle(transform.position, rotateGroundRadius, groundLayer);
    }
    private void OnDrawGizmosSelected()
    {
        // Draw ground check circle in editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rotateGroundRadius);
    }
}
