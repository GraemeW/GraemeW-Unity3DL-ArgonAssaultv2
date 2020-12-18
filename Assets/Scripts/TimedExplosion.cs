using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedExplosion : MonoBehaviour
{
    // Tunables
    [SerializeField] GameObject deathExplosion = null;
    [Tooltip("Random b/w 0 and time to wait before exploding")][SerializeField] float timeToExplode = 1.0f;
    [Tooltip("Time for explosion to finish, related to animation")][SerializeField] float deathExplosionTime = 0.7f;
    [SerializeField] Transform spawnedObjectParent = null;

    private void OnEnable()
    {
        StartCoroutine(QueueExplosion());
    }

    private IEnumerator QueueExplosion()
    {
        float waitTime = UnityEngine.Random.Range(0f, timeToExplode);
        yield return new WaitForSeconds(waitTime);
        GameObject enemyExplosion = Instantiate(deathExplosion, transform.position, Quaternion.identity);
        enemyExplosion.transform.parent = spawnedObjectParent;
        Destroy(enemyExplosion, deathExplosionTime);
    }
}
