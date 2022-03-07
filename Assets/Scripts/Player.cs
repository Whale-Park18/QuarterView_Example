using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Walk ���� �Ӽ�
    private float axisX;
    private float axisZ;
    private Vector3 moveVector3;
    public float moveSpeed;

    // Run ���� �Ӽ�
    private bool runState = false;
    private bool isRunKeyDown;

    // Jump ���� �Ӽ�
    private bool isJumpKeyDown;
    private bool isJumping = false;
    public float jumpPower;

    // Dodge ���� �Ӽ�
    private bool isDodgeKeyDown;
    private bool isDodging;
    private Vector3 dodgeVector3;

    // ������Ʈ ����
    private Animator animator;
    private Rigidbody rigidbody;

    // ���� ������Ʈ ����
    public Text runModeText;

    // ����׿�
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

    // ���ͺ� �׼� ���� Ư���� ���������� ���� �̵��� �����ϱ� �����
    // ���������� ���� ������ ���̽� ���� ������ �ۿ��� �ʿ��� �����ӿ� ��︲
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
