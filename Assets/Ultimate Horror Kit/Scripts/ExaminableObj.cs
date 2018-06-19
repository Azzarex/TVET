using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AxlPlay;

namespace AxlPlay
{
    public class ExaminableObj : MonoBehaviour
    {

        public GameObject ModelView;
        public float rotationSpeed = 5f;

        private float newX;
        private float newY;
        private bool isExaminingThis;

        void Update()
        {

            if (isExaminingThis)
            {
                if (Input.GetMouseButton(0))
                {
                    newY += Input.GetAxis("Mouse Y") * rotationSpeed;
                    newX += Input.GetAxis("Mouse X") * rotationSpeed;
                    newY = Mathf.Clamp(newY, -360, 360);
                }

                ModelView.transform.localEulerAngles = new Vector3(newY, newX, 0);
            }
            else
            {
                newX = 0;
                newY = 0;
            }
        }
        // called by interactive object
        public void StartExamining()
        {
            //activate model examinable
            ModelView.SetActive(true);
            // make invisible this
            transform.localScale = Vector3.zero;
         // player cant look around while examining
            GameManager.Instance.Player.canLookAround = false;

            isExaminingThis = true;
        }
        public void StopExamining()
        {

            ModelView.SetActive(false);
            transform.localScale = Vector3.one;

       

            isExaminingThis = false;
            GameManager.Instance.Player.canLookAround = true;

        }
        // create examinable model (because requires to be inside the player camera)
        public void Create()
        {
            if (ModelView != null)
                DestroyImmediate(ModelView);

            ModelView = (GameObject)Instantiate(this.gameObject, GameManager.Instance.ExaminableObjectTransform);
            ModelView.transform.localScale = this.transform.localScale;
            ModelView.transform.localPosition = Vector3.zero;
            ModelView.transform.localRotation = this.transform.localRotation;
            ModelView.SetActive(false);
            SetLayerRecursively(ModelView, 10);

            //	DestroyImmediate (ModelView.GetComponent<ExaminableObj>());
            //	DestroyImmediate (ModelView.GetComponent<InteractiveObject>());
            foreach (MonoBehaviour script in ModelView.GetComponents(typeof(MonoBehaviour)))
            {
                DestroyImmediate(script);

            }
            foreach (Collider collider in ModelView.GetComponents(typeof(Collider)))
            {
                DestroyImmediate(collider);

            }

        }

        public void SetLayerRecursively(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

    }

}