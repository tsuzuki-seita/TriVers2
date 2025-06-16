using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    public float velosity;
    public string casterName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector2(velosity,0) * Time.deltaTime);
    }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.tag == "BoundingSpace") return;
    //     if (collision.gameObject.tag == casterName) return;
    //     Destroy(gameObject);
    // }
}
