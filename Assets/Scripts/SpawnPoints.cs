using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public List<Vector3> spawns = new List<Vector3>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            spawns.Add(child.position);
            child.gameObject.SetActive(false);
        }
    }
}