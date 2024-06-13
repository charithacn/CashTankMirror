using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptBoy.Digable2DTerrain;

public class ResetTerrain : MonoBehaviour
{
    public List<GameObject> terrains = new List<GameObject>();
    public List<GameObject> terrainsfr = new List<GameObject>();

    void Awake()
    {
        int index = 0;
        foreach (Transform child in transform)
        {
            terrainsfr.Add(child.gameObject);
            terrains.Add(Instantiate(child.gameObject));
            terrains[terrains.Count - 1].SetActive(false);
            index++;
            if (index > 10000)
                break;
        }
    }

    public void Refresh()
    {
        foreach (GameObject g in terrainsfr)
            Destroy(g);

        int index = 0;
        foreach (GameObject g in terrains)
        {
            terrainsfr.Add(Instantiate(g, transform));
            terrainsfr[terrainsfr.Count - 1].SetActive(true);
            index++;
            if (index > 10000)
                break;
        }
    }

    public void UpdateObjectCount()
    {
        terrainsfr.Clear();
        int index = 0;
        foreach (Transform child in transform)
        {
            terrainsfr.Add(child.gameObject);
            index++;
            if (index > 10000)
                break;
        }
    }
}