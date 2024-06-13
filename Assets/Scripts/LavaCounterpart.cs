using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaCounterpart : MonoBehaviour
{
    public Lava lava;
    float timer;
    public float offset;

    void Update()
    {
        timer += Time.deltaTime;
        transform.position = lava.transform.position + (Mathf.Sin(timer * 1) * 2 * Vector3.right) + (offset * Vector3.down);
    }
}