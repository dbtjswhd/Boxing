using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpbar : MonoBehaviour
{
    public Slider HpBarSlider;  // ����� Slider
    public float curHp;         // ���� ü��
    public float maxHp;         // �ִ� ü��
    public Animator animator;

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
        Down();     // ��� ���� Ȯ�� �� ó��
    }

    // ü���� 0 ������ �� ��� �ִϸ��̼��� ����ϴ� �Լ�
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
            SetHp(maxHp);  // �ִ� ü���� �����ϰ� �����̴� ���� �ʱ�ȭ
        }
    }

    void Update()
    {
        
        Ragearts();  // Rage Arts ���¸� �� ������ Ȯ��
    }

    // Rage Arts ���¸� Ȯ���ϴ� �Լ� (ü���� ���� ������ �� ���� ����)
    public void Ragearts()
    {
        if (curHp < 40 && HpBarSlider != null)
        {
            HpBarSlider.fillRect.GetComponent<Image>().color = Color.blue;  // �����̴� ���� ����
        }
    }
}
