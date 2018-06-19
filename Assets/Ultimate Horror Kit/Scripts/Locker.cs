using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AxlPlay
{
    public class Locker : MonoBehaviour
    {
        public Door LockerDoor;
        public float RotSpeed = 2.5f;
        public Transform InsideLocker;
        [HideInInspector]
        public PlayerController playerInLocker;

        // make the player enter to the locker when enters the trigger
        private void OnTriggerEnter(Collider other)
        {
            playerInLocker = other.GetComponent<PlayerController>();
            if (playerInLocker)
                EnterToLocker();
        }
      /*  private void OnTriggerExit(Collider other)
        {
            if (playerInLocker)
                ExitFromLocker();
        }
        */
        private void Update()
        {
            if(playerInLocker)
            {
                playerInLocker.transform.rotation = Quaternion.Lerp(playerInLocker.transform.rotation,InsideLocker.transform.rotation, RotSpeed * Time.deltaTime);

            }

        }
        void OpenDoor()
        {
            if(playerInLocker)
            ExitFromLocker();
        }
        public void EnterToLocker()
        {
            // freeze player and make him look outwards
            playerInLocker.CanMove = false;

            playerInLocker.GetComponent<Rigidbody>().isKinematic = true;
            playerInLocker.GetComponent<PlayerMotor>().enabled = false;
            //   playerInLocker.transform.SetParent(InsideLocker.transform);
            playerInLocker.transform.position = InsideLocker.position;//Vector3.zero;
            LockerDoor.OpenOrCloseDoorByEvent();
            playerInLocker.pickupSystem.ToggleCurrentItem();
        }
        public void ExitFromLocker()
        {
            // unfreeze player
            playerInLocker.CanMove = true;
            playerInLocker.GetComponent<Rigidbody>().isKinematic = false;
            playerInLocker.GetComponent<PlayerMotor>().enabled = true;
            //     playerInLocker.transform.SetParent(null);

            playerInLocker.pickupSystem.ToggleCurrentItem();

            playerInLocker = null;
        }
    }
}