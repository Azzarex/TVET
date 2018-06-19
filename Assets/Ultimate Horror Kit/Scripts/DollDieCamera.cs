using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AxlPlay
{
    public class DollDieCamera : MonoBehaviour
    {

        public GameObject jumpscareHolder;
   //     private Vector3 initialPos;
   //     private Quaternion initialRot = Quaternion.identity;

        private void Awake()
        {
     //       initialPos = transform.localPosition;
     //       initialRot = transform.localRotation;
        }
        // camera activated when the dolls kills the player to make an effect like falling
        private void OnEnable()
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }

        private void OnDisable()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            GetComponent<Rigidbody>().isKinematic = true;

        }
        public void DesactivateHolder()
        {
            jumpscareHolder.gameObject.SetActive(false);
        }
        public void ActivateHolder()
        {
            jumpscareHolder.gameObject.SetActive(true);
        }

    }
}
