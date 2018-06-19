using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace AxlPlay
{
	[RequireComponent (typeof(Collider))]
	public class SendEventOnTrigger : MonoBehaviour
	{


		public string Tag;
		public bool EnterEventCanRepeat;
		public bool StayEventCanRepeat;
		public bool ExitEventCanRepeat;
		public UnityEvent OnTriggerEnterEvent;
		public UnityEvent OnTriggerStayEvent;
		public UnityEvent OnTriggerExitEvent;

		private bool hasEnter;
		private bool hasStay;
		private bool hasExit;

		void Reset ()
		{
			GetComponent<BoxCollider> ().isTrigger = true;
		}
        // send events on the different trigger events of unity
		void OnTriggerEnter (Collider other)
		{
			if (EnterEventCanRepeat || !hasEnter) {
				if (OnTriggerEnterEvent != null) {
					if (other.tag == Tag)
						OnTriggerEnterEvent.Invoke ();

				}
			}
			hasEnter = true;
		}

		void OnTriggerStay (Collider other)
		{
			if (StayEventCanRepeat || !hasStay) {
				
				if (OnTriggerStayEvent != null) {
					if (other.tag == Tag)
						OnTriggerStayEvent.Invoke ();

				}
			}
			hasStay = true;
		}

		void OnTriggerExit (Collider other)
		{
			if (ExitEventCanRepeat || !hasExit) {
				
				if (OnTriggerExitEvent != null) {
					if (other.tag == Tag)
						OnTriggerExitEvent.Invoke ();
				}

			}

			hasExit = true;
		}
	}

}