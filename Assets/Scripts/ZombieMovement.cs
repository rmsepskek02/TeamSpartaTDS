using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    public float speed = 2f;  // ������ �̵� �ӵ�
    public float jumpHeight = 0.8f; // ���� �ö�Ÿ�� ���� (���� 1.2 �� 0.8�� ����)
    public float stopDistance = 0.2f; // ��ֹ��� ���� �Ÿ�
    public LayerMask zombieLayer; // �ٸ� ���� ���� ���̾�
    public LayerMask obstacleLayer; // ��ֹ�(�ڽ�) ���� ���̾�

    private bool isStacked = false; // ���� ���� Ȯ��
    private bool isStopped = false; // �̵� ���� ����
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-speed, 0); // ? ���� �̵�, Y�� �ӵ� ����
    }

    void Update()
    {
        if (isStacked || isStopped) return; // ���õǰų� ���� ��� �̵� X

        DetectObstacle(); // �տ� ��ֹ� �ִ��� Ȯ��
        DetectZombie(); // �տ� ���� �ִ��� Ȯ��

        // ? Y�� �ӵ��� �̻��ϸ� �ڵ� ���� (�������� Ƣ�� ���� �ذ�)
        if (Mathf.Abs(rb.velocity.y) > 0.5f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    void DetectObstacle()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, stopDistance, obstacleLayer);

        if (hit.collider != null) // ��ֹ�(�ڽ�) ������
        {
            StopAndStack(); // ���߰� ���� �õ�
        }
    }

    void DetectZombie()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, stopDistance, zombieLayer);

        if (hit.collider != null && !isStacked) // �տ� ���� �ְ�, ���õ��� �ʾҴٸ�
        {
            TryJump(); // ���� �õ�
        }
    }

    void StopAndStack()
    {
        isStopped = true;
        rb.velocity = Vector2.zero; // ? �̵� ����

        RaycastHit2D belowHit = Physics2D.Raycast(transform.position, Vector2.down, 1f, zombieLayer);

        if (belowHit.collider != null) // ? �Ʒ��� �ٸ� ���� ���� ���� �ö�
        {
            isStacked = true;
            transform.position += new Vector3(0, jumpHeight, 0); // ? ������ ��� ����
        }
    }

    void TryJump()
    {
        // �տ� ���� ���� ���� ����
        RaycastHit2D frontHit = Physics2D.Raycast(transform.position, Vector2.left, stopDistance, zombieLayer);

        if (frontHit.collider != null) // �տ� ���� ���� ��쿡�� ����
        {
            rb.velocity = new Vector2(-speed, 0.5f); // ? Y���� �����Ͽ� �ʹ� ���� �������� �ʵ��� ����
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle")) // ? �ڽ��� ������ ���� ?, �׳� ���߱�
        {
            StopAndStack();
        }
    }
}
