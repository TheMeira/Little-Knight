using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    public PlayerController controller;
    [SerializeField] private GameObject VFX_Attack;
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            float[] damageParams = new float[2];
            damageParams[0] = controller.attackForce; // Valeur arbitraire pour les dégâts
            damageParams[1] = transform.position.x; // Position x du joueur
            collision.gameObject.SendMessage("Damage", damageParams, SendMessageOptions.DontRequireReceiver);
            Instantiate(VFX_Attack, transform.position, Quaternion.identity);
        }
    }

}
