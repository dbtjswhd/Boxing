// using System.Collections.Generic;
// using UnityEngine;


// public class CommandInput : MonoBehaviour
// {
//     private Queue<InputEntry> inputBuffer = new Queue<InputEntry>(); // �Է��� �����ϴ� ť
//     [SerializeField] private PlayerMove playerMove;
    
//     [SerializeField]
//     private bool debugModeOn = false;
   
//     [SerializeField] private string bufferContent;
//     [SerializeField] private string lastInput;

//     public float bufferTime = 0.5f; // �Է��� ��ȿ�� �ð� (0.5��)

//     [SerializeField] private MonoCommand[] commandList;
//     private Dictionary<CommandKey, MonoCommand> commandDictionary = new Dictionary<CommandKey, MonoCommand>(); // Ŀ�ǵ� ��ųʸ�

//     [SerializeField] private ComboCommand[] comboList;

//     [HideInInspector] public float moveInputX; //�¿��Է�
//     [HideInInspector] public float moveInputY; //�����Է�

    
//     private void Start()
//     {
//         playerMove.input = this;
        
//         // Ŀ�ǵ� ����Ʈ�� ��ųʸ��� ��ȯ
//         foreach (MonoCommand command in commandList)
//         {
//             if (!commandDictionary.ContainsKey(command.key))
//             {
//                 commandDictionary.Add(command.key, command);
//             }
//         }
//     }
//     void Update()
//     {
//         //�̵��� ���� �Է��� �߰��� ���� Ȯ��
//         CheckMoveInput();

//         // �Է��� �߻��ϸ� ���ۿ� �߰�
//         if (Input.anyKeyDown)
//         {
//             AddInputToBuffer();
//         }

//         // ������ �Է��� ���ۿ��� ����
//         //RemoveOldInputs();
//     }

//     void CheckMoveInput()
//     {
//         moveInputX = Input.GetAxis("Horizontal");
//         moveInputY = Input.GetAxis("Vertical");
//     }

//     void AddInputToBuffer()
//     {
//         foreach(MonoCommand entry in commandList)         
//         {
//             if (Input.GetKeyDown(entry.KeyCode))
//             {
//                 AddCommand(entry);

//                 //�׼����� �ƴϸ� �޺�Ȯ���Ͽ� ó��
//                 //�׼����� �ƴϴ��� �޺��� ���� �Է��� ������ �װ��� ���
//                 //if(!playerMove.onAction)  
//                 if(CheckForSpecialMove(out ComboCommand addCombo))
//                 {
//                     if(!playerMove.onAction || !addCombo.isFirstCombo) 
//                         AddComboToPlayer(addCombo);
//                 }
                    
//                 break;
//             }
//         }


//         // ��: ����Ű�� ����Ű �Է��� Ȯ��
//         /*
//         if (Input.GetKeyDown(KeyCode.DownArrow))
//         {
//             AddCommand("down");
//         }
//         else if (Input.GetKeyDown(KeyCode.RightArrow))
//         {
//             AddCommand("forward");
//         }
//         else if (Input.GetKeyDown(KeyCode.P))
//         {
//             AddCommand("punch");
//         }
//         */

        
//     }
//     void UpdateDebugInput()
//     {
//         bufferContent = string.Empty;

//         foreach (var entry in inputBuffer)
//         {
//             bufferContent += $"[{entry.input.spell}] ";
//         }
//     }
//     void AddCommand(MonoCommand command )
//     {
//         inputBuffer.Enqueue(new InputEntry(command, Time.time));

//         if(debugModeOn) UpdateDebugInput();
//     }
//     public void ClearInputBuffer()
//     {
//         inputBuffer.Clear();
//           if (debugModeOn) UpdateDebugInput();
//     }
//     void RemoveOldInputs()
//     {
//         // �Է� ���ۿ��� �ð��� ���� �Է��� ����
//         while (inputBuffer.Count > 0 && Time.time - inputBuffer.Peek().timestamp > bufferTime)
//         {
//             inputBuffer.Dequeue();

//             if (debugModeOn) UpdateDebugInput();
//         }
//     }

//     // Ư�� Ŀ�ǵ尡 �ԷµǾ����� Ȯ���ϴ� �Լ�
//     public bool CheckForSpecialMove(out ComboCommand findCombo)
//     {
//         foreach (var combo in comboList)
//         {
//             // �޺��� ���̿� �Է� ������ ���̰� ���� ������ ���� �ʿ� ����
//             if (inputBuffer.Count >= combo.commandList.Length)
//             {
//                 bool match = true;
//                 var inputArray = inputBuffer.ToArray();

//                 int pushLength = inputArray.Length - combo.commandList.Length;
                
//                 for (int i = 0; i < combo.commandList.Length; i++)
//                 {
//                     int checkIndex = i + pushLength;

//                     // �Էµ� ���ɾ�� �޺� ���ɾ ��ġ�ϴ��� ��
//                     if (inputArray[checkIndex].input.key != combo.commandList[i])
//                     {
//                         match = false;
//                         break;
//                     }
//                 }

//                 // ��� ���ɾ ��ġ�ϸ� �޺� ����
//                 if (match)
//                 {
//                     findCombo = combo;
//                     return true;
//                 }
//             }
//         }

//         findCombo = null;
//         return false;
//     }
//     void AddComboToPlayer( ComboCommand addCombo)
//     {
//         playerMove.AddCombo(addCombo);

//         if (debugModeOn)
//         {
//             lastInput = addCombo.comboName;
//             //Debug.Log(lastInput);
//         }
//     }
// }

// public enum CommandKey
// {
//     UpSide ,
//     DownSide,
//     Forward,
//     Backward,
//     Hook,
//     Straight,
//     Jab,
//     Upper,
//     Ra
// }


// [System.Serializable]
// public class InputEntry
// {
//     public MonoCommand input;
//     public float timestamp;

//     public InputEntry(MonoCommand input, float timestamp)
//     {
//         this.input = input;
//         this.timestamp = timestamp;
//     }
// }

