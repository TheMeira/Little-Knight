using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevelScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        DontDestoryOnLoad.instance.Init();
        DontDestoryOnLoad.instance.playerController.ChangePositionPlayer(transform);

    }

   
}
