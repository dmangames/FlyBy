using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace MrFly
{
    [RequireComponent(typeof(MyFlyMovement))]
    public class MyFlyUserControl : MonoBehaviour
    {
        private MyFlyMovement m_Character;
        private bool m_Fly;


        private void Awake()
        {
            m_Character = GetComponent<MyFlyMovement>();
        }


        private void Update()
        {
                // Read the jump input in Update so button presses aren't missed.
                m_Fly = CrossPlatformInputManager.GetButton("Jump");
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script if there are any
            m_Character.Move(h, m_Fly);
        }
    }
}
