using UnityEngine;
using System.Collections;

public class PlayerPickerUpper : MonoBehaviour 
{
	public Player player;
	public Collider2D pickUpArea;
	public GameObject currentMonster;
	Pickupable pickup;
	public bool isAreaActive = false;
	private bool toggle = false;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		float fire = Input.GetAxis("Fire2");
		if (fire > 0.0f) // fire btn
		{
			if (!currentMonster) // if holding none
			{
				isAreaActive = true; // activate suction
			}
			else if (!pickup) // if holding but have not registered it
			{
				isAreaActive = false;
				pickup = currentMonster.GetComponent<Pickupable>();
				if (pickup && player && player.pointer)
				{
					pickup.pickupObj(player.pointer, Vector3.up); // trigger pickup, makes us own monster
				}
			}
			else if (!toggle) // if holding, and registered and first button press after release
			{
				currentMonster = null;
				if (pickup) pickup.shoot(Vector2.up * 100.0f); // fire away monster and reset
				pickup = null;
			}
			toggle = true;
		}
		else
		{
			toggle = false;
			isAreaActive = false;
		}
	}

	void OnTriggerEnter2D(Collider2D in_collider)
	{
		HandlePickup(in_collider);
	}

	void OnTriggerStay2D(Collider2D in_collider)
	{
		HandlePickup(in_collider);
	}


	void HandlePickup(Collider2D in_collider)
	{
		if (isAreaActive && in_collider && in_collider.gameObject)
		{
            Monster monster = in_collider.GetComponent<Monster>();
            if (monster && monster.staggered)
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
