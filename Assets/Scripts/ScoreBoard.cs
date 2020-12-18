using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    // Tunables
    [SerializeField] TextMeshProUGUI health = null;
    [SerializeField] TextMeshProUGUI score = null;
    [SerializeField] TextMeshProUGUI missileCount = null;

    // State
    int scoreValue = 0;

    // Cached References
    PlayerCollisionHandler playerCollisionHandler = null;
    PlayerWeaponController playerWeaponController = null;

    private void Start()
    {
        playerCollisionHandler = FindObjectOfType<PlayerCollisionHandler>();
        playerWeaponController = FindObjectOfType<PlayerWeaponController>();
        UpdateHealthUI();
        IncrementScore(0);
        UpdateMissileUI();
    }

    public void IncrementScore(int scoreAdder)
    {
        scoreValue += scoreAdder;
        score.text = scoreValue.ToString();
    }

    public void UpdateHealthUI()
    {
        health.text = playerCollisionHandler.GetPlayerHealth().ToString();
    }

    public void UpdateMissileUI()
    {
        missileCount.text = playerWeaponController.GetMissileCount().ToString();
    }
}
