using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTMovie : MonoBehaviour 
{
    [System.Serializable]
    public class CTMovieValues
    {
        public bool IsActive;
        [Header("min, max emission bright")]
        public float MinBright = 1.5f;
        public float MaxBright = 3.0f;
        [Header("frames speed")]
        public float Speed;
        [Header("shared material")]
        public Material SharedMaterial;
        [Header("frames")]
        public Texture[] Frames;
        [HideInInspector]
        public int i;
    }
    public CTMovieValues[] _CTMovieValues;

    void Awake()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks * 1000);
    }

	// Use this for initialization
	void Start () 
    {
        for (int k0 = 0; k0 < _CTMovieValues.Length; k0++)
        {
            if (_CTMovieValues[k0].IsActive)
            {
                //bright
                float random = Random.Range(_CTMovieValues[k0].MinBright, _CTMovieValues[k0].MaxBright);
                //color variations
                float random2 = Random.Range(0.2f, 0.8f);
                float random3 = Random.Range(0.2f, 0.8f);
                float random4 = Random.Range(0.2f, 0.8f);
                _CTMovieValues[k0].SharedMaterial.SetColor("_EmissionColor", new Color(random + random2, random + random3, random + random4));
            }
            else
            {
                _CTMovieValues[k0].SharedMaterial.SetColor("_EmissionColor", Color.black);
            }
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        for (int k0 = 0; k0 < _CTMovieValues.Length; k0++)
        {
            if (_CTMovieValues[k0].IsActive)
            {
                _CTMovieValues[k0].i = (int)(Time.time * _CTMovieValues[k0].Speed);
                _CTMovieValues[k0].i = _CTMovieValues[k0].i % _CTMovieValues[k0].Frames.Length;
                _CTMovieValues[k0].SharedMaterial.SetTexture("_EmissionMap", _CTMovieValues[k0].Frames[_CTMovieValues[k0].i]);   //for hdrp: _EmissiveColorMap
            }
        }
	}

    void OnDestroy()
    {
        for (int k0 = 0; k0 < _CTMovieValues.Length; k0++)
        {
            if (_CTMovieValues[k0].IsActive)
            {
                //_CTMovieValues[k0].SharedMaterial.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}
