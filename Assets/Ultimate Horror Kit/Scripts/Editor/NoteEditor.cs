
using UnityEngine;
using UnityEditor;
using AxlPlay;

namespace AxlPlay {
[CustomEditor(typeof(NoteSystem))]
public class NoteEditor : Editor {

	public override void OnInspectorGUI()
	{
		NoteSystem _target = (NoteSystem)target;

		_target.Canvas = (GameObject)EditorGUILayout.ObjectField ("Canvas", _target.Canvas, typeof(GameObject), true);

		_target.NoteImage = (Sprite)EditorGUILayout.ObjectField ("Note Image", _target.NoteImage, typeof(Sprite), true);

			_target.NoteTitle = EditorGUILayout.TextField ("Title", _target.NoteTitle);

		_target.NoteText = EditorGUILayout.TextField ("Text", _target.NoteText);

		_target.NoteTitleFont = (Font)EditorGUILayout.ObjectField ("Title Font", _target.NoteTitleFont, typeof(Font), true);
		_target.NoteTextFont = (Font)EditorGUILayout.ObjectField ("Text Font", _target.NoteTextFont, typeof(Font), true);



		if (GUILayout.Button ("Add note to Canvas",GUILayout.Width(130f),GUILayout.Height(40f))) {
			_target.LoadNotePrefab ();
		}

			EditorGUILayout.LabelField ("Optional");
			_target.canvasNote = (GameObject)EditorGUILayout.ObjectField ("Note UI", _target.canvasNote, typeof(GameObject), true);

			if(GUI.changed)
				EditorUtility.SetDirty(_target); 
	}
}

}