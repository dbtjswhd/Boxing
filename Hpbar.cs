using UnityEngine;
using UnityEngine.UI;

public class Hpbar : MonoBehaviour
{
    public Slider HpBarSlider;  // 연결된 Slider
    public float curHp;         // 현재 체력
    public float maxHp;         // 최대 체력
    public Animator animator;
    public PlayerMove playerMove;
    public NPCMove nPCMove;
    public Combo combo;
    public GameObject gameOverUI;
    
    public bool onRageArt = false;
    public bool useRageArt = false;

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

        if(onRageArt == false)
        {
            onRageArt = Ragearts(); // Rage Arts 상태 확인
            useRageArt = false;
        }
        
        Down();     // 체력 0일 때 Down 함수 실행
    }
    
    public bool checkRage()
    {
        if( onRageArt && !useRageArt)
            return true;
        else
            return false;
    }
    // Rage Arts 상태를 확인하는 함수 (체력이 일정 이하일 때 색상 변경)
    public bool Ragearts()
    {

        if (curHp < 40 && HpBarSlider != null)
        {
            Image fillImage = HpBarSlider.fillRect.GetComponent<Image>();
            
            if (fillImage != null)
            {
                fillImage.color = Color.blue;  // Change slider color to blue
                return true;  // Return true if conditions are met
            }
        }

        return false;  // Return false if conditions are not met
    }      


    // Start 함수: 초기 설정
    void Start()
    {
        gameOverUI.SetActive(false);

        if (HpBarSlider != null)
        {
            SetHp(maxHp);  // 최대 체력을 설정하고 슬라이더 값도 초기화
        }

        onRageArt = false;
        useRageArt = false;
    }

    // Update 함수: 매 프레임마다 호출
    void Update()
    {
        // 테스트용: Space 키를 누르면 10의 데미지를 받음
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Damage(10);
        }
    }

    // 체력이 0일 때 애니메이션 트리거 설정
    public void Down()
    {
        if (curHp <= 0 && animator != null)
        {
            animator.SetTrigger("isDie");
            playerMove.enabled = false;
            nPCMove.enabled = false;
            combo.enabled = false;
            gameOverUI.SetActive(true);
        }
    }
}
