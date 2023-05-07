using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCover : MonoBehaviour
{
    [SerializeField] private List<Collider> _hitBoxes = new List<Collider>();

    public List<Collider> HitBoxes => _hitBoxes;
}
