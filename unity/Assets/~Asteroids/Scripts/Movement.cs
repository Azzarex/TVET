using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids
{
    public class Movement : MonoBehaviour
    {
        public float speed = 20f; // Units travel per second 
        public float rotationSpeed = 360f; // Amount of rotations per second

        private Rigidbody2D rigid; //Reference to attacthed Rigibody2D

        // Use this for initialization
        void Start()
        {
            // Tried to add force in the transform's up direction via speed
            rigid = GetComponent<Rigidbody2D>();
        } // "im not a asshole, im a hemaroid, i irritate assholes." - Me *copyright symbol*

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                //Rotate left
                transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.W))
            {
                // This says 'move yer afff forward boi'
                rigid.AddForce(transform.up * speed);
            }
        }
    }
}
