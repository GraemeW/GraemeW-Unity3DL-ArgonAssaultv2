using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerWeaponController : MonoBehaviour
{
    // Tunables
    [Header("Guns, Lasers")]
    [SerializeField] GameObject guns = null;
    [SerializeField] float gunDamage = 1.0f;
    [SerializeField] AudioClip gunSound = null;
    [SerializeField] float gunSoundPeriod = 0.1f;
    [Header("Missiles")]
    [SerializeField] GameObject missile = null;
    [SerializeField] AudioClip missileSound = null;
    [SerializeField] int maxMissiles = 5;
    [SerializeField] float missileRefillPeriod = 4f;
    [Header("Admin")]
    [SerializeField] Transform spawnedObjectParent = null;

    // State
    bool isFiring = false;
    int currentMissileCount = 5;

    // Cached References
    ParticleSystem[] lasers;
    AudioSource audioSource = null;
    ScoreBoard scoreBoard = null;

    private void Start()
    {
        SetUpGuns();
        SetUpMissiles();
    }

    private void SetUpGuns()
    {
        lasers = GetComponentsInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = gunSound;
        ToggleGuns(false);
    }

    private void SetUpMissiles()
    {
        currentMissileCount = maxMissiles;
        scoreBoard = FindObjectOfType<ScoreBoard>();
        StartCoroutine(RefillMissiles());
    }

    void Update()
    {
        ProcessFiring();
    }

    private void ProcessFiring()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            ToggleGuns(true);
        }
        if (CrossPlatformInputManager.GetButtonUp("Fire1"))
        {
            ToggleGuns(false);
        }
        if (CrossPlatformInputManager.GetButtonDown("Fire2"))
        {
            FireMissile();
        }
    }

    private void ToggleGuns(bool toggleStatus)
    {
        foreach (Transform bulletSource in guns.transform)
        {
            if (toggleStatus)
            {
                bulletSource.GetComponent<ParticleSystem>().Play();
                isFiring = true;
                StartCoroutine(InitiateGunSounds());
            }
            else
            {
                bulletSource.GetComponent<ParticleSystem>().Stop();
                isFiring = false;
            }
        }
    }

    private IEnumerator InitiateGunSounds()
    {
        while (isFiring)
        {
            audioSource.Play();
            yield return new WaitForSeconds(gunSoundPeriod);
        }
    }

    public void EnableDisableLasers(bool setLasers)
    {
        foreach (ParticleSystem laser in lasers)
        {
            laser.gameObject.SetActive(setLasers);
        }
    }

    private void FireMissile()
    {
        if (currentMissileCount > 0)
        {
            currentMissileCount--;
            scoreBoard.UpdateMissileUI();
            GameObject missile = Instantiate(this.missile, transform.position, transform.rotation);
            missile.GetComponent<Missile>().SendMissileForward(transform.forward);
            missile.transform.parent = spawnedObjectParent;
            audioSource.PlayOneShot(missileSound);
        }
    }

    private IEnumerator RefillMissiles()
    {
        while (true)
        {
            if (currentMissileCount < maxMissiles)
            {
                currentMissileCount++;
                scoreBoard.UpdateMissileUI();
            }
            yield return new WaitForSeconds(missileRefillPeriod);
        }
    }

    public float GetGunDamage()
    {
        return gunDamage;
    }

    public int GetMissileCount()
    {
        return currentMissileCount;
    }
}
