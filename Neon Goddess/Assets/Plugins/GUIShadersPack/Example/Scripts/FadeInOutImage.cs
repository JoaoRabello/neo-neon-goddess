using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeInOutImage : MonoBehaviour
{
	public bool nowFadeIn = true;
	public string materialProgressName = "_Progress";
	public bool instantiateMaterial = true;
	public float speed = 1f;

	private Image image;

	protected void Start()
	{
		image = GetComponent<Image>();
		if(instantiateMaterial)
			image.material = new Material(image.material);
	}

	public void FadeInOrOut()
	{
		StopAllCoroutines();
		if(nowFadeIn)
			StartCoroutine(Fade(1f));
		else
			StartCoroutine(Fade(-1f));
		nowFadeIn = !nowFadeIn;
	}

	private IEnumerator Fade(float dir)
	{
		float progress = image.material.GetFloat(materialProgressName);
		float targetProgress = dir > 0 ? 1f : 0f;
		while(dir > 0 ? progress < 0.999f : progress > 0f)
		{
			yield return new WaitForEndOfFrame();
			progress += dir * Time.deltaTime * speed;
			image.material.SetFloat(materialProgressName, progress);
		}

		image.material.SetFloat(materialProgressName, targetProgress);
	}
}
