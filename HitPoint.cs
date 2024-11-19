using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    public Damage myPlayer;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (myPlayer.isLeftPlayer == true &&
            other.gameObject.layer == LayerMask.NameToLayer("PlayerRight"))
        {
            Debug.Log("���ʾְ� ������ �� ����");
            other.GetComponent<Hpbar>().Damage(myPlayer.lastAttackDamage);
        }
        else if (myPlayer.isLeftPlayer == false &&
            other.gameObject.layer == LayerMask.NameToLayer("PlayerLeft"))
        {
            Debug.Log("�����ְ� ���� �� ����");
            other.GetComponent<Hpbar>().Damage(myPlayer.lastAttackDamage);
        }
    }
}
