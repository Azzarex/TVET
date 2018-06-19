using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace AxlPlay
{
    public class CameraSystem : MonoBehaviour
    {

        private float zoomTime = 1f;

        public int BrokenGlassesCount;
        public List<Image> BrokenGlasses = new List<Image>();

        public AnimationClip ZoomInAnimation;
        public AnimationClip ZoomOutAnimation;

        public AudioSource CameraAudioSource;
        //	public KeyCode ZoomKey = KeyCode.Mouse1;
        public bool CanZoom;
        public AudioClip ZoomInSound;
        public AudioClip ZoomOutSound;
        public AudioClip BrokenSound;

        public GameObject CameraCanvas;
        public Slider ZoomSlider;
        public UIEffects CanvasEffects;
        public Animation CameraAnimation;
        public Camera PlayerCamera;
        public CanvasGroup CameraCanvasGroup;
        private bool isZooming;
        private bool isZoomed;
        public bool IsBroken;
    //    private Animation animationC;
        private Rechargable rechargable;
        void Awake()
        {
     //       animationC = GetComponent<Animation>();
            rechargable = GetComponent<Rechargable>();
        }

        void Start()
        {
            if (CameraAudioSource)
            {
                CameraAudioSource.loop = false;
                CameraAudioSource.playOnAwake = false;
            }
        }

        void Update()
        {
            if (GameManager.Instance.Player.pickupSystem.IsUsing(this.gameObject))
            {

                rechargable.ConsumeBattery();

                if (isZooming)
                {
                    if (!isZoomed)
                    {
                        ZoomSlider.value -= Time.deltaTime * zoomTime;
                    }
                    else
                    {

                        ZoomSlider.value += Time.deltaTime * zoomTime;
                    }
                }

                if (InputManager.inputManager.GetButtonDown(InputManager.inputManager.ZoomInput) && !isZooming && CanZoom)
                {

                    if (!isZoomed)
                    {


                        ZoomIn();

                    }
                    else
                    {

                        ZoomOut();

                    }
                }


            }
            /*
            else {
				if (PlayerData.PRGlitchEffect) {
					if (CameraCanvasGroup.alpha == 0f) {
						if (PlayerData.PRGlitchEffect.enabled) {
							PlayerData.PRGlitchEffect.enabled = false;
						}
					}
				}
			}
            */
        }

        public void Pickuped()
        {
            //transform.parent = PlayerData.InventoryItems.transform;
            transform.localScale = Vector3.zero;
        }
      
        public void StartUsing()
        {
            if (!GetComponent<Rechargable>().HasBattery())
                return;
            if (IsBroken)
                Broke();
         
          

            if (CanvasEffects != null)
            {
                CanvasEffects.DoFadeIn();
            }


        }

        public void StopUsing()
        {

       

            if (CanvasEffects != null)
                CanvasEffects.DoFadeOut();



        }

        public void Broke()
        {
            if (GameManager.Instance.Player.pickupSystem.IsUsing(this.gameObject))
            {
                if (BrokenGlassesCount > 0)
                {
                    int randomBrokenGlassIndex = Random.Range(0, BrokenGlasses.Count - 1);
                    Image brokenGlass = (Image)Instantiate(BrokenGlasses[randomBrokenGlassIndex], CameraCanvas.transform);
                    brokenGlass.GetComponent<RectTransform>().localPosition = Vector3.zero;

                    brokenGlass.gameObject.SetActive(true);
                }

                CanZoom = false;
                if (isZoomed)
                    ZoomOut();
          
                if (CameraAudioSource && BrokenSound && !IsBroken)
                    CameraAudioSource.PlayOneShot(BrokenSound);
            }
            IsBroken = true;


        }
        public void NoBattery()
        {
            GameManager.Instance.Player.pickupSystem.StopUsing();
        }
        public void LoadCanvas()
        {
            if (CameraCanvas)
                DestroyImmediate(CameraCanvas);
            CameraCanvas = (GameObject)Instantiate(Resources.Load("Camera Canvas"));
            CameraCanvasGroup = CameraCanvas.GetComponent<CanvasGroup>();
            CanvasEffects = CameraCanvas.GetComponent<UIEffects>();
            ZoomSlider = (Slider)CameraCanvas.GetComponentInChildren(typeof(Slider), true);

        }

        public void AddAnimationToCamera()
        {
            if (CameraAnimation)
                DestroyImmediate(CameraAnimation);
            if (!PlayerCamera.GetComponent<Animation>())
            {
                CameraAnimation = PlayerCamera.transform.gameObject.AddComponent<Animation>();
            }
            else
                CameraAnimation = PlayerCamera.transform.gameObject.GetComponent<Animation>();
            if (CameraAnimation != null)
            {
                CameraAnimation.AddClip(ZoomInAnimation, "ZoomIn");
                CameraAnimation.AddClip(ZoomOutAnimation, "ZoomOut");
            }
        }

        IEnumerator Zooming()
        {
            isZooming = true;
            yield return new WaitForSeconds(zoomTime);
            isZooming = false;
        }

        public void ZoomOut()
        {
            StartCoroutine(Zooming());
            isZoomed = false;

            if (CameraAnimation && ZoomOutAnimation)
                CameraAnimation.Play(ZoomOutAnimation.name);
            if (CameraAudioSource && ZoomOutSound)
                CameraAudioSource.PlayOneShot(ZoomOutSound);
        }

        public void ZoomIn()
        {
            StartCoroutine(Zooming());
            isZoomed = true;
            if (CameraAnimation && ZoomInAnimation)
                CameraAnimation.Play(ZoomInAnimation.name);

            if (CameraAudioSource && ZoomInSound)
                CameraAudioSource.PlayOneShot(ZoomInSound);
        }

    }

}
