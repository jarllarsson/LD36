using UnityEngine;
using System.Collections;

public class MonsterShootBehaviour : MonoBehaviour {

    public bool activated = false;
    public Vector3 constantMovement = Vector3.up;
    public GameObject affectedGO;
    private Rigidbody2D rb;
    private int shootLayer = 0;

	// Use this for initialization
	void Start () 
    {
	    shootLayer = LayerMask.NameToLayer("MonsterProj");
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
	    if (activated && rb)
        {
            rb.velocity = constantMovement;
        }
	}

    public void TriggerShoot()
    {
        activated = true; 
        affectedGO.layer = shootLayer;
        rb = affectedGO.AddComponent( typeof(Rigidbody2D) ) as Rigidbody2D;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        rb.velocity = constantMovement;

        CircleCollider2D cc = affectedGO.AddComponent( typeof(CircleCollider2D) ) as CircleCollider2D;
    }

    public void OnCollisionEnter2D(Collision2D in_coll)
    {
        if (activated) HandleCollision(in_coll);
    }

    public void OnCollisionStay2D(Collision2D in_coll)
    {
        if (activated) HandleCollision(in_coll);
    }

    void HandleCollision(Collision2D in_coll)
    {
        if (in_coll.gameObject.tag == "Destructible")
        {
            Destroy(in_coll.gameObject);
        }
        Destroy(affectedGO);
        Debug.Log("Hit brick");
    }
}
