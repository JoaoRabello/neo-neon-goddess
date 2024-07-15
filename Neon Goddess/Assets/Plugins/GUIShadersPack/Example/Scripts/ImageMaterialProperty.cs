using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Image material property setter usefull for Animation use
/// </summary>
[RequireComponent(typeof(Image))]
public class ImageMaterialProperty : MonoBehaviour
{
	public string materialPropertyName = "_Progress";
	public bool instantiateMaterial = true;

	[SerializeField] public float floatVal;
	[SerializeField] public Vector4 vectorVal;
	[SerializeField] public Color colorVal;

	private Image image;
	private float oldFloatVal;
	private Vector4 oldVectorVal;
	private Color oldColorVal;

	protected void Awake()
	{
		image = GetComponent<Image>();
		if(instantiateMaterial)
			image.material = new Material(image.material);
		oldFloatVal = floatVal;
		oldVectorVal = vectorVal;
		oldColorVal = colorVal;
	}

	protected void Update()
	{
		if(floatVal != oldFloatVal)
			image.material.SetFloat(materialPropertyName, oldFloatVal = floatVal);

		if(vectorVal != oldVectorVal)
			image.material.SetVector(materialPropertyName, oldVectorVal = vectorVal);

		if(colorVal != oldColorVal)
			image.material.SetColor(materialPropertyName, oldColorVal = colorVal);
	}
}
