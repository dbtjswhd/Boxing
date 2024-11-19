using System.Collections.Generic;
using UnityEngine;

namespace CommandSystem
{
    public class CommandManager : MonoBehaviour
    {
        //���� �Է� Ű �⺻ ����
        private KeyCode[] dirKey =
        {
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.RightArrow,
            KeyCode.LeftArrow
        };
        //�׼� �Է� Ű �⺻ ����
        private KeyCode[] actionKey =
        {
            KeyCode.E,
            KeyCode.R,
            KeyCode.D,
            KeyCode.F,
            KeyCode.T
        };

        private MonoCommand[] monoCommandList =
            {
                new MonoCommand("�߸�",Command.Neutral,"[*]"),

                new MonoCommand("��",Command.Up,"[��]"),
                new MonoCommand("��",Command.Down,"[��]"),
                new MonoCommand("��",Command.Front,"[��]"),
                new MonoCommand("��",Command.Back,"[��]"),

                new MonoCommand("����",Command.UpFront,"[��]"),
                new MonoCommand("����",Command.UpBack,"[��]"),
                new MonoCommand("����",Command.DownFront,"[��]"),
                new MonoCommand("����",Command.DownBack,"[��]"),

                new MonoCommand("�޼�",Command.LPunch,"[LP]"),
                new MonoCommand("������",Command.RPunch,"[RP]"),
                new MonoCommand("�޹�",Command.LKick,"[LK]"),
                new MonoCommand("������",Command.RKick,"[RK]"),

                new MonoCommand("���",Command.APunch,"[AP]"),
                new MonoCommand("���",Command.AKick,"[AK]"),

                new MonoCommand("Ư��Ű",Command.Special,"[S]")
            };
        private Dictionary<Command, MonoCommand> commandDict = new Dictionary<Command, MonoCommand>(); // Ŀ�ǵ� ��ųʸ�

        [Header("Detail Option")]
        public bool useNeutral = false;
        public bool collIsNeutral = false;
        public bool useDiagoalCol = false;
        [SerializeField]
        private float delayTime = 0.02f;
        private float delayCheck_dir = 0;
        private bool onDelayed_dir = false;
        private float delayCheck_action = 0;
        private bool onDelayed_action = false;

        [Header("Vertual Stick")]
        public Transform Stick;
        public float stickDist = 120f;

        [Header("Input Debug")]
        [SerializeField] public Vector2 dirInput = new Vector2(0, 0);
        [SerializeField] private Vector2 dirInputNow = new Vector2(0, 0);
        [SerializeField] private float inputSpeed = 30f;
        private Command lastInputStick;
        [Space(10)]
        [SerializeField] private bool input_LP;
        [SerializeField] private bool input_RP;
        [SerializeField] private bool input_LK;
        [SerializeField] private bool input_RK;
        [Space(10)]
        [SerializeField] private bool input_S;


        [Header("Output Debug")]
        public bool useDebugLog;
        [Space(10)]
        [SerializeField] private Command lastDirCommand;
        [SerializeField] private Command delayedDirCommand;
        [SerializeField] private string debug_InputKey;
        [Space(10)]
        [SerializeField] private Command lastActionCommand;
        [SerializeField] private Command delayedActionCommand;
        private bool onDualInput = false;
        [Space(10)]
        [SerializeField] private string debug_OutputKey;

        private Queue<MonoCommand> CommandBuffer = new Queue<MonoCommand>();

        [SerializeField] private PlayerMove playerMove;
        [SerializeField] private Combo getCombo;

        // �޺� ���� �ð� ����� ���� ����
        
        private float comboBufferTime = 0.5f;
        // Start is called before the first frame update
        void Start()
        {
            // Ŀ�ǵ� ����Ʈ�� ��ųʸ��� ��ȯ
            foreach (MonoCommand command in monoCommandList)
            {
                if (!commandDict.ContainsKey(command.key))
                {
                    commandDict.Add(command.key, command);
                }
            }

            playerMove.CommandManager = this;
        }

