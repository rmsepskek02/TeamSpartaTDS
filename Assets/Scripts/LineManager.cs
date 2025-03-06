using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class LineManager : MonoBehaviour
{
    // ���κ� ���� ����Ʈ
    public List<Monster> line1 = new List<Monster>();
    public List<Monster> line2 = new List<Monster>();
    public List<Monster> line3 = new List<Monster>();

    // �׼� ����
    public UnityAction<Monster> AddMonster;
    public UnityAction<Monster> RemoveMonster;

    private void OnEnable()
    {
        // AddMonster�� RemoveMonster �׼� ����
        AddMonster += HandleAddMonster;
        RemoveMonster += HandleRemoveMonster;
    }

    private void OnDisable()
    {
        // ���� ����
        AddMonster -= HandleAddMonster;
        RemoveMonster -= HandleRemoveMonster;
    }

    // ���� �߰� ó�� (���ο� ���� �߰�)
    private void HandleAddMonster(Monster monster)
    {
        // ���÷� line1�� �߰� (���������� ���� ������ �� ����)
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

        Debug.Log($"{monster.name} ���Ͱ� �߰��Ǿ����ϴ�.");
    }

    // ���� ���� ó�� (���ο��� ���� ����)
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

        Debug.Log($"{monster.name} ���Ͱ� ���ŵǾ����ϴ�.");
    }

    // �־��� ���Ͱ� ���� ���� ����Ʈ�� ��ȯ�ϴ� �Լ�
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
