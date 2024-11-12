using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AUDIOMANAGER : MonoBehaviour
{
    public static AUDIOMANAGER instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
    }
    public AudioSource winSFX, loseSFX,coinSFX,manaSFX,healthSFX;
   
    
}