        // Update is called once per frame
        void Update()
        {
            //����Ű �Է� ó��
            CheckInputStick(GetInputDir_KeyBoard());

            //�׼�Ű �Է� ó��
            GetInputAction_KeyBoard();


            if (useDebugLog && Input.GetKeyDown(KeyCode.Space))
            {
                ClearCommand();
            }

            comboBufferTime -= Time.deltaTime;
            if(comboBufferTime <= 0){
                //ClearCommand();
                CommandBuffer.Clear();
                debug_OutputKey = "";
            }
        }

        void GetInputAction_KeyBoard()
        {
            bool checkKey = false;

            int i = 0;
            foreach (var inputkey in actionKey)
            {
                //�׼� ���� �Է¿� ��ȭ Ȯ��
                if (Input.GetKeyDown(inputkey) || Input.GetKeyUp(inputkey))
                {
                    if (i == 0)
                        input_LP = Input.GetKey(inputkey);
                    else if (i == 1)
                        input_RP = Input.GetKey(inputkey);
                    else if (i == 2)
                        input_LK = Input.GetKey(inputkey);
                    else if (i == 3)
                        input_RK = Input.GetKey(inputkey);
                    else if (i == 4)
                        input_S = Input.GetKey(inputkey);

                    checkKey = true;

                }
                i++;
            }


            if (checkKey)
            {
                Command newInput = GetInputActionCommand();

                if (onDelayed_action)
                {
                    if (CheckDualAction(newInput, out Command dualCmd))
                    {
                        Debug.Log("���");

                        delayedActionCommand = dualCmd;

                        UpdateAcitonCommand();
                    }
                    else
                    {
                         Debug.Log("�����");
                        delayedActionCommand = newInput;

                    }
  
                }

               
                 if (newInput != delayedActionCommand)
                 {
                    delayedActionCommand = newInput;
                    delayCheck_action = Time.time;
                    onDelayed_action = true;
                 }

            }
               

            if (onDelayed_action && Time.time - delayCheck_action > delayTime + 0.02f)
            {
                if (onDualInput)
                {
                    delayedActionCommand = Command.Neutral;
                    onDualInput = false;
                }
                   
                UpdateAcitonCommand();
            }

        }
        Command GetInputActionCommand()
        {
            Command getCmd = Command.Neutral;

            if (input_LP)
            {
                if (input_RP)
                    getCmd = Command.APunch;
                else
                    getCmd = Command.LPunch;
            }
            else if (input_RP)
            {
                if (input_LP)
                    getCmd = Command.APunch;
                else
                    getCmd = Command.RPunch;
            }
            else if(input_LK)
            {
                if (input_RK)
                    getCmd = Command.AKick;
                else
                    getCmd = Command.LKick;
            }
            else if (input_RK)
            {
                if (input_LK)
                    getCmd = Command.AKick;
                else
                    getCmd = Command.RKick;
            }
            else if (input_S)
            {
               getCmd = Command.Special;
            }
            
            return getCmd;
        }
        void UpdateAcitonCommand()
        {
            lastActionCommand = delayedActionCommand;

            if ( delayedActionCommand != Command.Neutral)
            {
                CommandBuffer.Enqueue(commandDict[delayedActionCommand]);
                CheckCombo(); //�޺� Ȯ��   
            }
                

            delayedActionCommand = Command.Neutral;
            onDelayed_action = false;
            
            if (useDebugLog)
            {
                Debug_Queue();
            }
        }

