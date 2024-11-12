using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private int currentCoin;

    public static GameManager instance;

    public void Start()
    {
        currentCoin = 0;
        UIManager.instance.panelMenu.SetActive(false);
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }
    public void addCoin(int value)
    {
        currentCoin += value;
        UIManager.instance.addCoin(currentCoin);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        UIManager.instance.panelMenu.SetActive(true);

    }

    public void Resume()
    {
      Time.timeScale = 1;
        UIManager.instance.panelMenu.SetActive(false);
    }

    public void RestartGame()
    {
       Time.timeScale = 1;
      
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      

    }

    public void NextScene()
    {
        Time.timeScale = 1;
        UIManager.instance.panelWin.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void mainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void winLevel()
    {
      int nextSceneIndex =  SceneManager.GetActiveScene().buildIndex + 1;
        
        if (nextSceneIndex > PlayerPrefs.GetInt("LevelUnlocked"))
        {
            PlayerPrefs.SetInt("LevelUnlocked", nextSceneIndex);

        }

        AUDIOMANAGER.instance.winSFX.Play();
        UIManager.instance.panelWin.SetActive(true);
        Time.timeScale = 0;
    }
}
