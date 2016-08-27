using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

    public bool staggered = false;
    public float staggerTime = 1.0f;
    private float staggerCounter = 0.0f;
    public Renderer render;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (render)
        {
            if (staggered)
            {
                render.material.color = Color.gray;
            }
            else
            {
                render.material.color = Color.green;
            }
        }

        if (staggerCounter > 0.0f)
            staggerCounter -= Time.deltaTime;
        else
            staggered = false;
    }

    public void Stagger()
    {
        staggered = true;
        staggerCounter = staggerTime;
    }
}
