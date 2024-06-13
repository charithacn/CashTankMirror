using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    
    [SerializeField] private GameObject loadingScreen;

    private void Start()
    {
        loadingScreen.SetActive(false);
    }
    public void LoadSceneAsync(string sceneNameToLoad)
    {
        StartCoroutine(LoadAsync(sceneNameToLoad));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        loadingScreen.SetActive(true);
        // Begin loading the scene asynchronously
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName , LoadSceneMode.Single);

        // Don't allow the scene to be activated until it's fully loaded
        asyncOperation.allowSceneActivation = false;

        // While the scene is not yet loaded
        while (!asyncOperation.isDone)
        {
            // Output the current progress (value between 0 and 1)
            Debug.Log("Loading progress: " + asyncOperation.progress);

            // Optionally, you can add a loading bar or spinner to reflect the progress

            // Check if the load operation has completed (progress reaches 0.9)
            if (asyncOperation.progress >= 0.9f)
            {
                // Enable scene activation
                asyncOperation.allowSceneActivation = true;

                // Do any additional operations before scene activation if needed

                // Break out of the loop
                yield break;
            }

            yield return null; // Wait for the next frame
        }
    }
}
