using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankAutoShooting : MonoBehaviour
    {
        public Rigidbody m_Shell;                   // Prefab of the shell.
        public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio.
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
        public float m_LaunchForce = 20f;           // The force given to the shell when firing.
        public float m_FireRate = 1f;               // How often the tank can fire (in seconds).
        public float m_DetectionRange = 20f;        // The range at which the tank can detect the player.

        private Transform m_PlayerTransform;        // Reference to the player's transform.
        private float m_NextFireTime;               // The time when the tank can fire next.

        private void Start()
        {
            // Find the player in the scene
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_PlayerTransform = player.transform;
            }
            else
            {
//                Debug.LogError("Player not found in the scene. Make sure the player has the 'Player' tag.");
            }

            m_NextFireTime = Time.time;
        }

        private void Update()
        {
            if (m_PlayerTransform == null)
                return;

            // Calculate the direction to the player
            Vector3 directionToPlayer = m_PlayerTransform.position - transform.position;
            directionToPlayer.y = 0; // Keep the tank level

            // Check if the player is within detection range
            if (directionToPlayer.magnitude <= m_DetectionRange)
            {
                // Rotate towards the player
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

                // Check if it's time to fire
                if (Time.time >= m_NextFireTime)
                {
                    Fire();
                    m_NextFireTime = Time.time + m_FireRate;
                }
            }
        }

        private void Fire()
        {
            // Create an instance of the shell and store a reference to its rigidbody.
            Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.velocity = m_LaunchForce * m_FireTransform.forward; 

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();
        }
    }
}