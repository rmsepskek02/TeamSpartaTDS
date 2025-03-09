using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    public float speed = 2f;  // 일정한 이동 속도
    public float jumpHeight = 0.8f; // 좀비가 올라타는 높이 (기존 1.2 → 0.8로 조정)
    public float stopDistance = 0.2f; // 장애물과 멈출 거리
    public LayerMask zombieLayer; // 다른 좀비 감지 레이어
    public LayerMask obstacleLayer; // 장애물(박스) 감지 레이어

    private bool isStacked = false; // 스택 상태 확인
    private bool isStopped = false; // 이동 멈춤 여부
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-speed, 0); // ? 왼쪽 이동, Y축 속도 제거
    }

    void Update()
    {
        if (isStacked || isStopped) return; // 스택되거나 멈춘 경우 이동 X

        DetectObstacle(); // 앞에 장애물 있는지 확인
        DetectZombie(); // 앞에 좀비 있는지 확인

        // ? Y축 속도가 이상하면 자동 리셋 (공중으로 튀는 문제 해결)
        if (Mathf.Abs(rb.velocity.y) > 0.5f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    void DetectObstacle()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, stopDistance, obstacleLayer);

        if (hit.collider != null) // 장애물(박스) 감지됨
        {
            StopAndStack(); // 멈추고 스택 시도
        }
    }

    void DetectZombie()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, stopDistance, zombieLayer);

        if (hit.collider != null && !isStacked) // 앞에 좀비가 있고, 스택되지 않았다면
        {
            TryJump(); // 점프 시도
        }
    }

    void StopAndStack()
    {
        isStopped = true;
        rb.velocity = Vector2.zero; // ? 이동 멈춤

        RaycastHit2D belowHit = Physics2D.Raycast(transform.position, Vector2.down, 1f, zombieLayer);

        if (belowHit.collider != null) // ? 아래에 다른 좀비가 있을 때만 올라감
        {
            isStacked = true;
            transform.position += new Vector3(0, jumpHeight, 0); // ? 과도한 상승 방지
        }
    }

    void TryJump()
    {
        // 앞에 좀비가 있을 때만 점프
        RaycastHit2D frontHit = Physics2D.Raycast(transform.position, Vector2.left, stopDistance, zombieLayer);

        if (frontHit.collider != null) // 앞에 좀비가 있을 경우에만 점프
        {
            rb.velocity = new Vector2(-speed, 0.5f); // ? Y값을 제한하여 너무 높이 점프하지 않도록 수정
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle")) // ? 박스에 닿으면 점프 ?, 그냥 멈추기
        {
            StopAndStack();
        }
    }
}
