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
        // Collider를 비활성화하여 시작
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
    }
    private void Update()
    {
        //테스트용으로 잽에 대해서 설정
        if (Input.GetKeyDown(KeyCode.E))
            StartDamageCheck(5, 0.4f, 0.2f);
    }

    public void StartDamageCheck( int setDamage , float delay, float flow)
    {
        // 사용할 스킬의 정보 초기화
        lastAttackDamage = setDamage;
        damageDelay = delay;
        damageFlow = flow;

        StartCoroutine(CheckDamage());
    }

    IEnumerator CheckDamage()
    { 
        //시작
        yield return new WaitForSeconds(damageDelay);
        //콜라이더 활성화
        left.gameObject.SetActive(true);
        right.gameObject.SetActive(true);
        yield return new WaitForSeconds(damageFlow);
        //콜라이더 비활성화
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
    }
}
