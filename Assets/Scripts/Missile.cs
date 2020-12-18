using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Tunables
    [SerializeField] float instantaneousVelocity = 5000f;
    [SerializeField] float timeToExplode = 1.5f;
    [SerializeField] GameObject explosionVFX = null;
    [SerializeField] GameObject explosionCollider = null;

    // Tunables related to particle effect -- must match to explosion VFX!
    [SerializeField] float totalExplosionTime = 5.0f;
    [SerializeField] float timeToExplosionMaxSize = 1.0f;

    // State
    bool exploding = false;

    private void Start()
    {
        StartCoroutine(StartSelfDestructSequence());
    }

    public void SendMissileForward(Vector3 missileDirection)
    {
        gameObject.GetComponent<Rigidbody>().velocity = missileDirection * instantaneousVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }


    private IEnumerator StartSelfDestructSequence()
    {
        yield return new WaitForSeconds(timeToExplode);
        Explode();
    }

    private void Explode()
    {
        if (!exploding)
        {
            exploding = true;
            StopAndHideMissile();
            StartCoroutine(QueueExplosionCollider());
            Destroy(gameObject, totalExplosionTime);
        }
    }

    private void StopAndHideMissile()
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    private IEnumerator QueueExplosionCollider()
    {
        GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
        explosion.transform.parent = transform.parent;
        Destroy(explosion, totalExplosionTime);
        yield return new WaitForSeconds(timeToExplosionMaxSize);
        explosionCollider.SetActive(true);
    }
}