        void Debug_Queue()
        {
            debug_OutputKey = "";

            MonoCommand[] CommandList = CommandBuffer.ToArray();

            foreach (var item in CommandList)
            {
                debug_OutputKey += item.code;
            }
        }
        //GetInputDir_KeyBoard : PC�� ���� ó�� �Լ�
        //�����¿� �����Է� �޾� �������̽�ƽ���� ó��
        //�߸��� 8���� �� ��ȭ�� �����Ǹ�
        bool GetInputDir_KeyBoard()
        {
            bool hasInput = false;
            foreach (var inputkey in dirKey)
            {
                //�̵� ���� �Է¿� ��ȭ Ȯ��
                if (Input.GetKeyDown(inputkey) || Input.GetKeyUp(inputkey))
                {
                    InputDirCommand();
                }
            }
            
            //�Է��� Lerp�Ͽ� �������̽�ƽ ������ ó��
            //�ε巴�� �Է��� ó���Ͽ� Ű���� �Է� ���� ������ ������ 
            dirInputNow = Vector2.Lerp(dirInputNow, dirInput, Time.deltaTime * inputSpeed);
            if (Vector2.Distance(dirInputNow, dirInput) < 0.2f) dirInputNow = dirInput;

            if (useDebugLog && Stick != null)
            {
                Stick.localPosition = dirInputNow * stickDist;
            }

            //Ŀ�ǵ� ó���� ���� ��ƽ�� �Էº�ȭ Ȯ���ϴ� �κ�
            //�Է��� ������ �Է°� ���Ͽ� ���ο� �Է��� ������ True��ȯ
            Command newInput = AngleToDir(dirInputNow);

            if (lastInputStick != newInput)
            {
                lastInputStick = newInput;
                hasInput = true;
            }

            return hasInput;
        }
        //����Ű �Է��� ó���ϴ� �κ�
        //���� �Է��� ������ ��� �� ��� ���� �Ŀ� Ŀ�ǵ� Ȯ����
        //���� �߿� �� �Է��� �߰ߵǸ�,
        //--- �밢������ �ɼǿ� ���� ���� �Է��� Ȯ���ϰ� ���� �Է� ��������
        void CheckInputStick( bool inputChecked)
        {
            if (inputChecked)
            {
                Command newInput = lastInputStick;

                //���� �߿� ���ο� �Է��� ���� ��� 
                if (onDelayed_dir)
                {
                    //�밢�� ���� ���� ���,
                    //--- ������ �ɼ� ��� ���Է��� �ٷ� ó����
                    if (useDiagoalCol && CheckDiagoalCol(newInput))
                    {
                        if(useDebugLog)
                            debug_InputKey += commandDict[delayedDirCommand].code;
                        delayedDirCommand = newInput; //�� �Է����� ���� �Է��� ��ü��   
                    }

                    //������ �Է� Ȯ��
                    UpdateDirCommand();
                }
                    
                //���ο� �Է¿� ���� ���� ����
                if (newInput != delayedDirCommand)
                {
                    delayedDirCommand = newInput;
                    delayCheck_dir = Time.time;

                    onDelayed_dir = true;
                }
            }
            //���� �Է��� ���� ��� ��� ��� ��, �Է� Ȯ��
            if(onDelayed_dir && Time.time - delayCheck_dir > delayTime)
            {
                UpdateDirCommand();
            }
        }
        //������ ���� �Է��� Ȯ���ϴ� �Լ�
        void UpdateDirCommand()
        {
            lastDirCommand = delayedDirCommand;
            //�߸� Ŀ�ǵ� ��� ���� �ɼǿ� ����,
            //�߸� Ŀ�ǵ��� Ŀ��Ʈ �Է� ó�� ����
            if(!useNeutral || lastDirCommand != Command.Neutral)
            {
                CommandBuffer.Enqueue(commandDict[lastDirCommand]);
                CheckCombo(); //�޺� Ȯ��
            }
                

            if (useDebugLog)
            {
                Debug_Queue();
            }

            onDelayed_dir = false;
        }
        //Ŀ�ǵ� ������ �ʱ�ȭ�ϴ� �Լ�
        public void ClearCommand()
        {
            CommandBuffer.Clear();
            onDelayed_dir = false;
            onDelayed_action = false;

            debug_InputKey = "";
            debug_OutputKey = "";

            Debug_Queue();
        }
        //4���� PC ����Ű �Է��� ó���ϴ� �Լ� 
        //collIsNeutral �ɼǿ� ���� �����Ǵ� ���� ó�� ����� �޶���
        void InputDirCommand()
        {
            Vector2 inputDir = Vector2.zero;

            //���� �Է� �߰�
            if (Input.GetKeyDown(dirKey[0]))
            {
                //�ݴ��� �Է� ���̶��(�浹)
                if (Input.GetKey(dirKey[1]))
                    inputDir.y = collIsNeutral ? 0 : -1;
                else
                    inputDir.y = 1;

            }
            //���� �Է� ����
            else if ( Input.GetKeyUp(dirKey[0]))
            {
                //�ݴ��� �Է� ���̶��
                if (Input.GetKey(dirKey[1]))
                    inputDir.y = -1;
                else
                    inputDir.y = 0;

            }

            //�Ʒ��� �Է� �߰�
            if (Input.GetKeyDown(dirKey[1]))
            {
                //�ݴ��� �Է� ���̶��(�浹)
                if (Input.GetKey(dirKey[0]))
                    inputDir.y = collIsNeutral ? 0 : 1;
                else
                    inputDir.y = -1;

            }
            //�Ʒ��� �Է� ����
            else if (Input.GetKeyUp(dirKey[1]))
            {
                //�ݴ��� �Է� ���̶��
                if (Input.GetKey(dirKey[0]))
                    inputDir.y = 1;
                else
                    inputDir.y = 0;

            }

            //������ �Է� �߰�
            if (Input.GetKeyDown(dirKey[2]))
            {
                //�ݴ��� �Է� ���̶��(�浹)
                if (Input.GetKey(dirKey[3]))
                    inputDir.x = collIsNeutral ? 0 : 1;
                else
                    inputDir.x = 1;

            }
            //������ �Է� ����
            else if (Input.GetKeyUp(dirKey[2]))
            {
                //�ݴ��� �Է� ���̶��
                if (Input.GetKey(dirKey[3]))
                    inputDir.x = -1;
                else
                    inputDir.x = 0;

            }
            //���� �Է� �߰�
            if (Input.GetKeyDown(dirKey[3]))
            {
                //�ݴ��� �Է� ���̶��(�浹)
                if (Input.GetKey(dirKey[2]))
                    inputDir.x = collIsNeutral ? 0 : -1;
                else
                    inputDir.x = -1;

            }
            //���� �Է� ����
            else if (Input.GetKeyUp(dirKey[3]))
            {
                //�ݴ��� �Է� ���̶��
                if (Input.GetKey(dirKey[2]))
                    inputDir.x = 1;
                else
                    inputDir.x = 0;

            }

        
            dirInput.Normalize();

            //��ġ Ȯ���ؼ� ������ �ֱ�
            if(!playerMove.isLeftPlayer){
                inputDir *= -1;
            }
            dirInput = inputDir;
        }


