/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AxlPlay;

namespace AxlPlay {
	[CustomEditor(typeof(LampSystem))]
public class LampSystemEditor : Editor {

	public override void OnInspectorGUI()
		{
			LampSystem _target = (LampSystem)target;
			_target.lightC = (Light)EditorGUILayout.ObjectField ("Light", _target.GetComponent<Light>(), typeof(Light),true);
			if (_target.FlickeringLight)
				GUI.color = Color.green;
			else
				GUI.color = Color.white;

			if (GUILayout.Button ("Flickering Light", GUILayout.Width (100f), GUILayout.Height (40f))) {
				_target.FlickeringLight = !_target.FlickeringLight;
			}
			GUI.color = Color.white;
			if (_target.FlickeringLight) {
				EditorGUILayout.BeginVertical ("box");

				_target.distortColor = EditorGUILayout.ColorField ("Distort Color", _target.distortColor);
				_target.AudioSource_Noise = (AudioSource)EditorGUILayout.ObjectField ("Audio Source Noise", _target.AudioSource_Noise, typeof(AudioSource), true);
				_target.Noise = (AudioClip)EditorGUILayout.ObjectField ("Noise Audio", _target.Noise, typeof(AudioClip), true);
				_target.blinkFrequency = EditorGUILayout.FloatField ("Blink Frequency", _target.blinkFrequency);
				EditorGUILayout.EndVertical();

			}

			_target.LampRenderer = (Renderer)EditorGUILayout.ObjectField ("Lamp Renderer", _target.LampRenderer, typeof(Renderer), true);

			_target.OnMaterial = (Material)EditorGUILayout.ObjectField ("On Material", _target.OnMaterial, typeof(Material), true);
			_target.OffMaterial = (Material)EditorGUILayout.ObjectField ("Off Material", _target.OffMaterial, typeof(Material), true);


			_target.IsOn = EditorGUILayout.Toggle ("On", _target.IsOn);



			_target.AudioSource_Switch = (AudioSource)EditorGUILayout.ObjectField ("Audio Source Switch", _target.AudioSource_Switch, typeof(AudioSource), true);
			_target.SwitchOn = (AudioClip)EditorGUILayout.ObjectField ("Switch On Audio", _target.SwitchOn, typeof(AudioClip), true);
			_target.SwitchOff = (AudioClip)EditorGUILayout.ObjectField ("Switch Off Audio", _target.SwitchOff, typeof(AudioClip), true);

			if(GUI.changed)
				EditorUtility.SetDirty(_target); 
		}
	}
}

*/