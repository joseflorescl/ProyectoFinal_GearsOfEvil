using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWarriorTitleScreen : MonoBehaviour
{
    [Header("Managers")]
    public AudioManager audioManager;

    public void GunFire()
    {
        audioManager.PlayerGunFire();
    }

}
