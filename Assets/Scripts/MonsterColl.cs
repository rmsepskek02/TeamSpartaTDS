using System.Collections;
using UnityEngine;

public class MonsterColl : MonoBehaviour
{
    public GameObject hero;
    public Vector2 heroXPos;
    public bool canMove = true;
    public float moveSpeed;
    public float jumpForce;
    public float pushForce;
    public LayerMask detectionLayer;
    public float leftDetectRayDistance;
    public float topDetectRayDistance;
    public float groundDetectRayDistance;
    public Rigidbody rb;
    public bool canJump;
    public GameObject topObject;
    public bool canPush;
    public bool isGrounded;

    void Start()
    {
        if (hero == null)
        {
            hero = GameObject.FindGameObjectWithTag("Hero");
            heroXPos = new Vector2(hero.transform.position.x, 0);
        }
        moveSpeed = 2f;
        canJump = true;
        jumpForce = 5f;
        pushForce = 20f;
        leftDetectRayDistance = 0.6f;
        topDetectRayDistance = 0.55f;
        groundDetectRayDistance = 0.6f;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (canMove)
        {
            MoveToLeft();
        }
        DetectObjectOfLeft();
        DetectObjectOfTop();
        DetectGround();
    }
    public void MoveToLeft()
    {
        //rb.velocity = new Vector3(-moveSpeed, rb.velocity.y, rb.velocity.z);

        Vector3 moveDir = Vector3.left * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDir);
    }
    void DetectObjectOfLeft()
    {
        Vector3 rayOrigin = new Vector3(transform.position.x - 0.1f, transform.position.y + 0.4f, transform.position.z);

        int layerMask = detectionLayer;

        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.left, leftDetectRayDistance, layerMask);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) // �ڱ� �ڽ� ����
            {
                MonsterColl detectedMonster = hit.collider.GetComponent<MonsterColl>();
                if (detectedMonster != null)
                {
                    // ���� Z�࿡ �ִ��� Ȯ��
                    if (Mathf.Abs(transform.position.z - detectedMonster.transform.position.z) < 0.1f)
                    {
                        if (canJump && detectedMonster.topObject == null)
                        {
                            UnfreezeY();
                            //resetVelocity();
                            //rb.AddForce(new Vector3(-0.1f, 1f,0f) * 6f, ForceMode.Impulse);
                            rb.velocity = new Vector3(-moveSpeed, jumpForce, 0);
                            //rb.velocity = new Vector3(0f, jumpForce, 0);
                            canJump = false;
                            StartCoroutine(CanJump());
                        }
                    }
                    break; // ���� ����� ������Ʈ�� �����ϰ� ���� ����
                }
            }
        }

        Debug.DrawRay(rayOrigin, Vector3.left * leftDetectRayDistance, Color.red);
    }
    void DetectObjectOfTop()
    {
        Vector3 rayOrigin = new Vector3(transform.position.x - 0.1f, transform.position.y + 0.4f, transform.position.z);

        int layerMask = detectionLayer;

        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.up, topDetectRayDistance, layerMask);

        if (hits == null || hits.Length == 0)
        {
            if (topObject != null)
                topObject = null;
        }

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) // �ڱ� �ڽ� ����
            {
                MonsterColl detectedMonster = hit.collider.GetComponent<MonsterColl>();
                if (detectedMonster != null)
                {
                    // ���� Z�࿡ �ִ��� Ȯ��
                    if (Mathf.Abs(transform.position.z - detectedMonster.transform.position.z) < 0.1f)
                    {
                        if (topObject == null)
                        {
                            topObject = detectedMonster.gameObject;
                            canPush = true;
                        }
                    }
                    PushRight();
                    break; // ���� ����� ������Ʈ�� �����ϰ� ���� ����
                }
            }
        }

        Debug.DrawRay(rayOrigin, Vector3.up * topDetectRayDistance, Color.red);
    }
    void DetectGround()
    {
        Vector3 rayOrigin = new Vector3(transform.position.x - 0.1f, transform.position.y +0.4f, transform.position.z);
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, groundDetectRayDistance))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            isGrounded = false;
        }

        Debug.DrawRay(rayOrigin, Vector3.down * groundDetectRayDistance, Color.green);
    }

    public void resetVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; // ȸ�� �ӵ��� �ʱ�ȭ
    }
    public void PushRight()
    {
        if (canPush)
        {
            // ���� ��� ���� �ӵ��� �ʱ�ȭ (��ø ����)
            resetVelocity();

            rb.AddForce(Vector3.right * pushForce, ForceMode.Impulse);
            //rb.velocity = new Vector3(-pushForce, 0f, 0);
            //StartCoroutine(PushCo());
            canPush = false;
            UnfreezeY();
            //isPush = true;
        }
    }
    void FreezeY()
    {
        rb.constraints |= RigidbodyConstraints.FreezePositionY; // ���� Constraints �����ϸ鼭 Y�� ���� �߰�
    }

    void UnfreezeY()
    {
        rb.constraints &= ~RigidbodyConstraints.FreezePositionY; // ���� Constraints �����ϸ鼭 Y�� ������ ����
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            canMove = false;
            canJump = false;
            if (!isGrounded)
            {
                UnfreezeY();
            }
        }
        if (collision.gameObject.CompareTag("Monster"))
        {
            if (collision.gameObject.transform.position.y + 0.1f < transform.position.y) return;
            if (collision.gameObject.transform.position.x > transform.position.x)
            {
                FreezeY();
            }
            else
            {

            }
            //jumpForce = 8f;
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            // �ٴڿ� ����� �� Y�� �ӵ��� ������ 0���� ����
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            //rb.angularVelocity = Vector3.zero; // ȸ�� �ӵ��� �ʱ�ȭ (���ʿ��� ȸ�� ����)
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            canMove = true;
            StartCoroutine(CanJump());
        }
        if (collision.gameObject.CompareTag("Monster"))
        {
            //jumpForce = 6f;
        }
    }

    IEnumerator CanJump()
    {
        yield return new WaitForSeconds(3f);
        canJump = true;
    }
}
