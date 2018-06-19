using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AxlPlay
{
    // make a rechargeable item like a flashlight or camera
    public class Rechargable : MonoBehaviour
    {
        public int MaxBatteries = 2;
        public int StartBatteries = 2;
        [HideInInspector]

        public int batteries;
        [HideInInspector]
        public int batteryDuration = 30;

        [HideInInspector]
        public int actualBattery;
        [HideInInspector]

        public float actualBatteryTimer;

        [HideInInspector]
        public float totalDuration;
        public BatteryTypes BatteryType = BatteryTypes.Flashlight;

        private void Awake()
        {
            // load batteries of last played
            if (PlayerPrefs.GetString("firstPlay") == "true")
            {

                batteries = StartBatteries;

            }
            else
                Load();

        }

        private void Update()
        {

            if (GameManager.Instance.Player.pickupSystem.IsUsing(this.gameObject))
            {
                CheckBatteryUI();
                if (GetRemainingBattery() <= 0.1f)
                {
                    SendMessage("NoBattery", SendMessageOptions.DontRequireReceiver);
                }


                if (batteries == 0)
                {
                    actualBatteryTimer = 0f;
                    totalDuration = 0;
                }
                else if (actualBatteryTimer >= batteryDuration)
                {
                    batteries--;

                    actualBatteryTimer = 0f;
                    totalDuration = batteries * batteryDuration;

                }
            }
        }
        // consume battery needs to be called by the item in the update while is using
        public void ConsumeBattery()
        {
            actualBatteryTimer += Time.deltaTime;
        }
        public void StartUsing()
        {
            // calculate battery life
            totalDuration = batteries * batteryDuration;

            if (GameManager.Instance.BatteryLevelUI)
                GameManager.Instance.BatteryLevelUI.DoFadeIn();
        }
        public void StopUsing()
        {
            if (GameManager.Instance.BatteryLevelUI)
                GameManager.Instance.BatteryLevelUI.DoFadeOut();
        }
        public bool HasBattery()
        {
            if (GetRemainingBattery() > 0.1f)
            {
                return true;
            }
            return false;
        }
        // save batteries
        public void Save()
        {
            PlayerPrefs.SetInt(name + "batteries", batteries);

        }
        // load batteries

        public void Load()
        {
            if (PlayerPrefs.HasKey(name + "batteries"))
                batteries = PlayerPrefs.GetInt(name + "batteries");

        }
        private void OnApplicationQuit()
        {
            Save();
        }
        public void AddBattery()
        {
            batteries++;
            totalDuration = batteries * batteryDuration;

        }
        // show battery amount on canvas UI
        public void CheckBatteryUI()
        {

            var charge = 100 * GetRemainingBattery() / (MaxBatteries * batteryDuration);//totalDuration;
            if (charge > 80)
            {
                if (GameManager.Instance.BatteryLevel4)
                    GameManager.Instance.BatteryLevel4.DoFadeIn();
            }
            else
            {
                if (GameManager.Instance.BatteryLevel4)
                    GameManager.Instance.BatteryLevel4.DoFadeOut();
            }
            if (charge > 60)
            {
                if (GameManager.Instance.BatteryLevel3)
                    GameManager.Instance.BatteryLevel3.DoFadeIn();
            }
            else
            {
                if (GameManager.Instance.BatteryLevel3)
                    GameManager.Instance.BatteryLevel3.DoFadeOut();
            }
            if (charge > 40)
            {
                if (GameManager.Instance.BatteryLevel2)
                    GameManager.Instance.BatteryLevel2.DoFadeIn();
            }
            else
            {
                if (GameManager.Instance.BatteryLevel2)
                    GameManager.Instance.BatteryLevel2.DoFadeOut();
            }
            if (charge > 20)
            {
                if (GameManager.Instance.BatteryLevel1)
                    GameManager.Instance.BatteryLevel1.DoFadeIn();
            }
            else if (charge < 1)
            {
                if (GameManager.Instance.BatteryLevel1)
                    GameManager.Instance.BatteryLevel1.DoFadeOut();
            }

        }
        public float GetTotalDuration()
        {
            float amount = 0f;
            //   foreach (var item in batteries)

            for (int i = 0; i < batteries; i++)
            {
                amount += batteryDuration;// item.Duration;
            }
            return amount;
        }
        // know how much battery left
        public float GetRemainingBattery()
        {

            return Mathf.Abs(actualBatteryTimer - (totalDuration));
        }

    }
}
