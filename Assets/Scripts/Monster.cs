using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float moveSpeed;
    public int line;
    public bool isJumping = false;
    public float jumpHeight = 2.0f;
    public float jumpSpeed = 5.0f;
    public float gravity = -9.8f;
    public float zVelocity = 0;
    public bool canMove = true;
    public GameObject hero;
    public Vector2 heroXPos;
    public float stopDistance;
    public float detectionDistance;
    public LayerMask detectionLayer;
    public bool canJump;
    public LineManager lineManager;

    private bool hasLanded = false;
    private float verticalVelocity = 0f; // ���� �ӵ�

    private Transform targetMonster;  // ������ ������ ��ġ
    // Start is called before the first frame update
    void Start()
    {
        if (hero == null)
        {
            hero = GameObject.FindGameObjectWithTag("Hero");
            heroXPos = new Vector2(hero.transform.position.x, 0);
        }
        if(lineManager == null)
        {
            lineManager = FindObjectOfType<LineManager>();
        }
        moveSpeed = 2f;
        stopDistance = 1.5f;
        detectionDistance = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            MoveToLeft();
        }

        if (isJumping)
        {
            JumpOverMonster();
        }
        //Jump();
        DetectObjectOnLeft();
        CheckDistanceHero();
    }

    public void MoveToLeft()
    {
        Vector2 moveDir = Vector2.left;
        transform.position += (Vector3)moveDir * moveSpeed * Time.deltaTime;
    }

    public void CheckDistanceHero()
    {
        if (Vector2.Distance(new Vector2(transform.position.x, 0), heroXPos) < stopDistance)
        {
            canMove = false;
            //TODO :: Attack
        }
        else
        {
            //canMove = true;
        }
    }

    void DetectObjectOnLeft()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y + 0.5f);

        int layerMask = detectionLayer; // �ʿ��� ���̾ ����

        // ���� �������� RaycastAll �߻�
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.left, detectionDistance, layerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) // �ڱ� �ڽ� ����
            {
                Monster detectedMonster = hit.collider.GetComponent<Monster>();
                if (detectedMonster != null && detectedMonster.line == line)
                {
                    PerformAction(hit.collider.transform);
                    if (Mathf.Abs(transform.position.x - hit.collider.transform.position.x) < 4f)
                    {
                        canMove = false;
                    }
                    break; // ���� ����� ������Ʈ�� �����ϰ� ���� ����
                }
            }
        }

        Debug.DrawRay(rayOrigin, Vector2.left * detectionDistance, Color.red);
    }

    void PerformAction(Transform monster)
    {
        // ���� ���� ���� ����
        targetMonster = monster;
        isJumping = true;
        hasLanded = false;
        verticalVelocity = jumpSpeed;
        Debug.Log("Performing action: Jumping over monster!");
    }

    void JumpOverMonster()
    {
        if (targetMonster != null)
        {
            // ��ǥ ���� ��ġ�� ����
            Vector3 targetPosition = targetMonster.position + new Vector3(0, 1, 0); // ���� ���� 1 ���� �ö󰡰�

            // ���� ���� ���·� ���
            transform.position += new Vector3(0, verticalVelocity * Time.deltaTime, 0);

            // �߷� ����
            verticalVelocity += gravity * Time.deltaTime;

            // ��ǥ ��ġ�� �����ϰų�, �� �̻� �ö��� ������ ����
            if (transform.position.y >= targetPosition.y)
            {
                transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
                hasLanded = true;
                isJumping = false;
                canMove = true;
            }
        }
    }

    private void OnDestroy()
    {
        // Monster�� �ı��� �� RemoveMonster �׼��� ȣ���Ͽ� ����
        lineManager.RemoveMonster?.Invoke(this);
        Debug.Log($"{gameObject.name} �ı��Ǿ� ����Ʈ���� ���ŵ˴ϴ�.");
    }

    //public void Jump()
    //{
    //    if (!isJumping && canJump)
    //    {
    //        isJumping = true;
    //        zVelocity = jumpSpeed;
    //    }

    //    if (isJumping)
    //    {
    //        transform.position += new Vector3(-1 * 0.2f * Time.deltaTime, zVelocity * Time.deltaTime, zVelocity * Time.deltaTime);
    //        zVelocity += gravity * Time.deltaTime;

    //        if (transform.position.z <= 0)
    //        {
    //            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    //            isJumping = false;
    //            canJump = false;
    //        }
    //    }
    //}
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("COLL MONSTER");
    //    // �ٸ� ����� �浹 �� �ѱ��
    //    if (collision.gameObject.CompareTag("Monster"))
    //    {
    //        Monster otherZombie = collision.gameObject.GetComponent<Monster>();
    //        if (otherZombie != null)
    //        {
    //            //�浹�� �ٸ� ���� �տ� ������ �Ѿ��
    //            if (otherZombie.transform.position.x < transform.position.x && otherZombie.line == line)
    //            {
    //                canJump = true;
    //                canMove = false;
    //                //�ٸ� ���� �տ� ������ �Ѿ �� �ֵ��� ó��
    //                //transform.position = new Vector2(otherZombie.transform.position.x - 0.5f, transform.position.y);
    //            }
    //        }
    //    }
    //}
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    // �ٸ� ����� �浹 �� �ѱ��
    //    if (other.CompareTag("Monster"))
    //    {
    //        Monster otherZombie = other.GetComponent<Monster>();
    //        if (otherZombie != null)
    //        {
    //            // �浹�� �ٸ� ���� �տ� ������ �Ѿ��
    //            if (otherZombie.transform.position.x < transform.position.x && otherZombie.line == line)
    //            {
    //                canJump = true;
    //                canMove = false;
    //                // �ٸ� ���� �տ� ������ �Ѿ �� �ֵ��� ó��
    //                //transform.position = new Vector2(otherZombie.transform.position.x - 0.5f, transform.position.y);
    //            }
    //        }
    //    }
    //}
}
