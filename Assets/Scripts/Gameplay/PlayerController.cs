using UnityEngine;

public enum State
{
    Idle,
    Walking,
    Jumping,
    Falling
}

public class PlayerController : MonoDodge
{
    public float moveSpeed = 10f;
    public float jumpForce = 3.49f;
    public float gravity = -7.73f;
    public float fallMultiplier = 1.86f;
    public float lowJumpMultiplier = 1.75f;
    public GameObject cameraPivot;
    public LayerMask enviroment;
    public float groundCheckDistance = 0.47f;
    public State state;
    public bool cameraRelativeControls = true;

    public bool isGrounded //Does not need to be a property, but we do not have hideininspector so i made it one.
    {
        set { _isGrounded = value; }
        get { return _isGrounded; }
    } 
    private bool _isGrounded = false;
    public Quaternion movementRotation;
    public Vector3 MoveDirection
    {
        get { return movedir; }
        set
        {
            movedir = value;
        }
    }

    private Vector3 movedir;
    private Vector3 velocity = Vector3.zero;
    private CharacterController cc;
    private float jumpBuffer = 0f;
    private float coyoteTime = 0f;
    public override void Start()
    {
        cc = transform.gameObject.GetComponent<CharacterController>();
    }

    public override void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        MoveDirection = new Vector3(horizontal, 0, vertical);
        if (cameraRelativeControls || Mathf.Abs(vertical) < 0.9f && Mathf.Abs(horizontal) < 0.9f)
        {
            movementRotation = Quaternion.Euler(0, 180, 0) * cameraPivot.transform.rotation;

        }
        if (_isGrounded && MoveDirection.magnitude < 0.01f)
        {
            state = State.Idle;
        }
        else
        {
            state = _isGrounded ? State.Walking : velocity.y > 0 ? State.Jumping : State.Falling;
        }
        
        cc.Move( movementRotation * (MoveDirection * (moveSpeed * Time.deltaTime)));


        if (Input.GetButtonDown("Jump")) jumpBuffer = 0.5f;
        if (Physics.SphereCast(new Ray(transform.position, Vector3.down), 0.2f, groundCheckDistance, enviroment))
            coyoteTime = 0.5f;
        _isGrounded = coyoteTime > 0f;
        
        if (jumpBuffer > 0f) jumpBuffer -= Time.deltaTime;
        if (coyoteTime > 0f) coyoteTime -= Time.deltaTime;
        if (jumpBuffer > 0f && _isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravity);
            jumpBuffer = 0f;
        }

        if (_isGrounded && velocity.y < 0) velocity.y = 0f;
        velocity.y += gravity * Time.deltaTime;
        if (velocity.y < 0)
        {
            velocity.y += gravity * (fallMultiplier - 1) * Time.deltaTime;
        } else if (velocity.y > 0 && !Input.GetButton("Jump"))
        {
            velocity.y += gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        cc.Move(velocity * Time.deltaTime);
    }

    public void Teleport(Vector3 newPosition)
    {
        bool ccisenabled = cc.enabled;
        cc.enabled = false;
        transform.position = newPosition;
        // ReSharper disable once Unity.InefficientPropertyAccess
        cc.enabled = ccisenabled;
    }

    public override void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.DrawLine(position,position - Vector3.up * groundCheckDistance);
    }
}
