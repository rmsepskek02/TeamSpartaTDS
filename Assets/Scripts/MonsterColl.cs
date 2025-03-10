using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterColl : MonoBehaviour
{
    public GameObject hero;
    public Vector2 heroXPos;
    public bool canMove = true;
    public float moveSpeed;
    public LayerMask detectionLayer;
    public float leftDetectRayDistance;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        if (hero == null)
        {
            hero = GameObject.FindGameObjectWithTag("Hero");
            heroXPos = new Vector2(hero.transform.position.x, 0);
        }
        moveSpeed = 2f;
        leftDetectRayDistance = 0.5f;
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
        DetectObjectOnLeft();
    }
    public void MoveToLeft()
    {
        Vector3 moveDir = Vector3.left * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDir);
    }

    void DetectObjectOnLeft()
    {
        Vector3 rayOrigin = new Vector3(transform.position.x - 0.2f, transform.position.y + 0.4f, transform.position.z);

        int layerMask = detectionLayer;

        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.left, leftDetectRayDistance, layerMask);


        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject) // 자기 자신 제외
            {
                MonsterColl detectedMonster = hit.collider.GetComponent<MonsterColl>();
                if (detectedMonster != null)
                {
                    // 감지된 몬스터와 현재 몬스터의 Z축 비교 (같은 Z축만 감지)
                    if (Mathf.Abs(transform.position.z - detectedMonster.transform.position.z) < 0.1f)
                    {
                        float targetY = detectedMonster.transform.position.y + 1f;
                        Vector3 newPos = new Vector3(transform.position.x, targetY, transform.position.z);

                        rb.MovePosition(Vector3.Lerp(transform.position, newPos, Time.fixedDeltaTime * 30f));
                    }
                    break; // 가장 가까운 오브젝트만 감지하고 루프 종료
                }
            }
        }

        Debug.DrawRay(rayOrigin, Vector3.left * leftDetectRayDistance, Color.red);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            canMove = false;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            canMove = true;
        }
    }
}
