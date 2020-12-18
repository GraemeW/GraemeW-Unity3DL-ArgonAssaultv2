using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // Tunables
    [SerializeField] AudioClip introMusic = null;
    [SerializeField] AudioClip gameMusic = null;

    // Cached References
    AudioSource audioSource = null;

    private void Awake()
    {
        int numberOfMusicPlayers = FindObjectsOfType<MusicPlayer>().Length;
        if (numberOfMusicPlayers > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = introMusic;
        audioSource.Play();
    }

    public void PlayIntroMusic()
    {
        audioSource.clip = introMusic;
        audioSource.Play();
    }
    public void PlayGameMusic()
    {
        audioSource.clip = gameMusic;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
