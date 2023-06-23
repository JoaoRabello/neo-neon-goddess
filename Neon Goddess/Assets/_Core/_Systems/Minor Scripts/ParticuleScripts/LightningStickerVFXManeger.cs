using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStickerVFXManeger : MonoBehaviour
{
    public AimSystem aimSystem;
   public ParticleSystem lightningStickerVFX;


    // Start is called before the first frame update
    public void EffectActivator()
    {
        bool isAiming = aimSystem.IsAiming;
        bool weaponEquipped = aimSystem.weaponEquipped;

        switch (isAiming, weaponEquipped) 
        {
            case (true, false):
            lightningStickerVFX.gameObject.SetActive(true);
            break;
            case (false, true):
            lightningStickerVFX.gameObject.SetActive(false);
            break;
            case (false, false):
            lightningStickerVFX.gameObject.SetActive(false);
            break;
        }

}

  
}
