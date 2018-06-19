using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AxlPlay
{
    public class Food : MonoBehaviour
    {
        public float HealthAmount = 10f;

        public void Pickuped()
        {
            transform.localScale = Vector3.zero;
        }
        // heal to the player
        public void Consume()
        {

            GameManager.Instance.PlayerHealth.CurrentHealth += HealthAmount;
            if (GameManager.Instance.PlayerHealth.CurrentHealth > GameManager.Instance.PlayerHealth.startCurrentHealth)
                GameManager.Instance.PlayerHealth.CurrentHealth = GameManager.Instance.PlayerHealth.startCurrentHealth;

            NCData data = new NCData();
            data._gameObject = this.gameObject;

            NotificationCenter.DefaultCenter.PostNotification(this, "ItemConsumed", data);

            Destroy(this.gameObject);
        }

    }
}