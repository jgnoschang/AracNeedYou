using StarterAssets;

using UnityEngine;
using UnityEngine.InputSystem;

public class SpiderController : MonoBehaviour
{
    public float _speed = 3f;
    public float smoothness = 5f;
    public int raysNb = 8;
    public float raysEccentricity = 0.2f;
    public float outerRaysOffset = 2f;
    public float innerRaysOffset = 25f;
    public float RotationSmoothTime = 0.12f;

    private Vector3 velocity;
    private Vector3 lastVelocity;
    private Vector3 lastPosition;
    private Vector3 forward;
    private Vector3 upward;
    private Quaternion lastRot;
    private Vector3[] pn;
    private Camera camera;
    private StarterAssetsInputs _input;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;


    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;
    private bool inWall;
    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.40f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffsety = -0.14f;
    public float GroundedOffsetz = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;

    // player

    private float _rotationVelocityY;
    private float _rotationVelocityX;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;


    private Animator _animator;
    private bool _hasAnimator;
    private Rigidbody rb;
    private PlayerInput _playerInput;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    public StarterAssetsInputs Input { get => _input; set => _input = value; }

    Vector3[] GetIcoSphereCoords(int depth)
    {
        Vector3[] res = new Vector3[(int)Mathf.Pow(4, depth) * 12];
        float t = (1f + Mathf.Sqrt(5f)) / 2f;
        res[0] = (new Vector3(t, 1, 0));
        res[1] = (new Vector3(-t, -1, 0));
        res[2] = (new Vector3(-1, 0, t));
        res[3] = (new Vector3(0, -t, 1));
        res[4] = (new Vector3(-t, 1, 0));
        res[5] = (new Vector3(1, 0, t));
        res[6] = (new Vector3(-1, 0, -t));
        res[7] = (new Vector3(0, t, -1));
        res[8] = (new Vector3(t, -1, 0));
        res[9] = (new Vector3(1, 0, -t));
        res[10] = (new Vector3(0, t, 1));
        res[11] = (new Vector3(0, -t, -1));

        return res;
    }

    Vector3[] GetClosestPointIco(Vector3 point, Vector3 up, float halfRange)
    {
        Vector3[] res = new Vector3[2] { point, up };

        Vector3[] dirs = GetIcoSphereCoords(0);
        raysNb = dirs.Length;

        float amount = 1f;

        foreach (Vector3 dir in dirs)
        {
            RaycastHit hit;
            Ray ray = new Ray(point + up * 0.15f, dir);
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.SphereCast(ray, 0.01f, out hit, 2f * halfRange))
            {
                if (hit.transform.gameObject != gameObject)
                {
                    res[0] += hit.point;
                    res[1] += hit.normal;
                    amount += 1;
                }
            }
        }
        res[0] /= amount;
        res[1] /= amount;
        return res;
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private void LateUpdate()
    {
        CameraRotation();
        if (!Grounded)
        {
            RotateIfAwayFromGround();
        }
    }


