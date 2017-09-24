using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderFollowPlayer : MonoBehaviour {

    public GameObject target;

    public bool chasePlayer;
    public float moveSpeed;
    public float rotationSpeed;
    public float chargeDuration;
    public float rotateDuration;

    private Vector2 direction;
    private float chargeTicker;
    private float rotateTicker;

    private Vector2 startPosition;

    private GameObject musicTriggers; // Reference to all music triggers so he can turn them off to play the thumping noise

	// Use this for initialization
	void Start () {
        rotateTicker = rotateDuration;
        chargeTicker = 0;
        startPosition = transform.position;
        musicTriggers = GameObject.Find("Triggers");
	}
	
	// Update is called once per frame
	void Update () {
        if (chasePlayer)
        {


            if (rotateTicker > 0 && chargeTicker == 0)
            {
                //rotate to look at the player
                Quaternion newRotation = Quaternion.LookRotation(transform.position - target.transform.position, Vector3.forward);
                newRotation.x = 0;
                newRotation.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
                rotateTicker -= Time.deltaTime;
                if (rotateTicker <= 0)
                {
                    rotateTicker = 0;
                    chargeTicker = chargeDuration;

                }
            }
            else if (chargeTicker > 0 && rotateTicker == 0)
            {
                //move towards the player
                transform.position += transform.up * moveSpeed * Time.deltaTime;
                chargeTicker -= Time.deltaTime;
                if (chargeTicker <= 0)
                {
                    chargeTicker = 0;
                    rotateTicker = rotateDuration;
                }
            }
        }
    }

}