        bool CheckDualAction(Command newInput, out Command dualCommand)
        {
            bool allowDual = false;

            dualCommand = Command.Neutral;

            if (newInput == Command.APunch)
            {
                if (delayedActionCommand == Command.RPunch
                    || delayedActionCommand == Command.LPunch)
                {
                    dualCommand = Command.APunch;

                    allowDual = true;
                }
            }
            else if (newInput == Command.AKick)
            {
                if (delayedActionCommand == Command.LKick
                    || delayedActionCommand == Command.RKick)
                {
                    dualCommand = Command.AKick;

                    allowDual = true;
                }
            }
            //
            else if (newInput == Command.Neutral)
            {
                if (delayedActionCommand == Command.LPunch
                    || delayedActionCommand == Command.RPunch)
                {
                    if (lastActionCommand == Command.APunch)
                    {
                        dualCommand = Command.Neutral;

                        allowDual = true;
                    }
                }
                else if (delayedActionCommand == Command.LKick
                    || delayedActionCommand == Command.RKick)
                {
                    if (lastActionCommand == Command.AKick)
                    {
                        dualCommand = Command.Neutral;

                        allowDual = true;
                    }
                }
            }

            onDualInput = allowDual;

            return allowDual;
        }
        //�밢�� ���� ������ Ȯ���ϴ� �Լ�
        //���� �밢�� �Է��� ��Ȯ������.
        bool CheckDiagoalCol(Command newInput)
        {
            //�⺻������ ���� ���� ��ȯ
            bool overlap = false;

            //�밢�� �Է��� ���� ��� - �ֱ� �Է°� ���Ͽ� ��ø
            if (newInput == Command.DownBack)
            {
                if (delayedDirCommand == Command.Down
                    || delayedDirCommand == Command.Back)
                    overlap = true;
            }
            else if (newInput == Command.DownFront)
            {
                if (delayedDirCommand == Command.Down
                    || delayedDirCommand == Command.Front)
                    overlap = true;
            }
            else if (newInput == Command.UpBack)
            {
                if (delayedDirCommand == Command.Up
                    || delayedDirCommand == Command.Back)
                    overlap = true;
            }
            else if (newInput == Command.UpFront)
            {
                if (delayedDirCommand == Command.Up
                    || delayedDirCommand == Command.Front)
                    overlap = true;
            }

            //���� ��ġ �Է��� ���� ��� - �ֱ� �Է°� ���Ͽ� ��ø
            else if (newInput == Command.Neutral)
            {
                if (delayedDirCommand == Command.Up)
                {
                    if (lastDirCommand == Command.UpBack
                        || lastDirCommand == Command.UpFront)
                        overlap = true;
                }
                else if (delayedDirCommand == Command.Down)
                {
                    if (lastDirCommand == Command.DownBack
                        || lastDirCommand == Command.DownFront)
                        overlap = true;
                }
                else if (delayedDirCommand == Command.Front)
                {
                    if (lastDirCommand == Command.DownFront
                        || lastDirCommand == Command.UpFront)
                        overlap = true;
                }
                else if (delayedDirCommand == Command.Back)
                {
                    if (lastDirCommand == Command.UpBack
                        || lastDirCommand == Command.DownBack)
                        overlap = true;
                }
            }

            
            
            return overlap;
        }

