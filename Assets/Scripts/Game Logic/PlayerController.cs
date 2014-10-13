using UnityEngine;
using System.IO;
using System.Threading;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Members
    #region Character Control Members
    public float walkSpeed = 6.0f;

    public float runSpeed = 11.0f;

    // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
    public bool limitDiagonalSpeed = true;

    // If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
    // There must be a button set up in the Input Manager called "Run"
    public bool toggleRun = false;

    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    public float fallingDamageThreshold = 10.0f;

    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = false;

    // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
    public bool slideOnTaggedObjects = false;

    public float slideSpeed = 12.0f;

    // If checked, then the player can change direction while in the air
    public bool airControl = false;

    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    public float antiBumpFactor = .75f;

    // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
    public int antiBunnyHopFactor = 1;

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl = false;
    private int jumpTimer;
    #endregion
    #endregion

    #region Methods
    #region MonoBehavior Methods
    void Start()
    {

    }

    void Update()
    {

    }
    #endregion


    private void InitializeInternals()
    {
        InitializeCharacterController();
    }

    private void InitializeCharacterController()
    {
        controller = GetComponent<CharacterController>();

        myTransform = transform;
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        jumpTimer = antiBunnyHopFactor;
    }
    #endregion
}