using UnityEngine;

public class TacticalUI : MonoBehaviour
{
    public Transform player;
    public Transform enemy;
    public float viewAngle = 45f; // 정면 기준 좌우 45도 (총 시야각 90도)

    void Update()
    {
        if (enemy == null || player == null) return;

        if (IsEnemyInFrontLeft())
        {
            Debug.Log("경고: 좌측 전방에 적 출현!");
        }
    }

    private bool IsEnemyInFrontLeft()
    {
        // 1. 플레이어에서 적을 향하는 방향 벡터 구하기 및 정규화
        
        Vector3 dir = enemy.transform.position - player.transform.position;
        dir.Normalize();
        // 2. 내적(Dot)을 사용하여 전방 시야각(±45도) 이내인지 판별
        // (내적값이 기준 코사인 값보다 크면 시야 내)
        
        float dot = Vector3.Dot(dir, player.transform.forward);
        if (dot < Mathf.Cos(Mathf.Deg2Rad * viewAngle)) return false;
        
        // 3. 외적(Cross)을 사용하여 적이 왼쪽에 있는지 판별
        // (외적 벡터의 Y값이 음수면 왼쪽)
        Vector3 cross = Vector3.Cross(player.transform.forward,dir);
        if (cross.y >= 0) return false;

        // 4. 두 조건(시야 내 AND 왼쪽)을 모두 만족할 경우 true 반환
        return true;
    }
}