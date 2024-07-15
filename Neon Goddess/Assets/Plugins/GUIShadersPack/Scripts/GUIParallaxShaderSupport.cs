using GUIShadersPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(MaskableGraphic))]
public class GUIParallaxShaderSupport : GUIShaderSupport
{
	public enum LookDirControlType
	{
		ManualOrScript, CameraLookDir, CameraPosDir, MousePositionLocal, MousePositionGlobal
	}

	public LookDirControlType lookDirControlType;
	public Camera observer;
	public Vector3 lookDir;//normalized world space overver look direction
	public Vector3 lookDirFactor = Vector3.one;//good for inverting look dir vector

	private Vector3 lastObserverPos;
	private Vector3 lastMousePos;

	protected override void OnEnable()
	{
		base.OnEnable();
		RefreshLookDir();
	}

	public override void Refresh()
	{
		base.Refresh();

#if UNITY_EDITOR
		if(!Application.isPlaying && !updateInEditMode)
			return;
#endif

		if(currentMaterial == null)
			return;

		RefreshLookDir();
	}

	protected override bool CheckForChange()
	{
		if(base.CheckForChange())
			return true;

		if(observer && lookDirControlType == LookDirControlType.CameraLookDir 
			|| lookDirControlType == LookDirControlType.CameraPosDir)
		{
			if(lastObserverPos != observer.transform.position)
			{
				Refresh();
				return true;
			}
		}
		else if(lookDirControlType == LookDirControlType.MousePositionGlobal
			|| lookDirControlType == LookDirControlType.MousePositionLocal)
		{
			if(lastMousePos != Input.mousePosition)
			{
				Refresh();
				return true;
			}
		}

		return false;
	}

	private void RefreshLookDir()
	{
		switch(lookDirControlType)
		{
			case LookDirControlType.ManualOrScript:
			{
				SetLookDir(lookDir);
				break;
			}

			case LookDirControlType.MousePositionGlobal:
			{
				RefreshMouseLookDir(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
				break;
			}

			case LookDirControlType.MousePositionLocal:
			{
				if(observer == null)
					break;
				RefreshMouseLookDir(observer.WorldToScreenPoint(rectTrans.position));
				break;
			}

			case LookDirControlType.CameraLookDir:
			{
				if(observer == null)
					break;
				lastObserverPos = observer.transform.position;
				SetLookDir(rectTrans.TransformDirection(-observer.transform.forward));
				break;
			}

			case LookDirControlType.CameraPosDir:
			{
				if(observer == null)
					break;
				lastObserverPos = observer.transform.position;
				Vector3 lookDir = lastObserverPos - rectTrans.position;
				SetLookDir(rectTrans.TransformDirection(lookDir.normalized));
				break;
			}
		}
	}

	private void RefreshMouseLookDir(Vector2 center)
	{
		Vector3 screenPos = Input.mousePosition;
#if UNITY_EDITOR
		if(!Application.isPlaying && updateInEditMode)
		{
			if(Event.current != null)
				screenPos = Event.current.mousePosition;
			else
			{
				SetLookDir(lookDir);
				return;
			}
		}
#endif
		lastMousePos = screenPos;
		Vector3 newLookDir = new Vector3((center.x - screenPos.x) / Screen.width * 2, -(center.y - screenPos.y) / Screen.height * 2, 0);
		float sqr = newLookDir.sqrMagnitude;
		if(sqr > 1)
		{
			newLookDir.z = 0.001f;
			newLookDir.Normalize();
		}
		else
			newLookDir.z = Mathf.Sqrt(1 - sqr);
		SetLookDir(newLookDir);
	}

	public void SetLookDir(Vector3 lookDirection)
	{
		lookDir = lookDirection;
		currentMaterial.SetVector("_LookDir", new Vector3(lookDir.x * lookDirFactor.x, lookDir.y * lookDirFactor.y, lookDir.z * lookDirFactor.z));
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();

#if UNITY_EDITOR
		if(!Application.isPlaying && !updateInEditMode)
			return;
#endif

		if(currentMaterial == null)
			return;

		RefreshLookDir();
	}
}
