using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBehaviour : MonoBehaviour {

    private Rigidbody2D rb;
    private AudioManagerLocal aml;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        aml = GetComponent<AudioManagerLocal>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            if (Mathf.Abs(rb.velocity.y) > 3f)
            {
                aml.Play("Hit");
            }
            else if (rb.velocity.magnitude > .5f)
            {
                int num = (int)Random.Range(1, 4);
                string sound = "Roll" + num.ToString();
                aml.Play(sound);
            }
        }
    }
}
