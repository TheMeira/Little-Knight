using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Resource_Controller
{
    public float currentValue { protected set; get; }
    public float maxValue;
    public Slider slider;
    private TextMeshProUGUI textInfo;



    public void ModifyValue(float value)
    {
        if(currentValue + value > maxValue)
        {
            currentValue = maxValue;
        }else
        {

        currentValue += value;
        }
        slider.value = currentValue;
        textInfo.text = currentValue + "/" + maxValue;
    }

    public void Init(float max, Slider slider,TextMeshProUGUI text)
    {
        this.maxValue = max;
        this.slider = slider;
        this.textInfo = text;
        currentValue = maxValue;
        slider.maxValue = maxValue;
        slider.minValue = 0;
        slider.value = currentValue;

        textInfo.text = currentValue + "/" + maxValue;

    }
}
