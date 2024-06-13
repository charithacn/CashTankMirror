using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public GameObject lavaObject;
    public float speed = 0.02f;
    public bool moving;

    void Start()
    {
        ResetLava();
    }

    void Update()
    {
        if (moving)
            lavaObject.transform.position += Vector3.up * speed * Time.deltaTime;
    }

    public void ResetLava()
    {
        lavaObject.transform.position = new Vector3(0, -255, 0);
    }
}