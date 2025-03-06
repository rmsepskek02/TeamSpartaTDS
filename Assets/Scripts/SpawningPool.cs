using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class SpawningPool : MonoBehaviour
{
    public Camera mainCamera;
    public float lastSpawnTime;
    public float spawnDelay;
    public Monster zombie;
    public float[] spawnYPos = new float[] { -2.2f, -2.9f, -3.6f };
    public int num = 0;
    public LineManager lineManager;
    // Start is called before the first frame update
    void Start()
    {
        spawnDelay = 3f;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        transform.position = new Vector2(mainCamera.transform.position.x + 5, -2.9f);
        if (lineManager == null)
        {
            lineManager = FindObjectOfType<LineManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpawnMonster(zombie);
    }

    public void SpawnMonster(Monster prefab)
    {
        if ((Time.time - lastSpawnTime) >= spawnDelay)
        {
            int randomIndex = Random.Range(0, spawnYPos.Length);
            float yValue = spawnYPos[randomIndex];
            Vector3 spawnPos = new Vector3(transform.position.x, yValue, randomIndex);

            // 몬스터를 인스턴스화
            Monster go = Instantiate(prefab, spawnPos, Quaternion.identity);
            go.GetComponent<Monster>().line = randomIndex;
            go.gameObject.name += num;
            num++;

            // LineManager에 몬스터 추가
            lineManager.AddMonster?.Invoke(go);

            // 각 SpriteRenderer의 sortingOrder 수정
            SpriteRenderer[] spriteRenderers = go.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sprite in spriteRenderers)
            {
                // 기존 sortingOrder 값을 가져오기
                int baseSortingOrder = sprite.sortingOrder;

                // line 값에 맞춰 sortingOrder를 계산
                sprite.sortingOrder = (randomIndex * 10) + baseSortingOrder;
            }

            // 마지막 스폰 시간 갱신
            lastSpawnTime = Time.time;
        }
    }
}
