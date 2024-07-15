using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Transform))]
public class ScaleUpAndDownTransform : MonoBehaviour
{
	public bool nowFadeIn = true;
	public float speed = 1f;
	public float targetScale = 100f;

	private Transform trans;

	protected void Start()
	{
		trans = transform;
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
		float progress = (trans.localScale.x - 1f) / (targetScale - 1f);
		float targetProgress = dir > 0 ? 1f : 0f;
		while(dir > 0 ? progress < 0.999f : progress > 0f)
		{
			yield return new WaitForEndOfFrame();
			progress += dir * Time.deltaTime * speed;
			float scale = 1f + progress * (targetScale - 1f);
			trans.localScale = new Vector3(scale, scale, scale);
		}

		float finalScale = 1f + targetProgress * (targetScale - 1f);
		trans.localScale = new Vector3(finalScale, finalScale, finalScale);
	}
}
