using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class LineManager : MonoBehaviour
{
    // ���κ� ���� ����Ʈ
    public List<Monster> line0 = new List<Monster>();
    public List<Monster> line1 = new List<Monster>();
    public List<Monster> line2 = new List<Monster>();

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
        // ���÷� line0�� �߰� (���������� ���� ������ �� ����)
        if (monster.line == 0)
        {
            line0.Add(monster);
        }
        else if (monster.line == 1)
        {
            line1.Add(monster);
        }
        else if (monster.line == 2)
        {
            line2.Add(monster);
        }
    }

    // ���� ���� ó�� (���ο��� ���� ����)
    private void HandleRemoveMonster(Monster monster)
    {
        if (monster.line == 0)
        {
            line0.Remove(monster);
        }
        else if (monster.line == 1)
        {
            line1.Remove(monster);
        }
        else if (monster.line == 2)
        {
            line2.Remove(monster);
        }
    }

    // �־��� ���Ͱ� ���� ���� ����Ʈ�� ��ȯ�ϴ� �Լ�
    public List<Monster> GetLineListForMonster(Monster monster)
    {
        if (monster.line == 0)
        {
            return line0;
        }
        else if (monster.line == 1)
        {
            return line1;
        }
        else if (monster.line == 2)
        {
            return line2;
        }
        else
        {
            Debug.LogWarning("Invalid line for this monster.");
            return null;
        }
    }
}
