using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestoryOnLoad : MonoBehaviour
{

    public GameObject player;
    public PlayerController playerController;
    public UIManager uiManager;
    public GameManager gameManager;
    
    // Start is called before the first frame update
    public static DontDestoryOnLoad instance;
    
     private void Awake() {
      if(instance != null){
            Destroy(gameObject);
            return;
        } else {
            instance = this;
         DontDestroyOnLoad(this);
        }  
        if(FindObjectOfType<PlayerController>()){
            player.SetActive(true);
            playerController = player.GetComponent<PlayerController>();
        }
    }

    public void Init()
    {
        playerController.Start();
        uiManager.Start();
        gameManager.Start();
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
