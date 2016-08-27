﻿using UnityEngine;
using System.Collections;

public class FollowLerp : MonoBehaviour {
	public Transform target;
	public float time;
	private Vector3 currentV;
    public Vector2 margin;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () 
    {
         // todo margins
		transform.position = Vector3.SmoothDamp(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), ref currentV, time);
	}
}
