using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public float moveSpeed = 1.0f;
	public Rigidbody2D rbody;
	public Collider2D collider;

	//public bool canMove = true;
	public float downCheckLen = 1.0f;
	public float downCheckOffsetLen = 1.0f;
	public float wallCheckLen = 1.0f;
	private float hitMove = 0.0f;

	private int playerLayer = 0;
	private int platformLayer = 0;
	private int worldLayer = 0;
	public Renderer myRenderer;

	public float jumpPwr = 100.0f;
	public float instantJumpV = 3.0f;
	float jumpCooldown = 0.4f;
	float jumpCooldownCount = 0.0f;
	public bool onGround = false;

	private Vector2 jumpTrigStart;
	private bool jumpTrigTapOn = false;
	private Vector2 jumpTrigDelta;
	private Vector3 jumpVec;
	private Vector2 oldRStickDir;

	public Camera mainCam;
	bool canMove = true;

    public bool dragJump = false;

    public bool regularJump = true;

	// Use this for initialization
	void Start () {
		playerLayer = LayerMask.NameToLayer("Player");
		platformLayer = LayerMask.NameToLayer("Platforms");
		worldLayer = LayerMask.NameToLayer("World");
	}
	
    void Update()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dir = Input.GetAxis("Horizontal");
        Vector2 rStickDir = new Vector2(Input.GetAxis("RHorizontal"), Input.GetAxis("RVertical"));
        float rStickMagnitude = rStickDir.magnitude;
        if (rStickMagnitude >= 0.2f)
            oldRStickDir = rStickDir;
        float jump = Input.GetAxis("Jump");
        if (jump > 0.0f)
        {
            jumpVec = Vector2.down;
        }
        //canMove = false;
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        bool jumpPress = Input.GetAxis("Fire1") > 0.0f;

        if (!regularJump)
        {
            if (jumpPress || rStickMagnitude > 0.1f)
            {
                if (jumpPress)
                {
                    jumpVec = Vector2.down;
                }
                Debug.Log(mousePos);
                if (jumpTrigTapOn == false)
                {
                    jumpTrigStart = mousePos;
                    jumpTrigTapOn = true;
                    //Debug.Log("start");
                }
                else
                {
                    //Debug.Log("hold");
                    jumpTrigDelta = mousePos - jumpTrigStart; // TODO gamepad joysticks
                    Vector3 endWorldPos = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, transform.position.z - mainCam.transform.position.z));
                    Vector3 startWorldPos = mainCam.ScreenToWorldPoint(new Vector3(jumpTrigStart.x, jumpTrigStart.y, transform.position.z - mainCam.transform.position.z));
                    Vector3 deltaWorld = endWorldPos - startWorldPos;
                    if (deltaWorld.normalized.sqrMagnitude > 0.1f)
                        jumpVec = deltaWorld.normalized;
                    //Debug.DrawLine(transform.position, transform.position + deltaWorld, Color.yellow);
                    Debug.DrawLine(startWorldPos, endWorldPos, Color.red);
                    Debug.DrawLine(transform.position, transform.position + new Vector3(oldRStickDir.x, oldRStickDir.y, 0.0f), Color.red);
                }
            }
            else if (jumpTrigTapOn)
            {
                if (oldRStickDir.sqrMagnitude > jumpVec.sqrMagnitude * 0.1f)
                {
                    jumpVec = oldRStickDir.normalized;
                }
                jumpTrigTapOn = false;
                Debug.DrawLine(transform.position, transform.position + new Vector3(jumpTrigDelta.x, jumpTrigDelta.y, 0.0f), Color.white, 2.0f);
                Debug.DrawLine(transform.position, transform.position + new Vector3(oldRStickDir.x, oldRStickDir.y, 0.0f) * jumpPwr, Color.white, 2.0f);
                jump = 1.0f;
            }
        }

		// Jump
		jumpCooldownCount -= Time.deltaTime;
		if (jump > 0.0f && jumpCooldownCount < 0.0f && rbody.velocity.y <= 0.01f && (!regularJump || onGround))
		{
			//canMove = false;
			rbody.velocity = new Vector2(rbody.velocity.x, Mathf.Max(rbody.velocity.y, -jumpVec.y * instantJumpV));
			rbody.AddForce(-jumpVec * jumpPwr);
			jumpCooldownCount = jumpCooldown;
			oldRStickDir = Vector2.zero;
		}
		if (rbody.velocity.y <= 0.0f)
		{
			//canMove = true;
			myRenderer.material.color = Color.red;
		}
		else
		{
			myRenderer.material.color = Color.blue;
		}

        // Movement
        float move = hitMove + dir * moveSpeed;
        if (canMove && Mathf.Abs(move) > 0.0f)
		{
			rbody.velocity = new Vector2(move, rbody.velocity.y);
		}
        hitMove *= 0.1f;

        // One way platform jump
        Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, rbody.velocity.y > 0.0f);

		// On ground check
		onGround = false;
		for (int i = -1; i < 2; i++)
		{
			Debug.DrawLine(transform.position - Vector3.right * (float)i * downCheckOffsetLen, transform.position - Vector3.up * downCheckLen - Vector3.right * (float)i * downCheckOffsetLen);
			Vector2 offset = -Vector3.right * (float)i * downCheckOffsetLen;
			RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + offset, -Vector2.up, downCheckLen, ~(1 << playerLayer));
			if (hit.collider != null)
			{
				if (hit.normal.y > 0.0f && hit.normal.y > hit.normal.x)
				{
					onGround = true;
				}
			}
		}
	}

	void OnCollisionEnter2D(Collision2D in_collision)
	{
		Vector2 avgNormal = averageHitNormal(in_collision);
		HandleWallBounce(avgNormal);
	}

	void OnCollisionStay2D(Collision2D in_collision)
	{
		Vector2 avgNormal = averageHitNormal(in_collision);
		HandleWallBounce(avgNormal);
	}

	void HandleWallBounce(Vector2 in_normal)
	{
		if (Mathf.Abs(in_normal.x) > 0.7f)
		{
            hitMove += in_normal.x * moveSpeed * 1.0f;
		}
	}

	Vector2 averageHitNormal(Collision2D in_collision)
	{
		Vector2 normal = Vector2.zero;
		int count = 0;
		foreach (ContactPoint2D contact in in_collision.contacts)
		{
			//Debug.DrawRay(contact.point, contact.normal, Color.white, 1.0f);
			count++;
			normal += contact.normal;
		}
		if (count > 0) normal /= (float)count;
		return normal;
	}
}
