using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class LineManager : MonoBehaviour
{
    // 라인별 몬스터 리스트
    public List<Monster> line1 = new List<Monster>();
    public List<Monster> line2 = new List<Monster>();
    public List<Monster> line3 = new List<Monster>();

    // 액션 선언
    public UnityAction<Monster> AddMonster;
    public UnityAction<Monster> RemoveMonster;

    private void OnEnable()
    {
        // AddMonster와 RemoveMonster 액션 구독
        AddMonster += HandleAddMonster;
        RemoveMonster += HandleRemoveMonster;
    }

    private void OnDisable()
    {
        // 구독 해제
        AddMonster -= HandleAddMonster;
        RemoveMonster -= HandleRemoveMonster;
    }

    // 몬스터 추가 처리 (라인에 몬스터 추가)
    private void HandleAddMonster(Monster monster)
    {
        // 예시로 line1에 추가 (선택적으로 라인 결정할 수 있음)
        if (monster.line == 1)
        {
            line1.Add(monster);
        }
        else if (monster.line == 2)
        {
            line2.Add(monster);
        }
        else if (monster.line == 3)
        {
            line3.Add(monster);
        }

        Debug.Log($"{monster.name} 몬스터가 추가되었습니다.");
    }

    // 몬스터 제거 처리 (라인에서 몬스터 제거)
    private void HandleRemoveMonster(Monster monster)
    {
        if (monster.line == 1)
        {
            line1.Remove(monster);
        }
        else if (monster.line == 2)
        {
            line2.Remove(monster);
        }
        else if (monster.line == 3)
        {
            line3.Remove(monster);
        }

        Debug.Log($"{monster.name} 몬스터가 제거되었습니다.");
    }

    // 주어진 몬스터가 속한 라인 리스트를 반환하는 함수
    public List<Monster> GetLineListForMonster(Monster monster)
    {
        if (monster.line == 1)
        {
            return line1;
        }
        else if (monster.line == 2)
        {
            return line2;
        }
        else if (monster.line == 3)
        {
            return line3;
        }
        else
        {
            Debug.LogWarning("Invalid line for this monster.");
            return null;
        }
    }
}
