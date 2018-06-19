using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AxlPlay;

namespace AxlPlay
{
    public enum BatteryTypes
    {
        Flashlight,
        Camera
        
    }

    public class Battery : MonoBehaviour
    {
        public bool NeedUseItemToRecharge = true;

        public BatteryTypes BatteryType = BatteryTypes.Flashlight;

        public void Pickuped()
        {
            transform.localScale = Vector3.zero;
        }

        public void Consume()
        {
            if (NeedUseItemToRecharge)
            {
                Rechargable rechargable = null;
                if (GameManager.Instance.Player.pickupSystem.CurrentItem.item)
                    rechargable = GameManager.Instance.Player.pickupSystem.CurrentItem.item.GetComponent<Rechargable>();

                if (rechargable && rechargable.BatteryType == BatteryType)
                {
                    if (rechargable.batteries < rechargable.MaxBatteries)
                    {
                        rechargable.AddBattery();
                        rechargable.totalDuration = rechargable.GetTotalDuration();
                        NCData data = new NCData();
                        data._gameObject = this.gameObject;

                        NotificationCenter.DefaultCenter.PostNotification(this, "ItemConsumed", data);
                        Destroy(this.gameObject);
                    }
                }
            }
            else
            {
                GameObject itemInInventory = null;
                if (BatteryType == BatteryTypes.Camera)
                    itemInInventory = GameManager.Instance.Player.pickupSystem.ReturnCameraItem();
                else if (BatteryType == BatteryTypes.Flashlight)
                    itemInInventory = GameManager.Instance.Player.pickupSystem.ReturnFlashlightItem();

                Rechargable rechargable = itemInInventory.GetComponent<Rechargable>();
                rechargable.AddBattery();
                NCData data = new NCData();
                data._gameObject = this.gameObject;
                NotificationCenter.DefaultCenter.PostNotification(this, "ItemConsumed", data);
                Destroy(this.gameObject);
            }
      
        }
    }

}
