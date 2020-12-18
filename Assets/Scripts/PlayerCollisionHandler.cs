using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class PlayerCollisionHandler : MonoBehaviour
{
    // Tunables
    [SerializeField] GameObject deathExplosion = null;
    [SerializeField] float health = 10;
    [SerializeField] int damageFlashCount = 5;
    [SerializeField] float damageFlashDelay = 0.5f;
    [SerializeField] float shakeIntensity = 0.1f;
    [SerializeField] int shakeNumber = 30;
    [SerializeField] float shakePeriod = 0.02f;
    [SerializeField] AudioClip damageSound = null;

    // State
    bool damageImmune = false;
    Color currentColor = Color.white;

    // Cached References
    PlayerMovement playerShip = null;
    PlayerWeaponController playerWeaponController = null;
    Camera gameCamera = null;
    SceneController sceneController = null;
    Renderer playerRenderer = null;
    Shield shield = null;
    ScoreBoard scoreBoard = null;

    private void Start()
    {
        playerShip = GetComponent<PlayerMovement>();
        playerWeaponController = GetComponent<PlayerWeaponController>();
        gameCamera = Camera.main;
        sceneController = FindObjectOfType<SceneController>();
        playerRenderer = GetComponent<Renderer>();
        shield = FindObjectOfType<Shield>();
        scoreBoard = FindObjectOfType<ScoreBoard>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SpeedBump>()) { return; } // No damage for speed bump screen (game progress)
        if (!damageImmune)
        {
            if (health > 1)
            {
                DecrementHealth();
            }
            else
            {
                StartPlayerDeathSequence();
            }
        }
    }

    private void DecrementHealth()
    {
        damageImmune = true;
        health--;
        scoreBoard.UpdateHealthUI();
        StartCoroutine(DamageShipFX());
        StartCoroutine(ShakeShipFX());
        if (health==1)
        {
            shield.gameObject.SetActive(false);
        }
        if (damageSound != null)
        {
            AudioSource.PlayClipAtPoint(damageSound, transform.position);
        }
    }

    private IEnumerator DamageShipFX()
    {
        for (int flashIndex = 0; flashIndex < damageFlashCount; flashIndex++)
        {
            if (currentColor == Color.white)
            {
                playerRenderer.material.color = Color.red;
            }
            else
            {
                playerRenderer.material.color = Color.white;
            }
            yield return new WaitForSeconds(damageFlashDelay);
        }
        playerRenderer.material.color = Color.white;
        damageImmune = false;
    }

    private IEnumerator ShakeShipFX()
    {
        for (int shakeIndex = 0; shakeIndex < shakeNumber; shakeIndex++)
        {
            Vector3 shakeOffset = new Vector3(Random.Range(-shakeIntensity, shakeIntensity), Random.Range(-shakeIntensity, shakeIntensity), 0f);
            transform.localPosition += shakeOffset;
            yield return new WaitForSeconds(shakePeriod);
        }
    }

    private void StartPlayerDeathSequence()
    {
        playerWeaponController.EnableDisableLasers(false); 
        playerShip.EnableDisableMovement(false); 
        if (deathExplosion != null)
        {
            Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2.0f);
            GameObject playerExplosion = Instantiate(deathExplosion, explosionPosition, transform.rotation);
            Destroy(playerExplosion, 0.7f);
        }
        sceneController.LoadGameOverScreen();
        gameObject.SetActive(false);
    }

    public float GetPlayerHealth()
    {
        return health;
    }
}
