using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AxlPlay;
using UnityEngine.UI;
namespace AxlPlay
{
	[CustomEditor(typeof(CameraSystem))]

	public class CameraSystemEditor : Editor
	{

		public override void OnInspectorGUI ()
		{
			CameraSystem _target = (CameraSystem)target;
			_target.CameraAudioSource = (AudioSource)EditorGUILayout.ObjectField ("Audio Source", _target.CameraAudioSource, typeof(AudioSource),true);
			_target.PlayerCamera = (Camera)EditorGUILayout.ObjectField ("Player Camera", _target.PlayerCamera, typeof(Camera), true);
					
			_target.ZoomInAnimation = (AnimationClip)EditorGUILayout.ObjectField ("Zoom In Camera Animation", _target.ZoomInAnimation, typeof(AnimationClip), true);
			_target.ZoomOutAnimation = (AnimationClip)EditorGUILayout.ObjectField ("Zoom Out Camera Animation", _target.ZoomOutAnimation, typeof(AnimationClip), true);

			_target.BrokenSound = (AudioClip)EditorGUILayout.ObjectField ("Broken Sound", _target.BrokenSound, typeof(AudioClip), true);

			if (_target.CanZoom)
				GUI.color = Color.green;
			else
				GUI.color = Color.white;

			if (GUILayout.Button ("Zoom", GUILayout.Width (70f), GUILayout.Height (40f))) {
				_target.CanZoom = !_target.CanZoom;
			}
			GUI.color = Color.white;

			_target.BrokenGlassesCount = EditorGUILayout.IntSlider ("Broken Glasses", _target.BrokenGlassesCount, 0, 100);

			if (_target.BrokenGlassesCount < _target.BrokenGlasses.Count){

				var temp = _target.BrokenGlasses.Count -1;
				_target.BrokenGlasses.RemoveAt(temp);
			}


			for (int i = 0; i < _target.BrokenGlassesCount; i++) {
				if (_target.BrokenGlassesCount > _target.BrokenGlasses.Count){

					_target.BrokenGlasses.Add(null);
				}else if (_target.BrokenGlassesCount == _target.BrokenGlasses.Count) {

					_target.BrokenGlasses[i] = (Image)EditorGUILayout.ObjectField("Broken Glass", _target.BrokenGlasses[i], typeof(Image), true);
				}

			}


			if (_target.CanZoom) {
				EditorGUILayout.BeginVertical ("box");
				_target.ZoomInSound = (AudioClip)EditorGUILayout.ObjectField ("Zoom In Sound", _target.ZoomInSound, typeof(AudioClip), true);
				_target.ZoomOutSound = (AudioClip)EditorGUILayout.ObjectField ("Zoom Out Sound", _target.ZoomOutSound, typeof(AudioClip), true);
				if (GUILayout.Button ("Add Animation To Camera", GUILayout.Width (170f), GUILayout.Height (40f))) {
					_target.AddAnimationToCamera ();
				}
				EditorGUILayout.EndVertical ();
			}
				
			if (GUILayout.Button ("Create Canvas", GUILayout.Width (100f), GUILayout.Height (40f))) {
				_target.LoadCanvas ();
			}
		


			if(GUI.changed)
				EditorUtility.SetDirty(_target); 
		}
	}

}
