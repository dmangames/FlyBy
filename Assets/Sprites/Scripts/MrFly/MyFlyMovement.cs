using System;
using UnityEngine;
using System.Collections;

namespace MrFly
{
    public class MyFlyMovement : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_MoveForce = 5f;                  // Amount of force added when the player moves.
        [SerializeField] private float m_FlyForce = 40f;                  // Amount of force added when the player jumps.
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] private LayerMask m_WhatIsCeiling;                  // A mask determining what is ceiling to the character (usually ceiling stuff kills the character)

        public Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
		private Vector2 k_GroundedBoxDims = new Vector2(0.15f, 0.05f); // Dimensions of ground check box to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private bool m_Touching;            // Whether or not the player is touching anything.
        private bool m_AirIdle;             // Whether or not the player is idle in the air, for example in a web
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        private Vector2 k_CeilingBoxDims = new Vector2(0.15f, 0.05f); // Dimensions of ground check box to determine if touching ceiling
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
		private bool m_KnockedOut = false; // If player makes contact with anything in the air, he gets knocked out and falls to the ground
        private float k_KnockedOutDuration = 2; // Player is knocked out for 2 seconds, then recovers
        private float m_KnockedOutTicker = 0; // Keeps track of how long player has been knocked out
        private Vector2 respawnPoint; // Position the player goes back to after dying
        private SpiderFollowPlayer spiderFollowPlayer; // Reference to this component on spider. Used to reset the spider on player death
        //private BoulderSpawner boulderSpawner; // Reference to this component on the boulder spawner. Used to reset the boulders on player death
        private GameObject dizzyEffect; // Gameobject with the dizzy animation on it.
        private bool k_isDead; // Determines if the character is dead or not. He becomes static and can't move if dead


        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            respawnPoint = new Vector2(7.89f, -27.89f);

        }

        private void Update()
        {

            if (m_KnockedOutTicker > 0)
            {
                m_KnockedOutTicker -= Time.deltaTime;
            }
            else
            {
                m_KnockedOut = false;
                m_KnockedOutTicker = 0;
            }
        }


        private void FixedUpdate()
        {
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapBoxAll(m_GroundCheck.position, k_GroundedBoxDims, 0, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
					//Debug.Log (colliders [i].gameObject.name);
                    m_Grounded = true;
					m_KnockedOut = false;
                }

            }

            colliders = Physics2D.OverlapBoxAll(m_CeilingCheck.position, k_CeilingBoxDims, 0, m_WhatIsCeiling);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.tag == "boulder")
                {
                    //Debug.Log(colliders[i].gameObject.name);
                    if(colliders[i].gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > 3f)
                    {
                        kill();
                    }
                }

            }
            m_Anim.SetBool("Ground", m_Grounded);
            //Debug.Log(m_Rigidbody2D.velocity);
        }


        private void OnCollisionStay2D(Collision2D collision)
        {
            m_Touching = true;
			if (!m_Grounded) {
                if (m_KnockedOutTicker == 0)
                {
                    m_KnockedOut = true;
                    m_KnockedOutTicker = k_KnockedOutDuration;
                }
			}
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            m_Touching = false;
        }

        public void Move(float move, bool fly)
        {

            //only control the player left and right if he is not grounded
            if (!m_Grounded)
            {

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Clamp(m_Rigidbody2D.velocity.y, 0f, 10f));

                // Move the character
                //m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);
                if (Math.Abs(m_Rigidbody2D.velocity.x) < m_MaxSpeed * .6)
                {
                    if (!m_Grounded)
                    {
                        m_Rigidbody2D.AddForce(new Vector2(move * m_MoveForce * m_Rigidbody2D.mass, 0), ForceMode2D.Force);
                    }
                }
                else if (Math.Abs(m_Rigidbody2D.velocity.x) < m_MaxSpeed)
                {
                    if (!m_Grounded)
                    {
                        m_Rigidbody2D.AddForce(new Vector2(move * m_MoveForce * .5f * m_Rigidbody2D.mass, 0), ForceMode2D.Force);
                    }
                }


                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player tries to fly
            if (m_Grounded && fly && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_FlyForce * 1.5f * m_Rigidbody2D.mass));
            }
			else if (fly && !m_KnockedOut)
            {
                m_Rigidbody2D.AddForce(new Vector2(0f, m_FlyForce * m_Rigidbody2D.mass));
            }
//            Debug.Log("Velocity: " + m_Rigidbody2D.velocity);
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        public void setKnockDown(bool knockedOut)
        {
            m_KnockedOut = knockedOut;
            if (knockedOut)
            {
                m_KnockedOutTicker = k_KnockedOutDuration;
            }
            else
            {
                m_KnockedOutTicker = 0;
            }
        }

        public void setRespawnPoint(Vector2 pos)
        {
            respawnPoint = pos;
        }

        public void kill()
        {
            // Set fly to dead
            k_isDead = true;

            // If fly is dead, then take him out of physics
            m_Rigidbody2D.bodyType = RigidbodyType2D.Static;

            // Spider stops chasing player
            spiderFollowPlayer.chasePlayer = false;

            // Play death animationx
            m_Anim.SetBool("Dead", k_isDead);


            // Start revive
            //StartCoroutine(RevivePlayer(1));

        }

        public void RevivePlayerWrapper(float secs)
        {
            RevivePlayer(secs);
        }

        IEnumerator RevivePlayer(float secs)
        {

            yield return new WaitForSeconds(secs);

            // Revive player
            k_isDead = false;

            // Set Dead bool in anim
            m_Anim.SetBool("Dead", k_isDead);

            // Put fly back in physics
            m_Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;


            // Set position to respawn point
            transform.position = respawnPoint;

        }

        public void setLinearDrag(float linearDrag)
        {
            m_Rigidbody2D.drag = linearDrag;
        }
    }
}
