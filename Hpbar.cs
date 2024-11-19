using UnityEngine;
using UnityEngine.UI;

public class Hpbar : MonoBehaviour
{
    public Slider HpBarSlider;  // ����� Slider
    public float curHp;         // ���� ü��
    public float maxHp;         // �ִ� ü��
    public Animator animator;
    public PlayerMove playerMove;
    public NPCMove nPCMove;
    public Combo combo;
    public GameObject gameOverUI;
    
    public bool onRageArt = false;
    public bool useRageArt = false;

    // HP �ִ�ġ�� ����ġ�� �����ϴ� �Լ�
    public void SetHp(float amount)
    {
        maxHp = amount;
        curHp = maxHp;

        // �����̴��� �ִ밪�� ���簪�� ����
        if (HpBarSlider != null)
        {
            HpBarSlider.maxValue = maxHp;
            HpBarSlider.value = curHp;
        }
    }

    // ü�� �ٸ� �����ϴ� �Լ�
    public void CheckHp()
    {
        if (HpBarSlider != null)
        {
            HpBarSlider.value = curHp;  // ü�� ������ ���� �����̴� �� ����
        }
    }

    // �������� �޴� �Լ�
    public void Damage(float damage)
    {
        if (curHp <= 0)  // ü���� �̹� 0 �����̸� �Լ� ����
            return;

        curHp -= damage;
        curHp = Mathf.Clamp(curHp, 0, maxHp);  // ü�� ���� 0 ���Ϸ� �������� �ʵ��� ����

        CheckHp();  // ü�� �� ����

        if(onRageArt == false)
        {
            onRageArt = Ragearts(); // Rage Arts ���� Ȯ��
            useRageArt = false;
        }
        
        Down();     // ü�� 0�� �� Down �Լ� ����
    }
    
    public bool checkRage()
    {
        if( onRageArt && !useRageArt)
            return true;
        else
            return false;
    }
    // Rage Arts ���¸� Ȯ���ϴ� �Լ� (ü���� ���� ������ �� ���� ����)
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


    // Start �Լ�: �ʱ� ����
    void Start()
    {
        gameOverUI.SetActive(false);

        if (HpBarSlider != null)
        {
            SetHp(maxHp);  // �ִ� ü���� �����ϰ� �����̴� ���� �ʱ�ȭ
        }

        onRageArt = false;
        useRageArt = false;
    }

    // Update �Լ�: �� �����Ӹ��� ȣ��
    void Update()
    {
        // �׽�Ʈ��: Space Ű�� ������ 10�� �������� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Damage(10);
        }
    }

    // ü���� 0�� �� �ִϸ��̼� Ʈ���� ����
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
