using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public bool isLeftPlayer = true;

    public Collider left;
    public Collider right;

    public int lastAttackDamage = 1;

    public float damageDelay;
    public float damageFlow;


    void Start()
    {
        // Collider�� ��Ȱ��ȭ�Ͽ� ����
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
    }
    private void Update()
    {
        //�׽�Ʈ������ �쿡 ���ؼ� ����
        if (Input.GetKeyDown(KeyCode.E))
            StartDamageCheck(5, 0.4f, 0.2f);
    }

    public void StartDamageCheck( int setDamage , float delay, float flow)
    {
        // ����� ��ų�� ���� �ʱ�ȭ
        lastAttackDamage = setDamage;
        damageDelay = delay;
        damageFlow = flow;

        StartCoroutine(CheckDamage());
    }

    IEnumerator CheckDamage()
    { 
        //����
        yield return new WaitForSeconds(damageDelay);
        //�ݶ��̴� Ȱ��ȭ
        left.gameObject.SetActive(true);
        right.gameObject.SetActive(true);
        yield return new WaitForSeconds(damageFlow);
        //�ݶ��̴� ��Ȱ��ȭ
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
    }
}
