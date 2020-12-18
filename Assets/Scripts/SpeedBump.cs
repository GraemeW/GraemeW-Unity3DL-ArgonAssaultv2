using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBump : MonoBehaviour
{
    [SerializeField] float speedMultiplierOnRound = 1.25f;

    private void OnTriggerEnter(Collider other)
    {
        Time.timeScale *= speedMultiplierOnRound;
    }
}
