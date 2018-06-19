using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AxlPlay
{
    public class Torch : MonoBehaviour
    {
        public Collider colliderC;

        public Transform SpawnIn;
        public GameObject Flame;

        private Vector3 startScale;
        private void Awake()
        {
           startScale = transform.localScale;
        }
        public void Pickuped()
        {
            if (colliderC)
                colliderC.enabled = false;
            transform.parent = SpawnIn;

            transform.localScale = Vector3.zero;
            Flame.SetActive(false);
        }
        public void StartUsing()
        {

            if (transform.parent && transform.parent.parent)
                GetComponent<InteractiveObject>().animationC = transform.parent.parent.GetComponent<Animation>();

            transform.localPosition = Vector3.zero;
            transform.localRotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = startScale;
            Flame.SetActive(true);

        }


        public void StopUsing()
        {

            transform.localScale = Vector3.zero;
            Flame.SetActive(false);

        }
    }
}