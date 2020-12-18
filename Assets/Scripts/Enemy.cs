using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    static readonly string LAYER_PLAYER_PROJECTILE = "PlayerProjectile";

    // Tunables
    [SerializeField] GameObject deathExplosion = null;
    [SerializeField] float deathExplosionTime = 0.7f;
    [SerializeField] float healthPoints = 3.0f;
    [SerializeField] int scorePoints = 50;
    [SerializeField] bool boss = false;
    [SerializeField] float bossMissileImpact = 5.0f; // damage on missile hits for boss handled differently

    // State
    float currentHealthPoints = 3.0f;

    // Cached References
    PlayerWeaponController playerWeaponController = null;
    Rigidbody enemyRigidBody = null;
    [SerializeField] Transform spawnedObjectParent = null;
    ScoreBoard scoreBoard = null;

    private void Start()
    {
        SetUpRigidbody();

        playerWeaponController = FindObjectOfType<PlayerWeaponController>();
        scoreBoard = FindObjectOfType<ScoreBoard>();
    }

    private void OnEnable()
    {
        currentHealthPoints = healthPoints;
    }

    private void SetUpRigidbody()
    {
        gameObject.AddComponent<MeshCollider>();
        enemyRigidBody = gameObject.AddComponent<Rigidbody>();
        enemyRigidBody.isKinematic = true;
        enemyRigidBody.useGravity = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        HandleGunHits();
    }

    private void HandleGunHits()
    {
        currentHealthPoints -= playerWeaponController.GetGunDamage();
        if (currentHealthPoints <= 0)
        {
            scoreBoard.IncrementScore(scorePoints);
            StartEnemyDeathSequence();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleBoss(other);
        HandleEnemy(other);
    }

    private void HandleBoss(Collider other)
    {
        if (boss)
        {
            if (other.GetComponent<PlayerMovement>() || other.gameObject.layer == LayerMask.NameToLayer(LAYER_PLAYER_PROJECTILE))
            {
                currentHealthPoints -= bossMissileImpact;
                if (currentHealthPoints <= 0)
                {
                    scoreBoard.IncrementScore(scorePoints);
                    StartEnemyDeathSequence();
                }
            }
        }
    }

    private void HandleEnemy(Collider other)
    {
        if (boss) { return; }

        // Player collision handling
        if (other.GetComponent<PlayerMovement>())
        {
            scoreBoard.IncrementScore(scorePoints);
            StartEnemyDeathSequence();
        }

        // Missile handling
        if (other.gameObject.layer == LayerMask.NameToLayer(LAYER_PLAYER_PROJECTILE))
        {
            scoreBoard.IncrementScore(scorePoints);
            StartEnemyDeathSequence();
        }
    }

    private void StartEnemyDeathSequence()
    {
        if (deathExplosion != null)
        {
            Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2.0f);
            GameObject enemyExplosion = Instantiate(deathExplosion, explosionPosition, Quaternion.identity);
            enemyExplosion.transform.parent = spawnedObjectParent;
            Destroy(enemyExplosion, deathExplosionTime);
        }
        gameObject.SetActive(false);
        //Destroy(gameObject, deathExplosionTime);  -- avoid destroying so can re-activate on continuous loops (via WaveLogic)
    }
}
