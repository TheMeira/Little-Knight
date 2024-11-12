using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Button[] buttonLevels;
    public bool menu;

    public GameObject creditsPanel;
    [SerializeField] private float creditLength;
    [SerializeField] private AudioSource buttonSound;

    public string videoName;
    void Start()
    {
        if (FindObjectOfType<DontDestoryOnLoad>())
            FindObjectOfType<DontDestoryOnLoad>().transform.GetChild(0).gameObject.SetActive(false);
        if (menu)
        {
            int levelAt = PlayerPrefs.GetInt("LevelUnlocked", 1);
            for (int i = 0; i < buttonLevels.Length; i++)
            {
                if (i + 1 > levelAt)
                    buttonLevels[i].interactable = false;
            }
        }

    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        Invoke("CloseCredits", creditLength);
    }
    private void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }

    public void PlayButtonSound()
    {
        buttonSound.Play();
    }
    public void ExitButtonGame()
    {
        Application.Quit();
    }

}
