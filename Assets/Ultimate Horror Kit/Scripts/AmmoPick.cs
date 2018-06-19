using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AxlPlay
{
    // Only pickup ammo when the player's weapon is compatible with this type of magazine
    public enum WeaponAmmoTypes
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N

    }
    // for magazine on the floor pick up them
    public class AmmoPick : MonoBehaviour
    {
        public int AmmoAmount = 30;
        public WeaponAmmoTypes WeaponType;

        private void Interaction()
        {

        
            if (GameManager.Instance.Player.pickupSystem.CurrentItem.weapon == null)
                return;

            if (GameManager.Instance.Player.pickupSystem.CurrentItem.weapon.WeaponType != WeaponType)
                return;
            // when a player is up this ammo box
            // add ammo and desactivate this box
            GameManager.Instance.Player.pickupSystem.CurrentItem.weapon.ammunition += AmmoAmount;
            GameManager.Instance.UpdateAmmoUI();
            gameObject.SetActive(false);

        }

    }
}