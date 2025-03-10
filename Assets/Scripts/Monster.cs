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
    private float verticalVelocity = 0f; // 수직 속도

    private Transform leftMonster;      // 왼쪽 몬스터의 위치
    private Transform belowMonster;     // 아래쪽 몬스터의 위치
    private Transform rightMonster;     // 오른쪽 몬스터의 위치
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
        // 기본 이동 속도 설정
        moveSpeed = 2f;

        // 왼쪽에 몬스터가 있으면 멈춤
        if (leftMonster != null || CheckDistanceHero())
        {
            moveSpeed = 0f;
        }

        // 점프 중이라면 이동 속도를 점차 증가
        if (isJumping)
        {
            moveSpeed += Time.deltaTime * 10f; // 점진적으로 증가
        }

        // 점프가 실패하여 원래 위치로 돌아가는 중이라면 이동 속도를 다시 감소
        if (!isJumping && hasLanded == false)
        {
            moveSpeed -= Time.deltaTime * 3f; // 속도를 감소시키면서 원래 위치로 복귀
            if (moveSpeed < 0f) moveSpeed = 0f; // 음수 방지
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

        int layerMask = detectionLayer; // 필요한 레이어만 감지

        // 왼쪽 방향으로 RaycastAll 발사
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.left, leftDetectRayDistance, layerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) // 자기 자신 제외
            {
                Monster detectedMonster = hit.collider.GetComponent<Monster>();
                if (detectedMonster != null && detectedMonster.line == line)
                {
                    Jump(hit.collider.transform);
                    if (Mathf.Abs(transform.position.x - hit.collider.transform.position.x) < 1f)
                    {
                        moveSpeed = 0;
                    }
                    break; // 가장 가까운 오브젝트만 감지하고 루프 종료
                }
            }
        }

        Debug.DrawRay(rayOrigin, Vector2.left * leftDetectRayDistance, Color.red);
    }
    void DetectObjectOnLand()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x - 0.2f, transform.position.y + 0.4f);

        int layerMask = detectionLayer; // 필요한 레이어만 감지

        // 왼쪽 방향으로 RaycastAll 발사
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, leftDetectRayDistance, layerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) // 자기 자신 제외
            {
                Monster detectedMonster = hit.collider.GetComponent<Monster>();
                if (detectedMonster != null && detectedMonster.line == line)
                {
                    Push(hit.collider.transform);
                    //canMove = false;
                    break; // 가장 가까운 오브젝트만 감지하고 루프 종료
                }
            }
        }

        Debug.DrawRay(rayOrigin, Vector2.down * leftDetectRayDistance, Color.red);
    }

    void Jump(Transform monster)
    {
        // 몬스터 위로 점프 시작
        leftMonster = monster;
        isJumping = true;
        hasLanded = false;
        verticalVelocity = jumpSpeed;
    }

    void Push(Transform monster)
    {
        // 아래 몬스터 밀어내기
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

            // X축 이동도 추가
            transform.position += new Vector3(-moveSpeed * Time.deltaTime, verticalVelocity * Time.deltaTime, 0);

            verticalVelocity += gravity * Time.deltaTime;

            // 목표 높이에 도달하지 못하고 다시 하락하는 경우
            if (verticalVelocity < 0 && transform.position.y <= leftMonster.position.y)
            {
                isJumping = false;
                hasLanded = false;
                verticalVelocity = 0;
                moveSpeed = 0; // 점프 실패 시 이동 속도를 멈춤
            }

            // 목표 위치에 도달하면 착지
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
        // Monster가 파괴될 때 RemoveMonster 액션을 호출하여 제거
        lineManager.RemoveMonster?.Invoke(this);
    }
}
