using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    // Tunables / State
    float shieldHitColorDuration = 2.5f;
    Color initialColor;

    // Cached references
    Renderer shieldRenderer = null;

    private void Start()
    {
        shieldRenderer = gameObject.GetComponent<Renderer>();
        initialColor = shieldRenderer.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(ShieldHit());
    }

    private IEnumerator ShieldHit()
    {
        shieldRenderer.material.color = Color.yellow * 0.5f;
        yield return new WaitForSeconds(shieldHitColorDuration);
        shieldRenderer.material.color = initialColor;
    }

}
