using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
 
    [SerializeField] private GameObject VFX_Attack;
    public float force;
    private float[] damageParams = new float[2];

    public void Init(float force)
    {
        
            this.force = force;

            damageParams[0] = force;
       
        
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {

            damageParams[0] = force;
            damageParams[1] = transform.position.x;
            collision.gameObject.SendMessage("Damage", damageParams, SendMessageOptions.DontRequireReceiver);
            Instantiate(VFX_Attack, collision.transform.position, Quaternion.identity);
        }
    }
}
