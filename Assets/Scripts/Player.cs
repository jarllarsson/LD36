using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpd = 1.0f;
    public Rigidbody2D rbody;
    public Collider2D collider;

    public bool canMove = true;
    public float downCheckLen = 1.0f;
    public float sideCheckLen = 1.0f;

    private int playerLayer = 0;
    private int platformLayer = 0;
    private int worldLayer = 0;
    public Renderer myRenderer;

    public float jumpPwr = 100.0f;
    float jumpCooldown = 0.4f;
    float jumpCooldownCount = 0.0f;
    public bool onGround = false;


	// Use this for initialization
	void Start () {
        playerLayer = LayerMask.NameToLayer("Player");
        platformLayer = LayerMask.NameToLayer("Platforms");
        worldLayer = LayerMask.NameToLayer("World");
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        float dir = Input.GetAxis("Horizontal");
        float jump = Input.GetAxis("Jump");
        canMove = true;

        // Jump
        jumpCooldownCount -= Time.deltaTime;
        if (jump > 0.0f && jumpCooldownCount < 0.0f && rbody.velocity.y <= 0.01f && onGround)
        {
            canMove = false;
            rbody.AddForce(new Vector2(0.0f, jumpPwr));
            jumpCooldownCount = jumpCooldown;
        }
        if (rbody.velocity.y < 0.0f)
        {
            canMove = true;
            myRenderer.material.color = Color.red;
        }
        else
        {
            myRenderer.material.color = Color.blue;
        }

        // Movement
        if (canMove)
            rbody.velocity = new Vector2(dir * moveSpd, rbody.velocity.y);

        // One way platform jump
        Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, rbody.velocity.y > 0.0f);

        // On ground check
        onGround = false;
        for (int i = -1; i < 2; i++)
        {
            Debug.DrawLine(transform.position - Vector3.right * (float)i * sideCheckLen, transform.position - Vector3.up * downCheckLen - Vector3.right * (float)i * sideCheckLen);
            Vector2 offset = -Vector3.right * (float)i * sideCheckLen;
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + offset, -Vector2.up, downCheckLen, ~(1 << playerLayer));
            if (hit.collider != null)
            {
                if (hit.normal.y > hit.normal.x)
                {
                    onGround = true;
                }
            }
        }
    }

}
