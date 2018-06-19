using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace AxlPlay {

	public class NoteSystem : MonoBehaviour {

		public GameObject Canvas;
		public Sprite NoteImage;

		public Font NoteTitleFont;
		public Font NoteTextFont;

		public string NoteTitle;
		public string NoteText;

		public GameObject canvasNote;
		public UIEffects noteEffects;

		void Awake() {
			if (!noteEffects && canvasNote) {
				noteEffects = canvasNote.GetComponent<UIEffects> ();
			}
		}

		public void StartExamining() {
		
			if (noteEffects != null)
				noteEffects.DoFadeIn ();
		}
		public void StopExamining() {
	
			if (noteEffects != null)
				noteEffects.DoFadeOut ();
		}

		public void LoadNotePrefab() {
			if (canvasNote != null)
				DestroyImmediate (canvasNote);
			
			canvasNote = (GameObject)Instantiate (Resources.Load ("Note"));
			canvasNote.transform.parent = Canvas.transform;
			canvasNote.transform.localPosition = Vector3.zero;
			canvasNote.GetComponent<Image> ().sprite = NoteImage;
            canvasNote.GetComponent<Image>().raycastTarget = false;

            canvasNote.GetComponent<CanvasGroup> ().alpha = 0f;
			Text titleText = GameObject.Find ("Title").GetComponent<Text> ();
			titleText.text = NoteTitle;
			titleText.font = NoteTitleFont;
			Text text = GameObject.Find ("Text").GetComponent<Text> ();
			text.text = NoteText;
			text.font = NoteTextFont;

			noteEffects = canvasNote.GetComponent<UIEffects> ();
		}
}

}
