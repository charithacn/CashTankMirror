using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptBoy.Digable2DTerrain;

public class Explosions : MonoBehaviour
{
    List<Explosion> explosions = new List<Explosion>();
    public GameObject objectPrefab;
    // public BlowUpTiles tiles;
    public ScriptBoy.Digable2DTerrain.Shovel shovel;

    void Update()
    {
        for (int i = 0; i < explosions.Count; i++)
        {
            explosions[i].magnitude /= 1.05f;
            if (explosions[i].magnitude < 0.2f)
            {
                explosions.RemoveAt(i);
                i--;
            }
        }
    }

    public void Explode(Explosion boom)
    {
        explosions.Add(boom);
        Instantiate(objectPrefab, boom.location, objectPrefab.transform.rotation);
        // tiles.BlowUp(boom);
        shovel.transform.position = boom.location;
        shovel.radius = boom.magnitude / 3f;
        shovel.Dig();
    }

    public Vector2 BlastFling(Vector2 position)
    {
        Vector2 combinedVector = new Vector2();

        for (int i = 0; i < explosions.Count; i++)
        {
            Explosion e = explosions[i];
            if (Vector2.Distance(e.location, position) < e.magnitude - 1)
            {
                float force = -(e.magnitude - Mathf.Abs(Mathf.Abs(position.x) - Mathf.Abs(e.location.x)));
                combinedVector += new Vector2(e.location.x - position.x, e.location.y - position.y).normalized * force / 1.75f;
            }
        }

        return combinedVector;
    }

    public float BlastDamage(Vector2 position, GameObject parent)
    {
        float combinedForce = 0;

        for (int i = 0; i < explosions.Count; i++)
        {
            Explosion e = explosions[i];
            if (Vector2.Distance(e.location, position) < e.magnitude - 1 && (parent != e.parent) && e.canDamage.Contains(parent))
            {
                float force = Mathf.Abs(e.magnitude - Mathf.Abs(Mathf.Abs(position.x) - Mathf.Abs(e.location.x)));
                e.canDamage.Remove(parent);
                combinedForce += force;
            }
        }

        return combinedForce;
    }

    public GameObject GetTarget(Vector2 position, GameObject parent)
    {
        GameObject closestPlayer = null;
        float closestDistance = 100000;

        for (int i = 0; i < explosions.Count; i++)
        {
            Explosion e = explosions[i];
            if (Vector2.Distance(e.location, position) < e.magnitude + 2 && (parent != e.parent) && e.canDamage.Contains(parent))
            {
                if (Vector2.Distance(e.location, position) < closestDistance)
                {
                    closestPlayer = e.parent;
                    closestDistance = Vector2.Distance(e.location, position);
                }
            }
        }

        if (closestPlayer != null)
            return closestPlayer;

        for (int i = 0; i < explosions.Count; i++)
        {
            Explosion e = explosions[i];
            if (Vector2.Distance(e.location, position) < e.magnitude + 2 && (parent != e.parent))
                return e.parent;
        }

        return null;
    }
}

[System.Serializable]
public class Explosion
{
    public Vector2 location;
    public float magnitude;
    public GameObject parent;
    public List<GameObject> canDamage = new List<GameObject>();

    public Explosion(Vector2 v, float f, GameObject p, PlayerHandler playerHandler)
    {
        location = v;
        magnitude = f;
        parent = p;
        canDamage.Clear();
        if (playerHandler.Players.Count > 0)
            foreach (GameObject g in playerHandler.Players)
                canDamage.Add(g);
    }
}