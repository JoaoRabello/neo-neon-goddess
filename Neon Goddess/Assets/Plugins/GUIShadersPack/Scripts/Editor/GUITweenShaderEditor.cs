using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class GUITweenShaderEditor : ShaderGUI
{
	private Material material;
	private bool connectedToUpdateEvent;
	private GameObject currentSelection;
	private const float pivotHandleSize = 1f;

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{

		base.OnGUI(materialEditor, properties);

		material = materialEditor.target as Material;

		if(Selection.activeGameObject == null)
			return;

		currentSelection = Selection.activeGameObject;

		if(!connectedToUpdateEvent)
		{
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			SceneView.onSceneGUIDelegate += OnSceneGUI;
			connectedToUpdateEvent = true;
		}

		if(GUILayout.Button("Set _OperationsPivot to selected Transform position"))
		{
			Vector3 pos = Selection.activeGameObject.transform.position;
			material.SetVector("_OperationsPivot", new Vector4(pos.x, pos.y, 0f, 0f));
			EditorUtility.SetDirty(material);
		}

		if(GUILayout.Button("Center _OperationsPivot to selected Rect"))
		{
			RectTransform rect = Selection.activeGameObject.GetComponent<RectTransform>();
			Vector3[] corners = new Vector3[4];
			rect.GetWorldCorners(corners);
			Vector3 pos = (corners[0] + corners[2]) * 0.5f;
			material.SetVector("_OperationsPivot", new Vector4(pos.x, pos.y, 0f, 0f));
			EditorUtility.SetDirty(material);
		}
	}

	public void OnClose()
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
		connectedToUpdateEvent = false;
	}
	
	private void OnSceneGUI(SceneView sceneView)
	{
		if(material == null)
			return;

		if(Selection.activeGameObject != currentSelection)
		{
			OnClose();
			return;
		}

		Vector4 operationsPivot = material.GetVector("_OperationsPivot");
		Vector3 pivotPos = new Vector3(operationsPivot.x, operationsPivot.y, Selection.activeGameObject.transform.position.z);
		float handleSize = HandleUtility.GetHandleSize(pivotPos) * pivotHandleSize;
		Vector3 newPivotPos = Handles.FreeMoveHandle(pivotPos, Quaternion.identity, handleSize, Vector3.one, Handles.RectangleHandleCap);
#if UNITY_2021_1_OR_NEWER
		Handles.SphereHandleCap(0, pivotPos, Quaternion.identity, handleSize * 0.1f, EventType.Ignore);
#else
		Handles.DrawSphere(0, pivotPos, Quaternion.identity, handleSize * 0.1f);
#endif
		Handles.Label(pivotPos, "_OperationsPivot\n[" + material.name + " material]", EditorStyles.label);

		if(pivotPos.x != newPivotPos.x || pivotPos.y != newPivotPos.y)
		{
			Undo.RecordObject(material, "_OperationsPivot");
			material.SetVector("_OperationsPivot", new Vector4(newPivotPos.x, newPivotPos.y, 0f, 0f));
			EditorUtility.SetDirty(material);
		}
	}
}
