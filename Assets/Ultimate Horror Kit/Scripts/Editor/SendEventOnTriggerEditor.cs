using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AxlPlay;

namespace AxlPlay {
	
	[CustomEditor(typeof(SendEventOnTrigger))]

	public class SendEventOnTriggerEditor : Editor {

		public SerializedObject serialObj;
		public SerializedProperty OnTriggerEnterActionsSerial;
		public SerializedProperty OnTriggerStayActionsSerial;
		public SerializedProperty OnTriggerExitActionsSerial;

		void OnEnable() {
			SendEventOnTrigger _target = (SendEventOnTrigger)target;
			serialObj = new SerializedObject (_target);
			OnTriggerEnterActionsSerial = serialObj.FindProperty("OnTriggerEnterEvent");
			OnTriggerStayActionsSerial = serialObj.FindProperty("OnTriggerStayEvent");
			OnTriggerExitActionsSerial = serialObj.FindProperty("OnTriggerExitEvent");

		}



		public override void OnInspectorGUI()
		{
			SendEventOnTrigger _target = (SendEventOnTrigger)target;
			_target.Tag = EditorGUILayout.TagField ("Tag", _target.Tag);

			// Turning this Node into a serialized Object to access its properties
			if (serialObj != null) serialObj.Update();

			EditorGUILayout.BeginVertical ();

			EditorGUILayout.PropertyField(OnTriggerEnterActionsSerial);
			_target.EnterEventCanRepeat =	EditorGUILayout.Toggle ( "Can send event more than one time?",_target.EnterEventCanRepeat);

			EditorGUILayout.PropertyField(OnTriggerStayActionsSerial);
			_target.StayEventCanRepeat = EditorGUILayout.Toggle ("Can send event more than one time?", _target.StayEventCanRepeat);

			EditorGUILayout.PropertyField(OnTriggerExitActionsSerial);
			_target.ExitEventCanRepeat = EditorGUILayout.Toggle ("Can send event more than one time?", _target.ExitEventCanRepeat);


			EditorGUILayout.EndVertical (); 

			serialObj.ApplyModifiedProperties();

		}
}

}