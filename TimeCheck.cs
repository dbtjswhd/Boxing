using UnityEngine;
using UnityEngine.UI;

public class TinmeCheck : MonoBehaviour
{
    public Text timerText; // 타이머를 표시할 UI 텍스트
    private float currentTime = 0f; // 현재 타이머 시간
    private bool isTimerRunning = false; // 타이머 작동 상태

    void Start()
    {
        StartTimer(); // 게임 시작 시 타이머 시작
    }

    void Update()
    {
        if (isTimerRunning)
        {
            UpdateTimer(); // 매 프레임 타이머 갱신
        }
    }

    // 타이머 시작
    public void StartTimer()
    {
        currentTime = 0f;
        isTimerRunning = true;
        UpdateTimerText(); // 초기화된 타이머 UI 업데이트
    }

    // 타이머 멈춤
    public void StopTimer()
    {
        isTimerRunning = false;
    }

    // 타이머를 매 프레임 증가시키는 함수
    private void UpdateTimer()
    {
        currentTime += Time.deltaTime; // 시간 증가
        UpdateTimerText(); // 타이머 UI 업데이트
    }

    // UI에 시간을 표시하는 함수
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}"; // "MM:SS" 형식으로 표시
    }

    // 게임 종료 후 타이머를 재시작할 때 사용하는 함수
    public void ResetAndStartTimer()
    {
        currentTime = 0f;
        isTimerRunning = true;
        UpdateTimerText(); // 초기화된 타이머 UI 업데이트
    }
}
