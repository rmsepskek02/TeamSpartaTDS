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
    public float leftDetectRayDistance;
    public float belowDetectRayDistance;
    public LayerMask detectionLayer;
    public bool canJump;
    public LineManager lineManager;
    public List<Monster> lineOfMonster;
    private bool hasLanded = false;
    private float verticalVelocity = 0f; // ���� �ӵ�

    private Transform leftMonster;      // ���� ������ ��ġ
    private Transform belowMonster;     // �Ʒ��� ������ ��ġ
    private Transform rightMonster;     // ������ ������ ��ġ
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
        leftDetectRayDistance = 0.4f;
        belowDetectRayDistance = 0.5f;
        lineOfMonster = lineManager.GetLineListForMonster(this);
    }

    void Update()
    {
        UpdateMoveSpeed();
        MoveToLeft();

        if (isJumping)
        {
            JumpOverMonster();
        }

        DetectObjectOnLeft();
        DetectObjectOnLand();
        //CheckDistanceHero();
    }
    public void UpdateMoveSpeed()
    {
        // �⺻ �̵� �ӵ� ����
        moveSpeed = 2f;

        // ���ʿ� ���Ͱ� ������ ����
        if (leftMonster != null || CheckDistanceHero())
        {
            moveSpeed = 0f;
        }

        // ���� ���̶�� �̵� �ӵ��� ���� ����
        if (isJumping)
        {
            moveSpeed += Time.deltaTime * 10f; // ���������� ����
        }

        // ������ �����Ͽ� ���� ��ġ�� ���ư��� ���̶�� �̵� �ӵ��� �ٽ� ����
        if (!isJumping && hasLanded == false)
        {
            moveSpeed -= Time.deltaTime * 3f; // �ӵ��� ���ҽ�Ű�鼭 ���� ��ġ�� ����
            if (moveSpeed < 0f) moveSpeed = 0f; // ���� ����
        }
    }

    public void MoveToLeft()
    {
        Vector2 moveDir = Vector2.left;
        transform.position += (Vector3)moveDir * moveSpeed * Time.deltaTime;
    }

    public bool CheckDistanceHero()
    {
        if (Vector2.Distance(new Vector2(transform.position.x, 0), heroXPos) < stopDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void DetectObjectOnLeft()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x - 0.2f, transform.position.y + 0.4f);

        int layerMask = detectionLayer; // �ʿ��� ���̾ ����

        // ���� �������� RaycastAll �߻�
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.left, leftDetectRayDistance, layerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) // �ڱ� �ڽ� ����
            {
                Monster detectedMonster = hit.collider.GetComponent<Monster>();
                if (detectedMonster != null && detectedMonster.line == line)
                {
                    Jump(hit.collider.transform);
                    if (Mathf.Abs(transform.position.x - hit.collider.transform.position.x) < 1f)
                    {
                        moveSpeed = 0;
                    }
                    break; // ���� ����� ������Ʈ�� �����ϰ� ���� ����
                }
            }
        }

        Debug.DrawRay(rayOrigin, Vector2.left * leftDetectRayDistance, Color.red);
    }
    void DetectObjectOnLand()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x - 0.2f, transform.position.y + 0.4f);

        int layerMask = detectionLayer; // �ʿ��� ���̾ ����

        // ���� �������� RaycastAll �߻�
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, leftDetectRayDistance, layerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) // �ڱ� �ڽ� ����
            {
                Monster detectedMonster = hit.collider.GetComponent<Monster>();
                if (detectedMonster != null && detectedMonster.line == line)
                {
                    Push(hit.collider.transform);
                    //canMove = false;
                    break; // ���� ����� ������Ʈ�� �����ϰ� ���� ����
                }
            }
        }

        Debug.DrawRay(rayOrigin, Vector2.down * leftDetectRayDistance, Color.red);
    }

    void Jump(Transform monster)
    {
        // ���� ���� ���� ����
        leftMonster = monster;
        isJumping = true;
        hasLanded = false;
        verticalVelocity = jumpSpeed;
    }

    void Push(Transform monster)
    {
        // �Ʒ� ���� �о��
        belowMonster = monster;
        isJumping = false;
        //hasLanded = false;
        //verticalVelocity = jumpSpeed;
    }

    void JumpOverMonster()
    {
        if (leftMonster != null)
        {
            Vector3 targetCollSize = leftMonster.GetComponent<BoxCollider2D>().size;
            Vector3 targetPosition = leftMonster.position + new Vector3(0, targetCollSize.y, 0);

            // X�� �̵��� �߰�
            transform.position += new Vector3(-moveSpeed * Time.deltaTime, verticalVelocity * Time.deltaTime, 0);

            verticalVelocity += gravity * Time.deltaTime;

            // ��ǥ ���̿� �������� ���ϰ� �ٽ� �϶��ϴ� ���
            if (verticalVelocity < 0 && transform.position.y <= leftMonster.position.y)
            {
                isJumping = false;
                hasLanded = false;
                verticalVelocity = 0;
                moveSpeed = 0; // ���� ���� �� �̵� �ӵ��� ����
            }

            // ��ǥ ��ġ�� �����ϸ� ����
            if (transform.position.y >= targetPosition.y)
            {
                transform.position = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
                hasLanded = true;
                isJumping = false;
            }
        }
    }



    private void OnDestroy()
    {
        // Monster�� �ı��� �� RemoveMonster �׼��� ȣ���Ͽ� ����
        lineManager.RemoveMonster?.Invoke(this);
    }
}
