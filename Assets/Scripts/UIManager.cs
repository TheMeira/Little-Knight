using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Slider sliderHealth;
    public TextMeshProUGUI healthText;
    public Slider sliderMana;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI textCoin;
    [SerializeField] private GameObject moveRight;
    [SerializeField] public TextMeshProUGUI tutorialDesc;
    public GameObject panelQuest;

    public GameObject panelWin,panelMenu;
    public void Start()
    {
        panelWin.SetActive(false);
        textCoin.text = "0";
        moveRight.SetActive(false);
        panelQuest.SetActive(false);
    }
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(this);
        }
    }
  
        public void addCoin(int value)
    {
        textCoin.text = value.ToString();
    }

    public void setMoveRight(bool b)
    {
        moveRight.SetActive(b);
    }
}
