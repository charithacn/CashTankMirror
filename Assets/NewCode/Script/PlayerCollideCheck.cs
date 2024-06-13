using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollideCheck : MonoBehaviour
{
    public bool isGriounded = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("on trigger enter to collider of player");
        if(collision.gameObject.tag.Equals("Ground"))
        isGriounded = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
            isGriounded = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
            isGriounded = false;
    }
}
