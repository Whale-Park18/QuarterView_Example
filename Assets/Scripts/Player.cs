using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Walk 관련 속성
    private float axisX;
    private float axisZ;
    private Vector3 moveVector3;
    public float moveSpeed;

    // Run 관련 속성
    private bool runState = false;
    private bool isRunKeyDown;

    // Jump 관련 속성
    private bool isJumpKeyDown;
    private bool isJumping = false;
    public float jumpPower;

    // Dodge 관련 속성
    private bool isDodgeKeyDown;
    private bool isDodging;
    private Vector3 dodgeVector3;

    // 컴포넌트 관련
    private Animator animator;
    private Rigidbody rigidbody;

    // 게임 오브젝트 관련
    public Text runModeText;

    // 디버그용
    public Text KeyLeftShiftText;
    public Text KeyLeftCtrlText;
    public Text KeySpaceText;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        UpdateRunModeText();
    }

    // 쿼터뷰 액션 게임 특성상 물리엔진을 통한 이동은 제어하기 어려움
    // 물리엔진을 통한 게임은 레이싱 같은 물리적 작용이 필요한 움직임에 어울림
    //private void FixedMove()
    //{
    //    moveVector3 = new Vector3(axisX, 0f, axisZ).normalized;
    //    print(Time.deltaTime);
    //
    //    if (isDodging)
    //        moveVector3 = dodgeVector3;
    //
    //    //if(rigidbody.velocity > maxSpeed)
    //    print(rigidbody.velocity);
    //
    //    if (isDodging || runState)
    //        rigidbody.AddForce(moveVector3 * fixedMoveSpeed, ForceMode.Impulse); 
    //    else
    //        rigidbody.AddForce(moveVector3 * fixedMoveSpeed * 0.3f, ForceMode.Impulse);
    //
    //    animator.SetBool("IsWalk", moveVector3 != Vector3.zero);
    //    animator.SetBool("IsRun", runState);
    //}

    //private void FixedUpdate()
    //{
    //    FixedMove();
    //}

    private void GetInput()
    {
        axisX = Input.GetAxisRaw("Horizontal");
        axisZ = Input.GetAxisRaw("Vertical");
        isRunKeyDown = Input.GetButtonDown("Run");
        isJumpKeyDown = Input.GetButtonDown("Jump");
        isDodgeKeyDown = Input.GetKeyDown(KeyCode.LeftControl);
    }

    private void DebugKeyUp()
    {
        KeyLeftShiftText.color = Color.gray;
        KeyLeftCtrlText.color = Color.gray;
        KeySpaceText.color = Color.gray;
    }

    private void DebugKeyDown()
    {
        if (isRunKeyDown)
        {
            KeyLeftShiftText.color = Color.red;
            Invoke("DebugKeyUp", 1f);
        }

        if(isDodgeKeyDown)
        {
            KeyLeftCtrlText.color = Color.red;
            Invoke("DebugKeyUp", 1f);
        }

        if(isJumpKeyDown)
        {
            KeySpaceText.color = Color.red;
            Invoke("DebugKeyUp", 1f);
        }
    }

    private void UpdateRunModeText()
    {
        string mode;
        if (runState)
            mode = "On";
        else
            mode = "Off";
            
        runModeText.text = string.Format("Run: " + mode);
    }

    private void UpdateRunState()
    {
        if(isRunKeyDown && !isJumping)
        {
            runState = !runState;
            UpdateRunModeText();
        }
    }

    private void Move()
    {
        moveVector3 = new Vector3(axisX, 0f, axisZ).normalized;

        if (isDodging)
            moveVector3 = dodgeVector3;

        if (isDodging || runState)
            transform.position += moveVector3 * moveSpeed * Time.deltaTime;
        else
            transform.position += moveVector3 * moveSpeed * 0.3f * Time.deltaTime;


        animator.SetBool("IsWalk", moveVector3 != Vector3.zero);
        animator.SetBool("IsRun", runState);
    }

    private void Turn()
    {
        transform.LookAt(transform.position + moveVector3);
    }

    private void Jump()
    {
        if(isJumpKeyDown && !isJumping && !isDodging)
        {
            rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

            animator.SetBool("IsJump", true);
            animator.SetTrigger("DoJump");

            isJumping = true;
        }
    }

    private void Dodge()
    {
        if(isDodgeKeyDown && !isDodging && !isJumping)
        {
            moveSpeed *= 2;
            isDodging = true;
            dodgeVector3 = transform.forward;

            animator.SetTrigger("DoDodge");

            Invoke("DodgeOut", 1.3f);
        }
    }

    private void DodgeOut()
    {
        moveSpeed /= 2;
        isDodging = false;
    }

    private void GameOut()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        UpdateRunState();
        Move();
        Turn();
        Jump();
        Dodge();

        DebugKeyDown();

        GameOut();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false;
            animator.SetBool("IsJump", false);
        }
    }
}
