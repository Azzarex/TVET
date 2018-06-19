using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AxlPlay;

namespace AxlPlay
{
    [CustomEditor(typeof(InteractiveObject))]

    public class InteractiveObjectEditor : Editor
    {

        
        public override void OnInspectorGUI()
        {
            InteractiveObject _target = (InteractiveObject)target;
            if (_target.ShowItemSettings)
                GUI.color = Color.green;
            else
                GUI.color = Color.white;

            if (GUILayout.Button("Show Item Settings", GUILayout.Width(130f), GUILayout.Height(40f)))
            {

                _target.ShowItemSettings = !_target.ShowItemSettings;
            }
            GUI.color = Color.white;
            _target.ItemName = EditorGUILayout.TextField("Name", _target.ItemName);

            if (_target.ShowItemSettings)
            {

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("Item Settings");
                _target.Description = EditorGUILayout.TextField("Description", _target.Description);
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.LabelField("Item");

            _target.Type = (InteractiveObject.Types)EditorGUILayout.EnumPopup("Type", _target.Type);

            if (_target.Type == InteractiveObject.Types.Consumable)
            {
                _target.Icon = (Sprite)EditorGUILayout.ObjectField("Icon", _target.Icon, typeof(Sprite), true);
                _target.InputToInteract = (string)EditorGUILayout.TextField("Input to Pick up", _target.InputToInteract);
                //_target.InputToUse = (KeyCode)EditorGUILayout.EnumPopup ("Key to Consume", _target.KeyToUse);
                _target.ConsumedSound = (AudioClip)EditorGUILayout.ObjectField("Consumed Sound", _target.ConsumedSound, typeof(AudioClip), true);
                _target.PickupedSound = (AudioClip)EditorGUILayout.ObjectField("Pickuped Sound", _target.PickupedSound, typeof(AudioClip), true);
                _target.ItemAudioSource = (AudioSource)EditorGUILayout.ObjectField("Item Audio Source", _target.ItemAudioSource, typeof(AudioSource), true);

                _target.HeightIcon = EditorGUILayout.FloatField("Height Of Icon", _target.HeightIcon);
                _target.WidthIcon = EditorGUILayout.FloatField("Width Of Icon", _target.WidthIcon);
                _target.ConsumedUI = (UIEffects)EditorGUILayout.ObjectField("Consumed UI", _target.ConsumedUI, typeof(UIEffects), true);
                
                if (_target.Examinable)
                    GUI.color = Color.green;
                else
                    GUI.color = Color.white;

                if (GUILayout.Button("Examinable", GUILayout.Width(80f), GUILayout.Height(40f)))
                {

                    _target.Examinable = !_target.Examinable;
                }
                GUI.color = Color.white;
                if (_target.Examinable)
                {
                    _target.InputToInteract = (string)EditorGUILayout.TextField("Input to Examine", _target.InputToInteract);
                    _target.StartExaminingSound = (AudioClip)EditorGUILayout.ObjectField("Start Examining Sound", _target.StartExaminingSound, typeof(AudioClip), true);
                    _target.StopExaminingSound = (AudioClip)EditorGUILayout.ObjectField("Stop Examining Sound", _target.StopExaminingSound, typeof(AudioClip), true);
                    _target.ExaminingUI = (UIEffects)EditorGUILayout.ObjectField("Examining UI", _target.ExaminingUI, typeof(UIEffects), true);

                }

            }
            if (_target.Type == InteractiveObject.Types.Usable)
            {
                _target.Icon = (Sprite)EditorGUILayout.ObjectField("Icon", _target.Icon, typeof(Sprite), true);
                _target.InputToInteract = (string)EditorGUILayout.TextField("Input to Pick up", _target.InputToInteract);
                //		_target.KeyToUse = (KeyCode)EditorGUILayout.EnumPopup ("Key to Use", _target.KeyToUse);
                _target.ItemAudioSource = (AudioSource)EditorGUILayout.ObjectField("Item Audio Source", _target.ItemAudioSource, typeof(AudioSource), true);
                _target.StartUsingSound = (AudioClip)EditorGUILayout.ObjectField("Start Using Sound", _target.StartUsingSound, typeof(AudioClip), true);
                _target.StopUsingSound = (AudioClip)EditorGUILayout.ObjectField("Stop Using Sound", _target.StopUsingSound, typeof(AudioClip), true);
                _target.PickupedSound = (AudioClip)EditorGUILayout.ObjectField("Pickuped Sound", _target.PickupedSound, typeof(AudioClip), true);
                _target.HeightIcon = EditorGUILayout.FloatField("Height Of Icon", _target.HeightIcon);
                _target.WidthIcon = EditorGUILayout.FloatField("Width Of Icon", _target.WidthIcon);
                _target.UsingUI = (UIEffects)EditorGUILayout.ObjectField("Using UI", _target.UsingUI, typeof(UIEffects), true);

                _target.Sway = EditorGUILayout.Toggle("Sway", _target.Sway);

                _target.Sway_Amount = EditorGUILayout.FloatField("Sway Amount", _target.Sway_Amount);
                _target.Sway_MaxAmount = EditorGUILayout.FloatField("Sway Maximum Amount", _target.Sway_MaxAmount);
                _target.Sway_SmoothAmount = EditorGUILayout.FloatField("Sway Smooth Amount", _target.Sway_SmoothAmount);
                _target.TiltX = EditorGUILayout.FloatField("Tilt X", _target.TiltX);
                _target.TiltY = EditorGUILayout.FloatField("Tilt Y", _target.TiltY);

                _target.Idle = (AnimationClip)EditorGUILayout.ObjectField("Idle Clip", _target.Idle, typeof(AnimationClip), true);
                _target.SightIdle = (AnimationClip)EditorGUILayout.ObjectField("Idle Sight Clip", _target.SightIdle, typeof(AnimationClip), true);
                _target.Walk = (AnimationClip)EditorGUILayout.ObjectField("Walk Clip", _target.Walk, typeof(AnimationClip), true);
                _target.SightWalk = (AnimationClip)EditorGUILayout.ObjectField("Walk Sight Clip", _target.SightWalk, typeof(AnimationClip), true);
                _target.Run = (AnimationClip)EditorGUILayout.ObjectField("Run Clip", _target.Run, typeof(AnimationClip), true);
                _target.SightRun = (AnimationClip)EditorGUILayout.ObjectField("Run Sight Clip", _target.SightRun, typeof(AnimationClip), true);
                _target.Shoot = (AnimationClip)EditorGUILayout.ObjectField("Shoot Clip", _target.Shoot, typeof(AnimationClip), true);
                _target.SightShoot = (AnimationClip)EditorGUILayout.ObjectField("Shoot Sight Clip", _target.SightShoot, typeof(AnimationClip), true);
                _target.SwitchInClip = (AnimationClip)EditorGUILayout.ObjectField("Switch In Clip", _target.SwitchInClip, typeof(AnimationClip), true);
                _target.SwitchOutClip = (AnimationClip)EditorGUILayout.ObjectField("Switch Out Clip", _target.SwitchOutClip, typeof(AnimationClip), true);
                _target.ReloadClip = (AnimationClip)EditorGUILayout.ObjectField("Reload Clip", _target.ReloadClip, typeof(AnimationClip), true);
                _target.JumpClip = (AnimationClip)EditorGUILayout.ObjectField("Jump Clip", _target.JumpClip, typeof(AnimationClip), true);
                _target.LandClip = (AnimationClip)EditorGUILayout.ObjectField("Land Clip", _target.LandClip, typeof(AnimationClip), true);

                _target.FPSArms = (GameObject)EditorGUILayout.ObjectField("FPS Arms", _target.FPSArms, typeof(GameObject), true);
                _target.ItemBasePos = EditorGUILayout.Vector3Field("Position Base", _target.ItemBasePos);
                _target.ItemBaseRot = EditorGUILayout.Vector3Field("Rotation Base", _target.ItemBaseRot);

                _target.runPosition = EditorGUILayout.Vector3Field("Run Position Base", _target.runPosition);
                _target.runRotation = EditorGUILayout.Vector3Field("Run Rotation Base", _target.runRotation);

                _target.runPosePosSpeed = EditorGUILayout.FloatField("Run Lerp Position Speed", _target.runPosePosSpeed);
                _target.runPoseRotSpeed = EditorGUILayout.FloatField("Run Lerp Rotation Speed", _target.runPoseRotSpeed);


                if (_target.Examinable)
                    GUI.color = Color.green;
                else
                    GUI.color = Color.white;

                if (GUILayout.Button("Examinable", GUILayout.Width(80f), GUILayout.Height(40f)))
                {

                    _target.Examinable = !_target.Examinable;
                }
                GUI.color = Color.white;
                if (_target.Examinable)
                {
                    _target.InputToExamine = (string)EditorGUILayout.TextField("Input to Examine", _target.InputToExamine);
                    _target.StartExaminingSound = (AudioClip)EditorGUILayout.ObjectField("Start Examining Sound", _target.StartExaminingSound, typeof(AudioClip), true);
                    _target.StopExaminingSound = (AudioClip)EditorGUILayout.ObjectField("Stop Examining Sound", _target.StopExaminingSound, typeof(AudioClip), true);
                    _target.ExaminingUI = (UIEffects)EditorGUILayout.ObjectField("Examining UI", _target.ExaminingUI, typeof(UIEffects), true);

                }
            }
            
            if (_target.Type == InteractiveObject.Types.OneClickInteraction)
            {

                _target.InputToInteract = (string)EditorGUILayout.TextField("Input to Interact", _target.InputToInteract);


            }

            if (_target.Type == InteractiveObject.Types.Examinable)
            {
                _target.InputToExamine = (string)EditorGUILayout.TextField("Input to Examine", _target.InputToExamine);
                _target.ItemAudioSource = (AudioSource)EditorGUILayout.ObjectField("Item Audio Source", _target.ItemAudioSource, typeof(AudioSource), true);
                _target.ExaminingUI = (UIEffects)EditorGUILayout.ObjectField("Examining UI", _target.ExaminingUI, typeof(UIEffects), true);
                _target.StartExaminingSound = (AudioClip)EditorGUILayout.ObjectField("Start Examining Sound", _target.StartExaminingSound, typeof(AudioClip), true);
                _target.StopExaminingSound = (AudioClip)EditorGUILayout.ObjectField("Stop Examining Sound", _target.StopExaminingSound, typeof(AudioClip), true);
            }


            if (GUI.changed)
                EditorUtility.SetDirty(_target);

        }
    }

}