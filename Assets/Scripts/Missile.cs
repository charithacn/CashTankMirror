using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public GameObject missile;
    public Rigidbody2D rb;
    public Explosions explosionManager;

    public float Lifetime;
    public GameObject Parent;
    public PlayerHandler playerHandler;

    private void Awake()
    {
        explosionManager = GameObject.Find("ExplosionManager").GetComponent<Explosions>();
        playerHandler = GameObject.Find("PlayerHandler").GetComponent<PlayerHandler>();
    }

    void Update()
    {
        transform.up = rb.velocity.normalized;
        rb.AddForce(explosionManager.BlastFling(transform.position) * 2);
        Lifetime += Time.deltaTime;

        if (transform.position.y < -5)
        {
            explosionManager.Explode(new Explosion(transform.position, 4, Parent, playerHandler));
            Destroy(missile);
        }

        if (Parent == null)
            Destroy(gameObject);
    }

    public void Impact(Vector3 v)
    {
        rb.AddForce(v, ForceMode2D.Force);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Parent && Lifetime < 0.1f)
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
        }
        else if (collision.collider.tag != "Bouncy")
        {
            explosionManager.Explode(new Explosion(transform.position, 4, Parent, playerHandler));
            Destroy(missile);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<BoxCollider2D>().tag == "Lava")
        {
            explosionManager.Explode(new Explosion(transform.position, 4, Parent, playerHandler));
            Destroy(missile);
        }

    }
}