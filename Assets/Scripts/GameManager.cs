using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public int playerMaxLifes = 3;
    public Transform playerRespawn;
    public float secondsToRespawnPlayer = 7f;
    public float secondsToGameOverScreen = 10f;
    public float secondsToVictoryAnimation = 4f;
    public float secondsToVictoryPanel = 10f;
    public UIManager uiManager;
    public AudioManager audioManager;
    public SceneController sceneController;
    public GameObject victoryAnimation;
    public GameObject victoryPanel;

    [Header("Cameras")]
    public CinemachineVirtualCamera motionCam;
    public CinemachineVirtualCamera deathCam;
    public CinemachineVirtualCamera victoryCam;

    private PlayerWarriorController player;
    private int playerLifesRemaining = 0;
    private int totalEnemies;
    private int enemyCount;
    private EnemyController[] enemies;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWarriorController>();
        playerLifesRemaining = playerMaxLifes;

        SwitchToMotionCam();

        enemies = GameObject.FindObjectsOfType<EnemyController>();
        totalEnemies = enemies.Length;
        enemyCount = totalEnemies;

        uiManager.UpdateLifes(playerLifesRemaining);
        uiManager.UpdateEnemyCount(enemyCount);
    }

    public void OnPlayerDeath()
    {
        SwitchToDeathCam();
        playerLifesRemaining--;
        uiManager.UpdateLifes(playerLifesRemaining);

        if (playerLifesRemaining == 0)
        {
            Invoke(nameof(ShowGameOverScreen), secondsToGameOverScreen);
        }
        else
        {
            uiManager.ActivatePlayerDeath();
            Invoke(nameof(ResetCharacters), secondsToRespawnPlayer);
        }
    }

    public void OnEnemyDeath()
    {
        enemyCount--;
        uiManager.UpdateEnemyCount(enemyCount);
        if (enemyCount == 0)
        {
            Invoke(nameof(PlayerWins), secondsToVictoryAnimation);
        }

    }

    private void PlayerWins()
    {
        audioManager.PlayVictorySong();
        victoryAnimation.SetActive(true);
        SwitchToVictoryCam();
        player.SetVictoryState();
        Invoke(nameof(ShowVictoryPanel), secondsToVictoryPanel);
    }

    private void ShowVictoryPanel()
    {
        victoryPanel.SetActive(true);
    }
    


    private void ShowGameOverScreen()
    {
        sceneController.ShowGameOver();
    }

    private void ResetCharacters()
    {
        RespawnPlayer();
        RespawnEnemies();
    }

    private void RespawnPlayer()
    {
        uiManager.DeActivatePlayerDeath();
        player.ResetPlayerLife();
        player.transform.position = playerRespawn.position;
        player.transform.rotation = playerRespawn.rotation;
        SwitchToMotionCam();
    }

    private void RespawnEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.Health > 0)
            {
                enemy.ResetEnemyState();
            }
        }
    }

    private void SwitchToMotionCam()
    {
        motionCam.Priority = 10;
        deathCam.Priority = 0;
        victoryCam.Priority = 0;
    }

    private void SwitchToDeathCam()
    {
        motionCam.Priority = 0;
        deathCam.Priority = 10;
        victoryCam.Priority = 0;
    }

    private void SwitchToVictoryCam()
    {
        motionCam.Priority = 0;
        deathCam.Priority = 0;
        victoryCam.Priority = 10;
    }


}
