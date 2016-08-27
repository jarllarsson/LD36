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

    private Vector2 jumpTrigStart;
    private bool jumpTrigTapOn = false;
    private Vector2 jumpTrigDelta;
    private Vector3 jumpVec;

    public Camera mainCam;


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
        canMove = false;
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (Input.GetAxis("Fire1") > 0.0f)
        {
            Debug.Log(mousePos);
            if (jumpTrigTapOn == false)
            {
                jumpTrigStart = mousePos;
                jumpTrigTapOn = true;
                Debug.Log("start");
            }
            else
            {
                Debug.Log("hold");
                jumpTrigDelta = mousePos - jumpTrigStart; // TODO gamepad joysticks
                Vector3 endWorldPos = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, transform.position.z - mainCam.transform.position.z));
                Vector3 startWorldPos = mainCam.ScreenToWorldPoint(new Vector3(jumpTrigStart.x, jumpTrigStart.y, transform.position.z - mainCam.transform.position.z));
                Vector3 deltaWorld = endWorldPos - startWorldPos;
                jumpVec = deltaWorld.normalized;
                //Debug.DrawLine(transform.position, transform.position + deltaWorld, Color.yellow);
                Debug.DrawLine(startWorldPos, endWorldPos, Color.red);
            }
        }
        else if (jumpTrigTapOn)
        {
            jumpTrigTapOn = false;
            Debug.DrawLine(transform.position, transform.position + new Vector3(jumpTrigDelta.x, jumpTrigDelta.y, 0.0f), Color.white, 2.0f);
            jump = 1.0f;
        }

        // Jump
        jumpCooldownCount -= Time.deltaTime;
        if (jump > 0.0f && jumpCooldownCount < 0.0f && rbody.velocity.y <= 0.01f/* && onGround*/)
        {
            canMove = false;
            rbody.AddForce(-jumpVec * jumpPwr);
            jumpCooldownCount = jumpCooldown;
        }
        if (rbody.velocity.y <= 0.0f)
        {
            canMove = true;
            myRenderer.material.color = Color.red;
        }
        else
        {
            myRenderer.material.color = Color.blue;
        }

        // Movement
        if (canMove && Mathf.Abs(dir) > 0.0f)
        {
            rbody.velocity = new Vector2(dir * moveSpd, rbody.velocity.y);
        }

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
