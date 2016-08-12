using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
				// This was removed by Kaz Crowe because this package does not include Unity's CrossPlatformInputManager.
                //m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");

				// If you want a nice and easy UI Button to use for your Mobile Games, please see the Ultimate Button package on the Unity Asset Store.
				// Asset Store Link: https://www.assetstore.unity3d.com/en/#!/content/28824
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
			// read inputs - Modified by Kaz Crowe
			//float h = CrossPlatformInputManager.GetAxis( "Horizontal" );
			//float v = CrossPlatformInputManager.GetAxis( "Vertical" );
			// Create a Vector2 and replace h with joystickPosition.x and replace v with joystickPosition.y
			Vector2 joystickPosition = UltimateJoystick.GetPosition( "Movement" );
			bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;

				// Modified by Kaz Crowe \/
				m_Move = joystickPosition.y * m_CamForward + joystickPosition.x * m_Cam.right;

				// Original Code \/
                //m_Move = v*m_CamForward + h*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera

				// Modified by Kaz Crowe \/
				m_Move = joystickPosition.y * Vector3.forward + joystickPosition.x * Vector3.right;

				// Original Code \/
                //m_Move = v*Vector3.forward + h*Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }

		public void PlayerJump ()
		{
			m_Jump = true;
		}
    }
}
