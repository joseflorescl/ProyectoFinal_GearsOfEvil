using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem gunFireParticleSystem;

    public void GunFire()
    {
        gunFireParticleSystem.Play();
    }
}
