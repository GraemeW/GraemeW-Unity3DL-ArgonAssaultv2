using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Tunables
    [SerializeField] float splashDelayTime = 2.5f;
    [SerializeField] float postDeathWaitBeforeGameOver = 1.5f;

    // States
    int currentSceneIndex = 0;

    private void Awake()
    {
        int numberOfSceneControllers = FindObjectsOfType<SceneController>().Length;
        if (numberOfSceneControllers > 1)
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
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0) // Splash screen
        {
            StartCoroutine(QueueNextScene(splashDelayTime));
        }
    }

    private IEnumerator QueueNextScene(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadGame()
    {
        FindObjectOfType<MusicPlayer>().PlayGameMusic(); // Call on function call, avoid headache re: awake/start handling
        SceneManager.LoadScene(2);
    }

    public void LoadGameOverScreen()
    {
        StartCoroutine(QueueGameOver());
    }

    private IEnumerator QueueGameOver()
    {
        yield return new WaitForSeconds(postDeathWaitBeforeGameOver);
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
