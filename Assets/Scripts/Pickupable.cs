using UnityEngine;
using System.Collections;

public class Pickupable : MonoBehaviour 
{

    public MonoBehaviour monsterScript;
    public GameObject shootSpawn;
    public Rigidbody2D rbody;
    public Collider2D collider;

    public bool pickup = false;


	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    public void InvalidateMonster()
    {
        if (monsterScript)
        {
            DestroyImmediate(monsterScript);
        }
        if (rbody)
        {
            DestroyImmediate(rbody);
        }
        if (collider)
        {
            DestroyImmediate(collider);
        }
    }

    public void pickupObj(Transform in_obj, Vector3 in_offsetDir)
    {
        pickup = true;
        InvalidateMonster();
        Debug.Log("Picked up!");
        transform.parent = in_obj;
        transform.localPosition = in_offsetDir * transform.localScale.y;
    }

    public void shoot(Vector2 in_power)
    {
        //pickup = false;
        Debug.Log("shoot!");
        Debug.DrawLine(transform.position, transform.position + new Vector3(in_power.x, in_power.y, 0.0f), Color.magenta, 5.0f);
        Destroy(gameObject);
        // instance shootSpawn
        // set shootspawn sprite to my own
    }
}
