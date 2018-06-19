using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace AxlPlay
{
    // game options like game quality and sensitivity speed
    public class GameOptions : MonoBehaviour
    {

        public Dropdown ResolutionDropDown;
        public Dropdown QualityDropDown;
        public Dropdown VSyncDropDown;
        public Dropdown ShadowsDropDown;
        public Dropdown AntialiasingDropDown;

        public SliderAmount Volume;
        public SliderAmount LookSensitivity;
        public Toggle FullScreen;
        public static GameOptions Instance;

        private void Awake()
        {
            Instance = this;
            // load settings
            if (PlayerPrefs.HasKey("resolution"))

                ResolutionDropDown.value = (PlayerPrefs.GetInt("resolution"));
            if (PlayerPrefs.HasKey("quality"))

                QualityDropDown.value = (PlayerPrefs.GetInt("quality"));
            if (PlayerPrefs.HasKey("antialiasing"))

                AntialiasingDropDown.value = (PlayerPrefs.GetInt("antialiasing"));
            if (PlayerPrefs.HasKey("vsync"))

                VSyncDropDown.value = (PlayerPrefs.GetInt("vsync"));
            if (PlayerPrefs.HasKey("shadows"))

                ShadowsDropDown.value = (PlayerPrefs.GetInt("shadows"));
            if (PlayerPrefs.HasKey("fullscreen"))
                FullScreen.isOn = (PlayerPrefs.GetInt("fullscreen") == 1) ? true : false;
            if (PlayerPrefs.HasKey("volume"))
                Volume.slider.value = PlayerPrefs.GetFloat("volume");
            if (PlayerPrefs.HasKey("looksensitivity"))
                LookSensitivity.slider.value = PlayerPrefs.GetFloat("looksensitivity");

        }
        // set settings
        public void OnChangedResolutionInDropDown(int index)
        {

            PlayerPrefs.SetInt("resolution", index);
            switch (index)
            {
                case 0:
                    Screen.SetResolution(1920, 1080, FullScreen);
                    break;
                case 1:
                    Screen.SetResolution(1600, 900, FullScreen);
                    break;
                case 2:
                    Screen.SetResolution(1366, 768, FullScreen);
                    break;
                case 3:
                    Screen.SetResolution(1280, 720, FullScreen);
                    break;
                case 4:
                    Screen.SetResolution(1024, 768, FullScreen);
                    break;
                case 5:
                    Screen.SetResolution(800, 600, FullScreen);
                    break;
                default:

                    break;
            }
        }
        public void OnQualityChangedInDropDown(int index)
        {
            PlayerPrefs.SetInt("quality", index);

            QualitySettings.SetQualityLevel(index);


        }

        public void OnAntialiasingChangedInDropDown(int index)
        {


            PlayerPrefs.SetInt("antialiasing", index);

            switch (index)
            {
                case 0:
                    QualitySettings.antiAliasing = 2;
                    break;
                case 1:
                    QualitySettings.antiAliasing = 4;
                    break;
                case 2:
                    QualitySettings.antiAliasing = 8;
                    break;

            }
        }
        public void OnVSyncChangedInDropDown(int index)
        {
            PlayerPrefs.SetInt("vsync", index);

            QualitySettings.vSyncCount = index;
        }
        public void OnShadowsChangedInDropDown(int index)
        {
            PlayerPrefs.SetInt("shadows", index);

            QualitySettings.shadows = (ShadowQuality)index;
        }
        public void OnFullScreenChangedInToggle(bool value)
        {
            if (value)
                PlayerPrefs.SetInt("fullscreen", 1);
            else
                PlayerPrefs.SetInt("fullscreen", 0);

            Screen.fullScreen = value;
        }
        // save preferences
        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }
        public void OnChangedLookSensitivity(float value)
        {
            PlayerPrefs.SetFloat("looksensitivity", value);
         // made in player
            //   GameManager.Instance.Player.LookSensitivity = value;

        }
        public void OnChangedVolume(float value)
        {
            PlayerPrefs.SetFloat("volume", value);
            AudioListener.volume = value;
        }
    }
}