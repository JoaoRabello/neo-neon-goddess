using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hack Ammo Data", menuName = "Combat/Hack Ammo Data")]
public class HackAmmoData : ScriptableObject
{
    public enum HackAmmoType
    {
        Paralysis,
        Override,
        Conversion,
        Control
    }

    public List<HackAmmoDamage> DamageData = new List<HackAmmoDamage>();

    public int GetAmmoDamageByType(HackAmmoType ammoType)
    {
        return DamageData.Find(data => data.Type.Equals(ammoType)).Damage;
    }
}

[Serializable]
public class HackAmmoDamage
{
    public HackAmmoData.HackAmmoType Type;
    public int Damage;
}
