using System.Collections.Generic;
using UnityEngine;
using CommandSystem;


[System.Serializable]
public class ComboCommand
{
    public string comboName;
    public Command[] commands;

    public int damage = 10;
    public float firstDelay = 0.5f;
    public float lastDelay = 0.5f;
    public string comboTrigger;
    public bool isFirstCombo = false;
    public bool isRageArt = false;
    
}


public class Combo : MonoBehaviour
{
    public ComboCommand[] comboList;
    public Animator animator;
  
    public Hpbar hpbar;

    public bool ComboCheck(Queue<MonoCommand> checkCommand, out ComboCommand findCombo )
    {
        foreach (var combo in comboList)
        {
            // 버퍼가 콤보 시퀀스보다 짧으면 콤보 불성립
            if (checkCommand.Count >= combo.commands.Length)
            {
                bool match = true;
                var inputArray = checkCommand.ToArray();

                int pushLength = inputArray.Length - combo.commands.Length;
                
                for (int i = 0; i < combo.commands.Length; i++)
                {
                    int checkIndex = i + pushLength;

                    // �Էµ� ���ɾ�� �޺� ���ɾ ��ġ�ϴ��� ��
                    if (inputArray[checkIndex].key != combo.commands[i])
                    {
                        match = false;
                        break;
                    }
                }

                // ��� ���ɾ ��ġ�ϸ� �޺� ����
                if (match)
                {
                    if(!combo.isRageArt ||  hpbar.checkRage())
                    {
                        findCombo = combo;
                        animator.SetTrigger(combo.comboTrigger);

                        hpbar.useRageArt = true;

                        return true;
                    }

                }

            }

        }
        
        findCombo = null;
        return false;
    }
}


