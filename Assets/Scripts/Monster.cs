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
    private float verticalVelocity = 0f; // 수직 속도

    private Transform targetMonster;  // 점프할 몬스터의 위치
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

        int layerMask = detectionLayer; // 필요한 레이어만 감지

        // 왼쪽 방향으로 RaycastAll 발사
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.left, detectionDistance, layerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) // 자기 자신 제외
            {
                Monster detectedMonster = hit.collider.GetComponent<Monster>();
                if (detectedMonster != null && detectedMonster.line == line)
                {
                    PerformAction(hit.collider.transform);
                    if (Mathf.Abs(transform.position.x - hit.collider.transform.position.x) < 4f)
                    {
                        canMove = false;
                    }
                    break; // 가장 가까운 오브젝트만 감지하고 루프 종료
                }
            }
        }

        Debug.DrawRay(rayOrigin, Vector2.left * detectionDistance, Color.red);
    }

    void PerformAction(Transform monster)
    {
        // 몬스터 위로 점프 시작
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
            // 목표 몬스터 위치로 점프
            Vector3 targetPosition = targetMonster.position + new Vector3(0, 1, 0); // 몬스터 위로 1 유닛 올라가게

            // 점프 중인 상태로 상승
            transform.position += new Vector3(0, verticalVelocity * Time.deltaTime, 0);

            // 중력 적용
            verticalVelocity += gravity * Time.deltaTime;

            // 목표 위치에 도달하거나, 더 이상 올라가지 않으면 멈춤
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
        // Monster가 파괴될 때 RemoveMonster 액션을 호출하여 제거
        lineManager.RemoveMonster?.Invoke(this);
        Debug.Log($"{gameObject.name} 파괴되어 리스트에서 제거됩니다.");
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
    //    // 다른 좀비와 충돌 시 넘기기
    //    if (collision.gameObject.CompareTag("Monster"))
    //    {
    //        Monster otherZombie = collision.gameObject.GetComponent<Monster>();
    //        if (otherZombie != null)
    //        {
    //            //충돌한 다른 좀비가 앞에 있으면 넘어서기
    //            if (otherZombie.transform.position.x < transform.position.x && otherZombie.line == line)
    //            {
    //                canJump = true;
    //                canMove = false;
    //                //다른 좀비가 앞에 있으면 넘어갈 수 있도록 처리
    //                //transform.position = new Vector2(otherZombie.transform.position.x - 0.5f, transform.position.y);
    //            }
    //        }
    //    }
    //}
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    // 다른 좀비와 충돌 시 넘기기
    //    if (other.CompareTag("Monster"))
    //    {
    //        Monster otherZombie = other.GetComponent<Monster>();
    //        if (otherZombie != null)
    //        {
    //            // 충돌한 다른 좀비가 앞에 있으면 넘어서기
    //            if (otherZombie.transform.position.x < transform.position.x && otherZombie.line == line)
    //            {
    //                canJump = true;
    //                canMove = false;
    //                // 다른 좀비가 앞에 있으면 넘어갈 수 있도록 처리
    //                //transform.position = new Vector2(otherZombie.transform.position.x - 0.5f, transform.position.y);
    //            }
    //        }
    //    }
    //}
}
