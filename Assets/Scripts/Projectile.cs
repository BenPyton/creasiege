using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10.0f;
    [SerializeField] float lifeSpan = 3.0f;

    private float timer = 0.0f;

    public bool ally = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        yield return new WaitUntil(() =>
        {
            if(DataManager.instance.mode != Mode.Play)
            {
                Destroy(gameObject);
                return true;
            }

            //transform.position += transform.forward * speed * Time.deltaTime;
            timer += Time.deltaTime;
            if(timer >= lifeSpan)
            {
                Destroy(gameObject);
                return true;
            }
            return false;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ally)
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.GetDamage(1);
                Destroy(gameObject);
            }
        }
        else
        {
            Part part = other.GetComponent<Part>();
            if (part != null && !part.isRoot)
            {
                part.GetDamage(100);
                Destroy(gameObject);
            }
        }
    }

}