    static Vector3[] GetClosestPoint(Vector3 point, Vector3 forward, Vector3 up, float halfRange, float eccentricity, float offset1, float offset2, int rayAmount)
    {
        point += new Vector3(0, -0.15f, 0);
        Vector3[] res = new Vector3[2] { point, up };
        Vector3 right = Vector3.Cross(up, forward);
        float normalAmount = 1f;
        float positionAmount = 1f;

        Vector3[] dirs = new Vector3[rayAmount];
        float angularStep = 2f * Mathf.PI / rayAmount;
        float currentAngle = angularStep / 2f;
        for (int i = 0; i < rayAmount; ++i)
        {
            dirs[i] = -up + (right * Mathf.Cos(currentAngle) + forward * Mathf.Sin(currentAngle)) * eccentricity;
            currentAngle += angularStep;
        }

        foreach (Vector3 dir in dirs)
        {
            RaycastHit hit;
            Vector3 largener = Vector3.ProjectOnPlane(dir, up);
            Ray ray = new Ray(point - (dir + largener) * halfRange + largener.normalized * offset1 / 100f, dir);
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.SphereCast(ray, 0.01f, out hit, 2f * halfRange))
            {
                res[0] += hit.point;
                res[1] += hit.normal;
                normalAmount += 1;
                positionAmount += 1;
            }
            ray = new Ray(point - (dir + largener) * halfRange + largener.normalized * offset2 / 100f, dir);
            Debug.DrawRay(ray.origin, ray.direction, Color.green);
            if (Physics.SphereCast(ray, 0.01f, out hit, 2f * halfRange))
            {
                res[0] += hit.point;
                res[1] += hit.normal;
                normalAmount += 1;
                positionAmount += 1;
            }
        }
        res[0] /= positionAmount;
        res[1] /= normalAmount;
        return res;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _hasAnimator = TryGetComponent(out _animator);
        Input = GetComponent<StarterAssetsInputs>();
        camera = FindObjectOfType<Camera>();
        velocity = new Vector3();
        forward = transform.forward;
        upward = transform.up;
        lastRot = transform.rotation;
        _playerInput = GetComponent<PlayerInput>();
    }
    private void Update()
    {
        JumpAndGravity();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
     
        if (Vector3.Dot(transform.position + transform.up, transform.TransformDirection(Vector3.up)) > 130 && Grounded)
        {
            Gravity = 0.01f;
            inWall = true;
        }
        else if (Vector3.Dot(transform.position + transform.up, transform.TransformDirection(Vector3.up)) < -130 && Grounded)
        {
            Gravity = 0.01f;
            inWall = true;
        }
        else
        {
            Gravity = -10f;
            inWall = false;
        }
        GroundedCheck();
        rb.AddForce(new Vector3(0, Gravity * 100 * Time.fixedDeltaTime, 0), ForceMode.Acceleration);
        if (Grounded)
        {
            velocity = (smoothness * velocity + (transform.position - lastPosition)) / (1f + smoothness);
            if (velocity.magnitude < 0.00025f)
                velocity = lastVelocity;
            lastPosition = transform.position;
            lastVelocity = velocity;



            float multiplier = Input.sprint ? 1.5f : 1;

            float valueY = Input.move.y;
            if (valueY != 0)
                transform.position += transform.forward * valueY * _speed * multiplier * Time.fixedDeltaTime;
            float valueX = Input.move.x;
            if (valueX != 0)
                transform.position += Vector3.Cross(transform.up, transform.forward) * valueX * _speed * multiplier * Time.fixedDeltaTime;



            if (Input.move != Vector2.zero)
            {

                pn = GetClosestPoint(transform.position, transform.forward, transform.up, 1f, 0.3f, 80, -30, 5);
                //        pn = GetClosestPointIco(transform.position, transform.up, 0.2f);

                upward = pn[1];
                Vector3[] pos = GetClosestPoint(transform.position, transform.forward, transform.up, 0.5f, raysEccentricity, innerRaysOffset, outerRaysOffset, raysNb);
                transform.position = Vector3.Lerp(lastPosition, pos[0], 1f / (1f + smoothness));




                if (Input.move.y > 0 && velocity.magnitude < 0.1f)
                {
                    forward = velocity.normalized;
                    Quaternion q = Quaternion.LookRotation(forward, upward);

                    transform.rotation = Quaternion.Lerp(lastRot, q, 1f / (1f + smoothness));

                    if (inWall == false)
                    {
                        Vector3 inputDirection = new Vector3(Input.move.x, 0, Input.move.y).normalized;
                        float _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
                        float rotationy = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocityY, RotationSmoothTime);
                        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotationy, transform.rotation.eulerAngles.z);
                    }

                }
                else if (Input.move.y < 0)
                {
                    if (inWall == false)
                    {
                        Vector3 inputDirection = new Vector3(Input.move.x, 0, Input.move.y).normalized;
                        float _targetRotation = Mathf.Atan2(-inputDirection.x, -inputDirection.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
                        float rotationy = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocityY, RotationSmoothTime);
                        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotationy, transform.rotation.eulerAngles.z);
                    }
                }

                lastRot = transform.rotation;
            }
        }
        else
        {
            lastPosition = transform.position;
            if (FallTimeout <= 0)
            {
                pn = GetClosestPoint(transform.position, transform.forward, transform.up, 1f, 0.3f, 80, -30, 5);

                upward = pn[1];
                forward = velocity.normalized;
                Quaternion q = Quaternion.LookRotation(forward, upward);

                transform.rotation = Quaternion.Lerp(lastRot, q, 1f / (1f + smoothness));
                Vector3[] pos = GetClosestPoint(transform.position, transform.forward, transform.up, 0.5f, raysEccentricity, innerRaysOffset, outerRaysOffset, raysNb);
                transform.position = Vector3.Lerp(lastPosition, pos[0], 1f / (1f + smoothness));
            }
            lastRot = transform.rotation;
        }


    }

    private void RotateIfAwayFromGround()
    {

        float rotationZ = Mathf.SmoothDampAngle(transform.eulerAngles.z, 0f, ref _rotationVelocityY, RotationSmoothTime * 2);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rotationZ);
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffsety, transform.position.z - GroundedOffsetz);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }
    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (Input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -1f * Gravity);
                rb.AddForce( Vector3.up * _verticalVelocity, ForceMode.Impulse) ;
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // if we are not grounded, do not jump
            Input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffsety, transform.position.z - GroundedOffsetz), GroundedRadius);
    }

}
