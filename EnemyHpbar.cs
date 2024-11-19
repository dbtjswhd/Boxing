using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpbar : MonoBehaviour
{
    public Slider HpBarSlider;  // 연결된 Slider
    public float curHp;         // 현재 체력
    public float maxHp;         // 최대 체력
    public Animator animator;

    // HP 최대치와 현재치를 설정하는 함수
    public void SetHp(float amount)
    {
        maxHp = amount;
        curHp = maxHp;

        // 슬라이더의 최대값과 현재값을 설정
        if (HpBarSlider != null)
        {
            HpBarSlider.maxValue = maxHp;
            HpBarSlider.value = curHp;
        }
    }

    // 체력 바를 갱신하는 함수
    public void CheckHp()
    {
        if (HpBarSlider != null)
        {
            HpBarSlider.value = curHp;  // 체력 비율에 맞춰 슬라이더 값 변경
        }
    }

    // 데미지를 받는 함수
    public void Damage(float damage)
    {
        if (curHp <= 0)  // 체력이 이미 0 이하이면 함수 종료
            return;

        

        curHp -= damage;
        curHp = Mathf.Clamp(curHp, 0, maxHp);  // 체력 값이 0 이하로 내려가지 않도록 제한

        CheckHp();  // 체력 바 갱신
        Down();     // 사망 여부 확인 및 처리
    }

    // 체력이 0 이하일 때 사망 애니메이션을 재생하는 함수
    public void Down()
    {
        if (curHp <= 0)
        {
            animator.SetTrigger("isDie");
        }
    }

    void Start()
    {
        if (HpBarSlider != null)
        {
            SetHp(maxHp);  // 최대 체력을 설정하고 슬라이더 값도 초기화
        }
    }

    void Update()
    {
        
        Ragearts();  // Rage Arts 상태를 매 프레임 확인
    }

    // Rage Arts 상태를 확인하는 함수 (체력이 일정 이하일 때 색상 변경)
    public void Ragearts()
    {
        if (curHp < 40 && HpBarSlider != null)
        {
            HpBarSlider.fillRect.GetComponent<Image>().color = Color.blue;  // 슬라이더 색상 변경
        }
    }
}
