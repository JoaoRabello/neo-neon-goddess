using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GUIParallaxShaderSupport))]
public class GUIParallaxShaderSupportEditor : Editor
{
	private GUIParallaxShaderSupport parallaxObject;

	public override void OnInspectorGUI()
	{
		parallaxObject = target as GUIParallaxShaderSupport;
		if(!parallaxObject)
			return;

		EditorGUI.BeginChangeCheck();

		parallaxObject.updateInEditMode = EditorGUILayout.Toggle(new GUIContent("Update in edit mode*", "Select this option to see proper effect in edit mode, note that this will change values in material"), parallaxObject.updateInEditMode);
		parallaxObject.willObjectChangeDuringRuntime = EditorGUILayout.Toggle(new GUIContent("Will object change during runtime*", "Select when Image/Text will be changed frequently in runtime (like in animation)"), parallaxObject.willObjectChangeDuringRuntime);

		parallaxObject.lookDirControlType = (GUIParallaxShaderSupport.LookDirControlType)EditorGUILayout.EnumPopup("Look dir control", parallaxObject.lookDirControlType);
		parallaxObject.lookDirFactor = EditorGUILayout.Vector3Field(new GUIContent("Look dir factor*", "Allows to change intensity of certain movement on totaly invert them"), parallaxObject.lookDirFactor);
		if(parallaxObject.lookDirControlType == GUIParallaxShaderSupport.LookDirControlType.ManualOrScript)
		{
			parallaxObject.lookDir = EditorGUILayout.Vector3Field(new GUIContent("Look direction*", "Fake observator look direction in world space"), parallaxObject.lookDir);
			parallaxObject.lookDir.Normalize();
		}
		else
		{
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.Vector3Field(new GUIContent("Look direction*", "Fake observator look direction in world space"), parallaxObject.lookDir);
			EditorGUILayout.Vector3Field(new GUIContent("Look dir with factor", "Fake observator look direction in world space"), 
				new Vector3(parallaxObject.lookDir.x * parallaxObject.lookDirFactor.x, parallaxObject.lookDir.y * parallaxObject.lookDirFactor.y, parallaxObject.lookDir.z * parallaxObject.lookDirFactor.z));
			EditorGUI.EndDisabledGroup();
		}

		
		if(parallaxObject.lookDirControlType != GUIParallaxShaderSupport.LookDirControlType.ManualOrScript
			&& parallaxObject.lookDirControlType != GUIParallaxShaderSupport.LookDirControlType.MousePositionGlobal)
		{
			parallaxObject.observer = EditorGUILayout.ObjectField("Observer", parallaxObject.observer, typeof(Camera), true) as Camera;
		}


		if(EditorGUI.EndChangeCheck() && parallaxObject)
		{
			parallaxObject.Refresh();
			EditorUtility.SetDirty(parallaxObject);
		}

	}
}
