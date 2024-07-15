using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FadeInOutText : MonoBehaviour
{
	public bool nowFadeIn = true;
	public string materialProgressName = "_Progress";
	public float speed = 1f;

	private Text text;

	protected void Start()
	{
		text = GetComponent<Text>();
		text.material = new Material(text.material);
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
		float progress = text.material.GetFloat(materialProgressName);
		float targetProgress = dir > 0 ? 1f : 0f;
		while(dir > 0 ? progress < 0.999f : progress > 0f)
		{
			yield return new WaitForEndOfFrame();
			progress += dir * Time.deltaTime * speed;
			text.material.SetFloat(materialProgressName, progress);
		}

		text.material.SetFloat(materialProgressName, targetProgress);
	}
}