        //���� ���� Ȯ���� ����Ȯ���� �ϴ� �Լ�
        Command AngleToDir(Vector2 input)
        {
            if (input.magnitude < 0.3f)
            {
                return Command.Neutral;
            }

            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

            // ������ ���� ���� ����
            if (angle >= -22.5f && angle < 22.5f)
            {
                return Command.Front;
            }
            else if (angle >= 22.5f && angle < 67.5f)
            {
                return Command.UpFront;
            }
            else if (angle >= 67.5f && angle < 112.5f)
            {
                return Command.Up;
            }
            else if (angle >= 112.5f && angle < 157.5f)
            {
                return Command.UpBack;
            }
            else if (angle >= -67.5f && angle < -22.5f)
            {
                return Command.DownFront;
            }
            else if (angle >= -112.5f && angle < -67.5f)
            {
                return Command.Down;
            }
            else if (angle >= -157.5f && angle < -112.5f)
            {
                return Command.DownBack;
            }
            else
            {
                return Command.Back;
            }

        }  
        public bool CheckCombo()
        {
            comboBufferTime = 0.5f;

            if(getCombo.ComboCheck(CommandBuffer, out ComboCommand comboCmd))
            {
                Debug.Log(comboCmd.comboName);
                

                return true;
            }
            return false;
        }
    }

    //���� ������ ���� enum
    public enum Command
    {
        Neutral,
        Up,
        Down,
        Back,
        Front,
        UpBack,
        UpFront,
        DownBack,
        DownFront,

        LPunch,
        RPunch,
        LKick,
        RKick,

        APunch,
        AKick,

        Special
    }

    //�Է� ���� ������ ���� Ŀ�ǵ�
    [System.Serializable]
    public class MonoCommand
    {
        public string keyName;
        public Command key;
        public string code;

        public MonoCommand(string keyName, Command key, string code)
        { 
            this.keyName = keyName;
            this.key = key;
            this.code = code;
        }
    }
}