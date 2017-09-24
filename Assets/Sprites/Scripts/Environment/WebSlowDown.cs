using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebSlowDown : MonoBehaviour {

    public GameObject player;

    public float webLinearDrag = 40;

    private float originalDrag;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        originalDrag = player.GetComponent<Rigidbody2D>().drag;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.GetComponent<MrFly.MyFlyMovement>().setLinearDrag(webLinearDrag);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.GetComponent<MrFly.MyFlyMovement>().setLinearDrag(originalDrag);
        }
    }
}
