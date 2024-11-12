using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collactable : MonoBehaviour
{
    // Start is called before the first frame updateseri
    [SerializeField] private AudioClip sfx;
    private enum collectable
    {
        coin,health,mana
    }
   [SerializeField] private collectable currentCollectable;
    [SerializeField]
    private float value;
    [SerializeField] private GameObject VFX_OnDestory;

    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (currentCollectable == collectable.coin)
            {
                GameManager.instance.addCoin((int)value);
                AUDIOMANAGER.instance.coinSFX.Play();
            }
            else if (currentCollectable == collectable.health)
            {
                collision.gameObject.GetComponent<PlayerController>().addHealth((int)value);
                AUDIOMANAGER.instance.healthSFX.Play();
            }
            else if (currentCollectable == collectable.mana)
            {
                collision.gameObject.GetComponent<PlayerController>().addMana((int)value);
                AUDIOMANAGER.instance.manaSFX.Play();
            }
           
            if (VFX_OnDestory)
                Instantiate(VFX_OnDestory, transform.position, Quaternion.identity);


            Destroy(gameObject);
        }
    }
}
