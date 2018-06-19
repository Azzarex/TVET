using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AxlPlay;

namespace AxlPlay {
[CustomEditor(typeof(ExaminableObj))]
public class ExaminableObjEditor : Editor {


	public override void OnInspectorGUI()
	{
		ExaminableObj _target = (ExaminableObj)target;

		_target.rotationSpeed = EditorGUILayout.FloatField ("Rotation Speed", _target.rotationSpeed);
	//	_target.ExaminingTransform = (Transform)EditorGUILayout.ObjectField ("Examining Object Transform", _target.ExaminingTransform, typeof(Transform));
		if (GUILayout.Button ("Create Model", GUILayout.Width (100f), GUILayout.Height (40f))) {
			_target.Create ();
		}


			if(GUI.changed)
				EditorUtility.SetDirty(_target); 
	}


}

}