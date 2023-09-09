using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;
    
    [Header("Cameras")]
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _testCamera;
    
    [Header("UI")]
    [SerializeField] private GameObject _screenBarsContent;
    
    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }

    public void PlayItemCutscene(Camera itemCutsceneCamera)
    {
        PlayerStateObserver.Instance.OnCustsceneStart();
        
        _cameraManager.SelectCamera(itemCutsceneCamera);
        _screenBarsContent.SetActive(true);

        StartCoroutine(ItemCutscene());
    }
    
    private IEnumerator ItemCutscene()
    {
        yield return new WaitForSeconds(2);
        StopItemCutscene();
    }
    
    public void StopItemCutscene()
    {
        PlayerStateObserver.Instance.OnCustsceneEnd();
        
        _cameraManager.SelectCamera(_cameraManager.CurrentCamera);
        _screenBarsContent.SetActive(false);
    }
    
    public void PlayCutscene()
    {
        _cameraManager.SelectCamera(_testCamera);
        _screenBarsContent.SetActive(true);
    }

    public void StopCutscene()
    {
        _cameraManager.SelectCamera(_mainCamera);
        _screenBarsContent.SetActive(false);
    }
}
