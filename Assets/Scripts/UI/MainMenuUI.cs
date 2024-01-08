using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private FadeScreenUI fadeScreen;

    private void Start()
    {
        fadeScreen.gameObject.SetActive(true);

        if (!SaveManager.instance.HasSaveData)
            Destroy(continueButton);
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadSceneWithFade(1.5f));
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteData();
        StartCoroutine(LoadSceneWithFade(1.5f));
    }

    public void ExitGame()
    {
        Debug.Log("Exiting...");
        Application.Quit();
    }

    private IEnumerator LoadSceneWithFade(float delay)
    {
        fadeScreen.FadeIn();
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
