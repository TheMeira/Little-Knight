using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    public string tagToCheck;
    public float force;
    public float direction;
    private float[]  damageParams = new float[2];
    [SerializeField] private GameObject VFX_Attack;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(float force,string tag)
    {
        this.force = force;
       
        this.tagToCheck = tag;

        damageParams[0] = force;
      
    }

    public void LaunchBullet(Vector3 direction,float speed)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        // Utilisez AddForce pour propulser la balle
        rb.AddForce(direction.normalized * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(tagToCheck != null && other.CompareTag(tagToCheck))
        {
            damageParams[1] = transform.position.x;
            other.gameObject.SendMessage("Damage", damageParams, SendMessageOptions.DontRequireReceiver);
            Instantiate(VFX_Attack, other.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
