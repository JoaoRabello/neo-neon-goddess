using GUIShadersPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(MaskableGraphic))]
public class GUIShaderSupport : MonoBehaviour
{
	public const string shaderSupportKeyword = "ShaderSupportEnabled";

	[Tooltip("Select this option to see proper effect in edit mode, note that this will change values in material")]
	public bool updateInEditMode = false;
	[Tooltip("Select when Image/Text will be changed frequently in runtime (like in animation)")]
	public bool willObjectChangeDuringRuntime = false;

	protected Image image;
	protected Text text;
	protected Material currentMaterial;
	protected GUIObjectInfo previousObjectInfo;
	protected RectTransform rectTrans;
	
	protected virtual void OnEnable()
	{
		if(rectTrans == null)
		{
			rectTrans = GetComponent<RectTransform>();
		}
		Refresh();
		Canvas.willRenderCanvases += OnCanvasPreRender;
	}

	protected void OnDisable()
	{
		Canvas.willRenderCanvases -= OnCanvasPreRender;
	}

	private void OnCanvasPreRender()
	{
		if(willObjectChangeDuringRuntime || !Application.isPlaying)
		{
			CheckForChange();
		}
	}
	
	public virtual void Refresh()
	{
#if UNITY_EDITOR
		if(!Application.isPlaying && !updateInEditMode)
			return;
#endif
		RefreshMaterial();

		if(currentMaterial == null)
			return;
		
		SaveObjectInfo();

		RefreshWorldPosUV();
		
	}

	protected virtual bool CheckForChange()
	{
		if(previousObjectInfo == null)
		{
			Refresh();
			return true;
		}

		if(previousObjectInfo.sprite != image.sprite)
		{
			Refresh();
			return true;
		}

		if(previousObjectInfo.position != rectTrans.position)
		{
			Refresh();
			return true;
		}

		rectTrans.GetWorldCorners(previousObjectInfo.rectVerticesTmp);
		Vector3 newRectSize = previousObjectInfo.rectVerticesTmp[2] - previousObjectInfo.rectVerticesTmp[0];

		if(newRectSize != previousObjectInfo.rectSize)
		{
			Refresh();
			return true;
		}

		return false;
	}

	private void SaveObjectInfo()
	{
		if(previousObjectInfo == null)
			previousObjectInfo = new GUIObjectInfo();

		if(image)
			previousObjectInfo.sprite = image.sprite;

		previousObjectInfo.position = rectTrans.position;
		rectTrans.GetWorldCorners(previousObjectInfo.rectVerticesTmp);
		previousObjectInfo.rectSize = previousObjectInfo.rectVerticesTmp[2] - previousObjectInfo.rectVerticesTmp[0];
	}

	private void RefreshMaterial()
	{
		if(image == null && text == null)
		{
			image = GetComponent<Image>();
			text = GetComponent<Text>();
		}
		if(image != null)
		{
			currentMaterial = image.material;
		}
		else if(text != null)
		{
			currentMaterial = image.material;
		}

		if(currentMaterial && Application.isPlaying && currentMaterial.IsKeywordEnabled(shaderSupportKeyword))
		{
			currentMaterial = Instantiate<Material>(currentMaterial);
			currentMaterial.EnableKeyword(shaderSupportKeyword);
		}
	}

	private void RefreshWorldPosUV()
	{
		var worldVertices = previousObjectInfo.rectVerticesTmp;
		currentMaterial.SetVector("_WorldPosToUV", new Vector4(worldVertices[0].x, worldVertices[0].y, previousObjectInfo.rectSize.x, previousObjectInfo.rectSize.y));
	}

	protected virtual void OnDrawGizmosSelected()
	{
		CheckForChange();
	}
}
