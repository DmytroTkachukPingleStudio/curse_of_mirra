using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using TMPro;

public class HealthBarItem : MonoBehaviour
{
    [SerializeField]
    Sprite healthBarGreen,
        healthBarRed,
        yourLose,
        enemyLose,
        yourBackground,
        enemyBackground;
    
    [SerializeField]
    Image front,
        damage,
        background;

    [SerializeField]
    TextMeshProUGUI healthBarAmount;

    public void SetYourHealthBar(ulong health) {
        front.sprite = healthBarGreen;
        damage.sprite = yourLose;
        background.sprite = yourBackground;
        healthBarAmount.text = health + "";
    }

    public void SetEnemyCrateHealthBar(ulong health) {
        front.sprite = healthBarRed;
        damage.sprite = enemyLose;
        background.sprite = enemyBackground;
        healthBarAmount.text = health + "";
    }

    public void UpdateHealthBar(float health) {
        healthBarAmount.text = health + "";
    }
}
