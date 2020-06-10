using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{

    public float bulletSpeed;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position -= transform.right * Time.fixedDeltaTime * bulletSpeed; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Destroy(gameObject, 3f);
    }
}
