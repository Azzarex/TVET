using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
namespace AxlPlay
{
    public class MainMenu : MonoBehaviour
    {

        public UIEffects LoadingScreen;
        public Slider sliderBar;
        public Text loadingText;

        public string GameSceneName = "Demo";
        public string DollGameSceneName = "DollsDemo";

        public GameObject ContinueGameBt;

        private void Awake()
        {
            if (PlayerPrefs.HasKey("game1"))
                ContinueGameBt.gameObject.SetActive(true);
            else
                ContinueGameBt.gameObject.SetActive(false);

        }
     
        public void ContinueGame()
        {
            LoadingScreen.DoFadeIn();
            StartCoroutine(LoadNewScene(GameSceneName));
        }
        public void PlayDollsGame()
        {
            LoadingScreen.DoFadeIn();

            StartCoroutine(LoadNewScene(DollGameSceneName));

        }
        public void NewGame()
        {

            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("game1", "true");
            PlayerPrefs.Save();

            StartCoroutine(LoadNewScene(GameSceneName));

        }
        // load scene with loading bar
        IEnumerator LoadNewScene(string sceneName)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

            while (!async.isDone)
            {
                float progress = Mathf.Clamp01(async.progress / 0.9f);
                sliderBar.value = progress;
                loadingText.text = "Loading... (" + progress * 100f + "%)";
                yield return null;
            }
        }
    }
}