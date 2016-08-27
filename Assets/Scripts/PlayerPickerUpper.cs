using UnityEngine;
using System.Collections;

public class PlayerPickerUpper : MonoBehaviour 
{
    public Player player;
    public Collider2D pickUpArea;
    public GameObject currentMonster;
    Pickupable pickup;
    public bool isActive = false;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        float fire = Input.GetAxis("Fire2");
        if (fire > 0.0f)
        {
            if (!currentMonster)
            {
                isActive = true;
            }
            else if (!pickup)
            {
                isActive = false;
                pickup = currentMonster.GetComponent<Pickupable>();
                if (pickup && player && player.pointer)
                {
                    pickup.pickupObj(player.pointer, Vector3.up);
                }
            }
        }
        else
        {
            isActive = false;
            currentMonster = null;
            if (pickup) pickup.shoot(Vector2.up * 100.0f);
            pickup = null;
        }
	}

    void OnTriggerEnter2D(Collider2D in_collider)
    {
        if (isActive && in_collider && in_collider.gameObject)
        {
            currentMonster = in_collider.gameObject;
        }
    }

    void OnTriggerStay2D(Collider2D in_collider)
    {
        if (isActive && in_collider && in_collider.gameObject)
        {
            currentMonster = in_collider.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D in_collider)
    {
        if (in_collider && in_collider.gameObject && in_collider.gameObject == currentMonster)
        {
            currentMonster = null;
        }
    }

}
