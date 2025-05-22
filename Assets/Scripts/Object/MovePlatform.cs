using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [Header("이동 지점")]
    [SerializeField] private List<Transform> waypoints; // 멈추는 위치

    [Header("이동 설정")]
    [SerializeField] private float speed = 5f; // 속도
    [SerializeField] private float waitTime = 1f; // 기다리는 시간

    [field: SerializeField] public bool IsActive { get; set; } = false;


    private int currentIndex = 0; // 현재 인덱스
    private float waitCounter = 0f;

    private void Awake()
    {
        if (waypoints == null || waypoints.Count < 2)
        {
            Debug.LogError($"{name}: waypoints에 최소 2개 이상의 Transform을 등록하세요.");
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
        if (waitCounter > 0f) // 도착시 기다리는 시간 계산
        {
            waitCounter -= Time.deltaTime;
            return;
        }

        // 목표 웨이 포인트 지정 (로컬 좌표 사용)
        Vector3 targetLocalPos = waypoints[currentIndex].localPosition;

        // 플랫폼의 위치를 로컬 좌표로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetLocalPos, speed * Time.deltaTime);

        // 거의 도착하면 대기 후 다음 인덱스 계산 (로컬 좌표 기준 거리)
        // 0.01f는 충분히 작은 값이어야 합니다.
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
            // 플레이어가 플랫폼 위에 있는지 확인 (월드 Y를 비교하는 것이 가장 안정적)
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