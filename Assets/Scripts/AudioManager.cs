using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("BGM Sounds")] 
    [SerializeField] private AudioSource bgmMusicAudioSource;
    [SerializeField] private AudioSource bgmEnvironmentAudioSource;
    [SerializeField] private AudioClip victorySong;

    [Header("Player Sounds")]
    [SerializeField] private AudioSource playerAudioSource;

    [SerializeField] private AudioClip[] playerFootstepsWalking;
    [SerializeField] private AudioClip[] playerFootstepsRunning;
    [SerializeField] private AudioClip[] playerVoiceJumping;
    [SerializeField] private AudioClip[] playerTakeDamage;
    [SerializeField] private AudioClip playerFallingJump;
    [SerializeField] private AudioClip playerGunFire;
    [SerializeField] private AudioClip playerGunNoAmmo;
    [SerializeField] private AudioClip playerDied;
    [SerializeField] private AudioClip playerGetHealth;
    [SerializeField] private AudioClip playerGetAmmo;
    [SerializeField] private float minPitchGunFire = 0.9f;
    [SerializeField] private float maxPitchGunFire = 1.1f;

    [Header("Enemy Sounds")]
    [SerializeField] private AudioClip[] enemyFootstepsWalking;
    [SerializeField] private AudioClip[] enemyFootstepsRunning;
    [SerializeField] private AudioClip[] enemyRoar;
    [SerializeField] private float minPitchEnemy = 0.8f;
    [SerializeField] private float maxPitchEnemy = 1.2f;
    [SerializeField] private AudioClip enemyTakeDamage;
    [SerializeField] private AudioClip enemyDied;
    [SerializeField] private AudioClip enemyAttack;
    [SerializeField] private AudioClip enemyVictory;

    [Header("Generic Sounds")]
    [SerializeField] private AudioClip[] characterFallingGround;

    public void PlayVictorySong()
    {
        bgmEnvironmentAudioSource.Stop();
        bgmMusicAudioSource.Stop();
        bgmMusicAudioSource.clip = victorySong;
        bgmMusicAudioSource.Play();
    }

    public void PlayerGunNoAmmo()
    {
        float pitch = Random.Range(minPitchGunFire, maxPitchGunFire); // Se usan las vars de pitch para el Gun Fire
        playerAudioSource.pitch = pitch;
        playerAudioSource.PlayOneShot(playerGunNoAmmo);
    }

    public void PlayerGetHealth()
    {
        playerAudioSource.PlayOneShot(playerGetHealth);
    }

    public void PlayerGetAmmo()
    {
        playerAudioSource.PlayOneShot(playerGetAmmo);
    }

    public void PlayerFallingGround()
    {
        int idx = Random.Range(0, characterFallingGround.Length);
        playerAudioSource.PlayOneShot(characterFallingGround[idx]);
    }

    public void PlayerFootstepWalking()
    {
        int idx = Random.Range(0, playerFootstepsWalking.Length);
        playerAudioSource.PlayOneShot(playerFootstepsWalking[idx]);
    }

    public void PlayerFootstepRunning()
    {
        int idx = Random.Range(0, playerFootstepsRunning.Length);
        playerAudioSource.PlayOneShot(playerFootstepsRunning[idx]);
    }

    public void PlayerFallingJump()
    {
        float pitch = Random.Range(minPitchGunFire, maxPitchGunFire); // Se usan las vars de pitch para el Gun Fire
        playerAudioSource.pitch = pitch;
        playerAudioSource.PlayOneShot(playerFallingJump);
    }

    public void PlayerVoiceJumping()
    {
        int idx = Random.Range(0, playerVoiceJumping.Length);
        playerAudioSource.PlayOneShot(playerVoiceJumping[idx]);
    }

    public void PlayerGunFire()
    {
        float pitch = Random.Range(minPitchGunFire, maxPitchGunFire);
        playerAudioSource.pitch = pitch;
        playerAudioSource.PlayOneShot(playerGunFire);
    }

    public void PlayerTakeDamage()
    {
        int idx = Random.Range(0, playerTakeDamage.Length);
        playerAudioSource.PlayOneShot(playerTakeDamage[idx]);
    }

    public void PlayerDied()
    {
        playerAudioSource.PlayOneShot(playerDied);
    }

    public void EnemyTakeDamage(AudioSource audioSource)
    {
        float pitch = Random.Range(minPitchEnemy, maxPitchEnemy);
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(enemyTakeDamage);
    }

    public void EnemyDied(AudioSource audioSource)
    {
        float pitch = Random.Range(minPitchEnemy, maxPitchEnemy);
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(enemyDied);
    }

    public void EnemyFootstepWalking(AudioSource audioSource)
    {
        int idx = Random.Range(0, enemyFootstepsWalking.Length);
        audioSource.PlayOneShot(enemyFootstepsWalking[idx]);
    }

    public void EnemyFootstepRunning(AudioSource audioSource)
    {
        int idx = Random.Range(0, enemyFootstepsRunning.Length);
        audioSource.PlayOneShot(enemyFootstepsRunning[idx]);
    }

    public void EnemyAttack(AudioSource audioSource)
    {
        float pitch = Random.Range(minPitchEnemy, maxPitchEnemy);
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(enemyAttack);
    }

    public void EnemyVictory(AudioSource audioSource)
    {
        audioSource.PlayOneShot(enemyVictory);
    }

    public void EnemyRoar(AudioSource audioSource)
    {
        int idx = Random.Range(0, enemyRoar.Length);
        audioSource.PlayOneShot(enemyRoar[idx]);
    }

    public void EnemyFallingGround(AudioSource audioSource)
    {
        int idx = Random.Range(0, characterFallingGround.Length);
        audioSource.PlayOneShot(characterFallingGround[idx]);
    }



}
