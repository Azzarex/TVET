using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItemExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Called when the player pickup this item.
	public void Pickuped() {

	}
	// Called when the player consume this item.
	public void Consume() {

		// Once that the item has been consumed
		NotificationCenter.DefaultCenter.PostNotification (this, "ItemConsumed");

	}

	// Called when the player looks directly and closely this item.
	public void StartLooking() {

	}
	// Called when the player stops looking directly and closely this item.
	public void StopLooking() {

	}
}
