using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [Header("�̵� ����")]
    [SerializeField] private List<Transform> waypoints; // ���ߴ� ��ġ

    [Header("�̵� ����")]
    [SerializeField] private float speed = 5f; // �ӵ�
    [SerializeField] private float waitTime = 1f; // ��ٸ��� �ð�

    [field: SerializeField] public bool IsActive { get; set; } = false;


    private int currentIndex = 0; // ���� �ε���
    private float waitCounter = 0f;

    private void Awake()
    {
        if (waypoints == null || waypoints.Count < 2)
        {
            Debug.LogError($"{name}: waypoints�� �ּ� 2�� �̻��� Transform�� ����ϼ���.");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (IsActive) Move();
    }

    private void Move()
    {
        if (waitCounter > 0f) // ������ ��ٸ��� �ð� ���
        {
            waitCounter -= Time.deltaTime;
            return;
        }

        // ��ǥ ���� ����Ʈ ���� (���� ��ǥ ���)
        Vector3 targetLocalPos = waypoints[currentIndex].localPosition;

        // �÷����� ��ġ�� ���� ��ǥ�� �̵�
        transform.position = Vector3.MoveTowards(transform.position, targetLocalPos, speed * Time.deltaTime);

        // ���� �����ϸ� ��� �� ���� �ε��� ��� (���� ��ǥ ���� �Ÿ�)
        // 0.01f�� ����� ���� ���̾�� �մϴ�.
        if (Vector3.Distance(transform.localPosition, targetLocalPos) < 0.01f)
        {
            waitCounter = waitTime;
            currentIndex = (currentIndex + 1) % waypoints.Count;
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // �÷��̾ �÷��� ���� �ִ��� Ȯ�� (���� Y�� ���ϴ� ���� ���� ������)
            if (col.transform.position.y > transform.position.y)
                col.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        col.transform.SetParent(null);
    }
}