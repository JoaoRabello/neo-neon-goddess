using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorShaderScript : MonoBehaviour
{
    public bool doorStatus = false;
    public GameObject doorTrigger;
    public Renderer doorShaderRenderer;
    public float doorEmissionIntensity;
    public Light doorLight;
    public Light doorLight2;


    void DoorUnlocked()
    {
        doorShaderRenderer.materials[1].SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.0f));
        doorShaderRenderer.materials[1].SetColor("_EmissionColor", new Color(0.0f, 4342.935f, 0.0f, 1.0f));
        doorLight.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        doorLight2.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        doorShaderRenderer.materials[2].SetInt("_Status", 1);
        doorTrigger.gameObject.SetActive(true);
    }

    void DoorLocked()
    {
        doorShaderRenderer.materials[1].SetColor("_Color", new Color(1.0f, 0.0f, 0.0f, 0.0f));
        doorShaderRenderer.materials[1].SetColor("_EmissionColor", new Color(4342.935f, 0.0f, 0.0f, 1.0f));
        doorLight.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        doorLight2.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        doorShaderRenderer.materials[2].SetInt("_Status", 0);
    }

    // Update is called once per frame
    void Update()
    {
        switch (doorStatus)
        {
            case true:
                DoorUnlocked();
                break;
            case false:
                DoorLocked();
                break;
        }
        }
}
