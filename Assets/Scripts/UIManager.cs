using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Image imagePlayerDeath;
    public float timeEffectImagePlayerDeath = 5f;
    public float maxAlphaImagePlayerDeath = 0.8f;
    public Image healthBarImage;
    public TextMeshProUGUI healthBarValueText;
    public TextMeshProUGUI lifesValueText;
    public TextMeshProUGUI ammoValueText;
    public TextMeshProUGUI enemyCountValueText;

    private bool isPlayerDeathActive = false;
    private float progressLerpColor = 0;
    

    private void Start()
    {
        isPlayerDeathActive = false;
        progressLerpColor = 0;
    }

    public void Update()
    {
        if (isPlayerDeathActive)
        {        
            if (progressLerpColor <= 1)
            {
                Color color = imagePlayerDeath.color;
                color.a = Mathf.Lerp(0, maxAlphaImagePlayerDeath, progressLerpColor);
                imagePlayerDeath.color = color;
                progressLerpColor += Time.deltaTime / timeEffectImagePlayerDeath;
            }
            
        }
    }

    public void ActivatePlayerDeath()
    {
        progressLerpColor = 0;
        isPlayerDeathActive = true;
    }

    public void DeActivatePlayerDeath()
    {
        isPlayerDeathActive = false;
        Color color = imagePlayerDeath.color;
        color.a = 0;
        imagePlayerDeath.color = color;
    }

    public void UpdateHealth(float value) // Valor normalizado
    {
        healthBarImage.fillAmount = value;
        healthBarValueText.text = Mathf.Round(value * 100f).ToString();
    }

    public void UpdateLifes(int value)
    {
        lifesValueText.text = value.ToString();
    }

    public void UpdateAmmo(int value)
    {
        ammoValueText.text = value.ToString();
    }

    public void UpdateEnemyCount(int value)
    {
        enemyCountValueText.text = value.ToString();
    }
}
