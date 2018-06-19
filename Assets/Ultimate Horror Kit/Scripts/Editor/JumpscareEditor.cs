using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AxlPlay;


namespace AxlPlay {
	[CustomEditor(typeof(JumpScare))]

public class JumpscareEditor : Editor {

		public override void OnInspectorGUI()
		{
			JumpScare _target = (JumpScare)target;

			_target.Jumpscare = (GameObject)EditorGUILayout.ObjectField ("Jumpscare", _target.Jumpscare, typeof(GameObject), true);
			_target.LifeTime = EditorGUILayout.FloatField ("Life Time", _target.LifeTime);


			_target.audioSource = (AudioSource)EditorGUILayout.ObjectField ("Audio Source", _target.audioSource, typeof(AudioSource), true);
			_target.SpawnSound = (AudioClip)EditorGUILayout.ObjectField ("Spawn Sound", _target.SpawnSound, typeof(AudioClip), true);
			if (GUILayout.Button ("Create Jumpscare", GUILayout.Width (115f), GUILayout.Height (40f))) {

				_target.CreateJumpscare ();
			}

			if(GUI.changed)
				EditorUtility.SetDirty(_target); 
		}
}

}