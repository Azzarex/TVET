using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using AxlPlay;

namespace AxlPlay
{
	public class ShowInstructions : MonoBehaviour
	{
		public UIEffects txtEffects;
		public Text InstructionTxt;

		void Start() {
			NotificationCenter.DefaultCenter.AddObserver (this, "KeyDontMatch");
			NotificationCenter.DefaultCenter.AddObserver (this, "NeedAKey");
			NotificationCenter.DefaultCenter.AddObserver (this, "InventoryIsFull");

		}

		public IEnumerator KeyDontMatch() {
			if(InstructionTxt)
			InstructionTxt.text = "The key dont match with the lock type of the door";
			if(txtEffects)
			txtEffects.DoFadeIn ();
			yield return new WaitForSeconds (2f);
			if (txtEffects)
				txtEffects.DoFadeOut ();
		
		}

		public IEnumerator NeedAKey() {
			if(InstructionTxt)
				InstructionTxt.text = "You need a key to open this door";
			if(txtEffects)
				txtEffects.DoFadeIn ();
			yield return new WaitForSeconds (2f);
			if (txtEffects)
				txtEffects.DoFadeOut ();

		}

		public IEnumerator InventoryIsFull() {
			if(InstructionTxt)
				InstructionTxt.text = "The inventory is full";
			if(txtEffects)
				txtEffects.DoFadeIn ();
			yield return new WaitForSeconds (2f);
			if (txtEffects)
				txtEffects.DoFadeOut ();
		}
	
	}

}