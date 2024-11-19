using UnityEngine;

public class Cam : MonoBehaviour
{
    public Transform player1; // 플레이어 1의 Transform
    public Transform player2; // 플레이어 2의 Transform
    public float cameraDistance = 10.0f; // 카메라와 플레이어 간 기본 거리
    public float height = 5.0f; // 카메라 높이
    public float smoothSpeed = 0.125f; // 카메라 이동 부드러움 조정
    public float rotationSpeed = 5.0f; // 카메라 회전 속도

    void LateUpdate()
    {
        // 두 플레이어의 중간 지점 계산
        Vector3 middlePoint = (player1.position + player2.position) / 2;

        // 플레이어 간 거리 계산
        float distance = Vector3.Distance(player1.position, player2.position);

        // 카메라 위치 계산 (중간 지점에서 카메라Distance와 거리 기반으로 뒤로 이동)
        Vector3 desiredPosition = middlePoint - transform.forward * (cameraDistance + distance * 0.5f);
        desiredPosition.y = middlePoint.y + height;

        // 부드럽게 카메라 위치 이동
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 두 플레이어의 중간 지점을 향해 카메라 회전
        Quaternion targetRotation = Quaternion.LookRotation(middlePoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}