﻿using UnityEngine;
using System.Collections.Generic;

namespace AxlPlay
{
	public class MovementUtility
	{
		private static Dictionary<GameObject, AudioSource[]> transformAudioSourceMap;
		
        // Cast a sphere with the desired distance. Check each collider hit to see if it is within the field of view. Set objectFound
        // to the object that is most directly in front of the agent
		public static Collider[] WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, LayerMask objectLayerMask, Vector3 targetOffset, LayerMask ignoreLayerMask)
		{

			var hitColliders = Physics.OverlapSphere(transform.position, viewDistance, objectLayerMask);
			if (hitColliders != null)
			{
				float minAngle = Mathf.Infinity;
				for (int i = 0; i < hitColliders.Length; ++i)
				{
					float angle;
                    // Call the WithinSight function to determine if this specific object is within sight
					if (( WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, hitColliders[i].gameObject, targetOffset, false, 0, out angle, ignoreLayerMask)) != null)
					{
                        // This object is within sight. Set it to the objectFound GameObject if the angle is less than any of the other objects
						if (angle < minAngle)
						{
							minAngle = angle;
						}
					}
				}
			}
			return hitColliders;
		}
		
        // Cast a circle with the desired distance. Check each collider hit to see if it is within the field of view. Set objectFound
        // to the object that is most directly in front of the agent
		public static GameObject WithinSight2D(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, LayerMask objectLayerMask, Vector3 targetOffset, float angleOffset2D, LayerMask ignoreLayerMask)
		{
			GameObject objectFound = null;
			var hitColliders = Physics2D.OverlapCircleAll(transform.position, viewDistance, objectLayerMask);
			if (hitColliders != null)
			{
				float minAngle = Mathf.Infinity;
				for (int i = 0; i < hitColliders.Length; ++i)
				{
					float angle;
					GameObject obj;
                    // Call the 2D WithinSight function to determine if this specific object is within sight
					if ((obj = WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, hitColliders[i].gameObject, targetOffset, true, angleOffset2D, out angle, ignoreLayerMask)) != null)
					{
                        // This object is within sight. Set it to the objectFound GameObject if the angle is less than any of the other objects
						if (angle < minAngle)
						{
							minAngle = angle;
							objectFound = obj;
						}
					}
				}
			}
			return objectFound;
		}
		
        // Public helper function that will automatically create an angle variable that is not used. This function is useful if the calling object doesn't
        // care about the angle between transform and targetObject
		public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, LayerMask ignoreLayerMask)
		{
			float angle;
			return WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, targetObject, targetOffset, false, 0, out angle, ignoreLayerMask);
		}
		
        // Public helper function that will automatically create an angle variable that is not used. This function is useful if the calling object doesn't
        // care about the angle between transform and targetObject
		public static GameObject WithinSight2D(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, float angleOffset2D, LayerMask ignoreLayerMask)
		{
			float angle;
			return WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, targetObject, targetOffset, true, angleOffset2D, out angle, ignoreLayerMask);
		}
		
        // Determines if the targetObject is within sight of the transform. It will set the angle regardless of whether or not the object is within sight
		private static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, bool usePhysics2D, float angleOffset2D, out float angle, int ignoreLayerMask)
		{
            // The target object needs to be within the field of view of the current object
			var direction = targetObject.transform.position - transform.TransformPoint(positionOffset);
			if (usePhysics2D)
			{
				angle = Vector3.Angle(direction, transform.up) + angleOffset2D;
				direction.z = 0;
			}
			else
			{
				angle = Vector3.Angle(direction, transform.forward);
				direction.y = 0;
			}
			if (direction.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f)
			{
                // The hit agent needs to be within view of the current agent
				if (LineOfSight(transform, positionOffset, targetObject, targetOffset, usePhysics2D, ignoreLayerMask) != null)
				{
					return targetObject; // return the target object meaning it is within sight
				}
				else if (targetObject.GetComponent<Collider>() == null && targetObject.GetComponent<Collider2D>() == null)
				{
                    // If the linecast doesn't hit anything then that the target object doesn't have a collider and there is nothing in the way
					if (targetObject.gameObject.activeSelf)
						return targetObject;
				}
			}
            // return null if the target object is not within sight
			return null;
		}
		
		public static GameObject LineOfSight(Transform transform, Vector3 positionOffset, GameObject targetObject, Vector3 targetOffset, bool usePhysics2D, int ignoreLayerMask)
		{
			if (usePhysics2D)
			{
				RaycastHit2D hit;
				if ((hit = Physics2D.Linecast(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), ~ignoreLayerMask)))
				{
					if (hit.transform.IsChildOf(targetObject.transform) || targetObject.transform.IsChildOf(hit.transform))
					{
						return targetObject; // return the target object meaning it is within sight
					}
				}
			}
			else
			{
				RaycastHit hit;
				if (Physics.Linecast(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), out hit, ~ignoreLayerMask))
				{
					if (hit.transform.IsChildOf(targetObject.transform) || targetObject.transform.IsChildOf(hit.transform))
					{
						return targetObject; // return the target object meaning it is within sight
					}
				}
			}
			return null;
		}
		
        // Cast a sphere with the desired radius. Check each object's audio source to see if audio is playing. If audio is playing
        // and its audibility is greater than the audibility threshold then return the object heard
		public static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, float hearingRadius, LayerMask objectLayerMask)
		{
			GameObject objectHeard = null;
			var hitColliders = Physics.OverlapSphere(transform.TransformPoint(positionOffset), hearingRadius, objectLayerMask);
			if (hitColliders != null)
			{
				float maxAudibility = 0;
				for (int i = 0; i < hitColliders.Length; ++i)
				{
					float audibility = 0;
					GameObject obj;
                    // Call the WithinHearingRange function to determine if this specific object is within hearing range
					if ((obj = WithinHearingRange(transform, positionOffset, audibilityThreshold, hitColliders[i].gameObject, ref audibility)) != null)
					{
                        // This object is within hearing range. Set it to the objectHeard GameObject if the audibility is less than any of the other objects
						if (audibility > maxAudibility)
						{
							maxAudibility = audibility;
							objectHeard = obj;
						}
					}
				}
			}
			return objectHeard;
		}
		
        // Cast a circle with the desired radius. Check each object's audio source to see if audio is playing. If audio is playing
        // and its audibility is greater than the audibility threshold then return the object heard
		public static GameObject WithinHearingRange2D(Transform transform, Vector3 positionOffset, float audibilityThreshold, float hearingRadius, LayerMask objectLayerMask)
		{
			GameObject objectHeard = null;
			var hitColliders = Physics2D.OverlapCircleAll(transform.TransformPoint(positionOffset), hearingRadius, objectLayerMask);
			if (hitColliders != null)
			{
				float maxAudibility = 0;
				for (int i = 0; i < hitColliders.Length; ++i)
				{
					float audibility = 0;
					GameObject obj;
                    // Call the WithinHearingRange function to determine if this specific object is within hearing range
					if ((obj = WithinHearingRange(transform, positionOffset, audibilityThreshold, hitColliders[i].gameObject, ref audibility)) != null)
					{
                        // This object is within hearing range. Set it to the objectHeard GameObject if the audibility is less than any of the other objects
						if (audibility > maxAudibility)
						{
							maxAudibility = audibility;
							objectHeard = obj;
						}
					}
				}
			}
			return objectHeard;
		}
		
        // Public helper function that will automatically create an audibility variable that is not used. This function is useful if the calling call doesn't
        // care about the audibility value
		public static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, GameObject targetObject)
		{
			float audibility = 0;
			return WithinHearingRange(transform, positionOffset, audibilityThreshold, targetObject, ref audibility);
		}
		
		private static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, GameObject targetObject, ref float audibility)
		{
			AudioSource[] colliderAudioSource;
            // Check to see if the hit agent has an audio source and that audio source is playing
			if ((colliderAudioSource = GetAudioSources(targetObject)) != null)
			{
				for (int i = 0; i < colliderAudioSource.Length; ++i)
				{
					if (colliderAudioSource[i].isPlaying)
					{
						var distance = Vector3.Distance(transform.position, targetObject.transform.position);
						if (colliderAudioSource[i].rolloffMode == AudioRolloffMode.Logarithmic)
						{
							audibility = colliderAudioSource[i].volume / Mathf.Max(colliderAudioSource[i].minDistance, distance - colliderAudioSource[i].minDistance);
						}
						else
						{ // linear
							audibility = colliderAudioSource[i].volume * Mathf.Clamp01((distance - colliderAudioSource[i].minDistance) / (colliderAudioSource[i].maxDistance - colliderAudioSource[i].minDistance));
						}
						if (audibility > audibilityThreshold)
						{
							return targetObject;
						}
					}
				}
			}
			return null;
		}
		
        // Draws the line of sight representation
		public static void DrawLineOfSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float angleOffset, float viewDistance, bool usePhysics2D)
		{
#if UNITY_EDITOR
			var oldColor = UnityEditor.Handles.color;
			var color = Color.yellow;
			color.a = 0.1f;
			UnityEditor.Handles.color = color;
			
			var halfFOV = fieldOfViewAngle * 0.5f + angleOffset;
			var beginDirection = Quaternion.AngleAxis(-halfFOV, (usePhysics2D ? Vector3.forward : Vector3.up)) * (usePhysics2D ? transform.up : transform.forward);
			UnityEditor.Handles.DrawSolidArc(transform.TransformPoint(positionOffset), (usePhysics2D ? transform.forward : transform.up), beginDirection, fieldOfViewAngle, viewDistance);
			
			UnityEditor.Handles.color = oldColor;
#endif
		}
		
        // Caches the AudioSource GetComponents for quick lookup
		private static AudioSource[] GetAudioSources(GameObject target)
		{
			if (transformAudioSourceMap == null)
			{
				transformAudioSourceMap = new Dictionary<GameObject, AudioSource[]>();
			}
			
			AudioSource[] audioSources;
			if (transformAudioSourceMap.TryGetValue(target, out audioSources))
			{
				return audioSources;
			}
			
			audioSources = target.GetComponentsInChildren<AudioSource>();
			transformAudioSourceMap.Add(target, audioSources);
			return audioSources;
		}
	}
}
 