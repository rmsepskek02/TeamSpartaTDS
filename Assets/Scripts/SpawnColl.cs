using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnColl : MonoBehaviour
{
    public Camera mainCamera;
    public float lastSpawnTime;
    public float spawnDelay;
    public GameObject zombie;
    public int num;
    public float[] spawnYPos = new float[] { -3.4f, -3.0f, -2.6f };
    // Start is called before the first frame update
    void Start()
    {
        spawnDelay = 2f;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        transform.position = new Vector2(mainCamera.transform.position.x + 7, -2.9f);
    }

    // Update is called once per frame
    void Update()
    {
        SpawnMonster(zombie);
    }

    public void SpawnMonster(GameObject prefab)
    {
        if ((Time.time - lastSpawnTime) >= spawnDelay)
        {
            int randomIndex = Random.Range(0, spawnYPos.Length);
            float yValue = spawnYPos[randomIndex];
            Vector3 spawnPos = new Vector3(transform.position.x, yValue, randomIndex*2);

            GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
            go.gameObject.name += num;
            num++;

            // 각 SpriteRenderer의 sortingOrder 수정
            SpriteRenderer[] spriteRenderers = go.GetComponentsInChildren<SpriteRenderer>();

            // 1. 기준이 되는 z값 기반 sortingOrder 설정
            int baseSortingOrder = Mathf.RoundToInt(go.transform.position.z * -100);
            foreach (SpriteRenderer sprite in spriteRenderers)
            {
                int localSortingOrder = sprite.sortingOrder; // 기존 값 유지
                sprite.sortingOrder = baseSortingOrder * 10 + localSortingOrder;
            }

            // 마지막 스폰 시간 갱신
            lastSpawnTime = Time.time;
        }
    }
}
