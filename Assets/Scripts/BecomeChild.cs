using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BecomeChild : MonoBehaviour
{
    public Transform parent;

    void Awake()
    {
        transform.SetParent(parent, true);
    }

    void Update()
    {
        
    }
}