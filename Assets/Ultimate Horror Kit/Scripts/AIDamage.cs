using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AxlPlay
{
    public class AIDamage : MonoBehaviour
    {

        public float Damage = 10f;

        [HideInInspector]
        public bool canDamage;
        private void OnTriggerEnter(Collider other)
        {
            // can damage is set by melee attack animation event
            if (canDamage)
            {
                // and when trigger with health component apply damage
                Health health = other.GetComponent<Health>();
                if (health)
                    health.TakeDamage(Damage, Vector3.zero, this.gameObject);
            }
            
        }

    }
}