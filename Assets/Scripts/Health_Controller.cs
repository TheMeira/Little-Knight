using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_Controller : MonoBehaviour
{
    public float currentHealth { private set; get; }
    [SerializeField] private float maxHealth;
    [SerializeField] private Slider sliderHealth;


    private void Start()
    {
        currentHealth = maxHealth;
        sliderHealth.value = currentHealth; 

    }


    void modifyHealth(float health)
    {
        currentHealth -= health;
        sliderHealth.value = currentHealth;
    }
    
}
