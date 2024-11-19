using System.Collections.Generic;
using UnityEngine;

namespace CommandSystem
{
    public class CommandManager : MonoBehaviour
    {
        //방향 입력 키 기본 설정
        private KeyCode[] dirKey =
        {
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.RightArrow,
            KeyCode.LeftArrow
        };
        //액션 입력 키 기본 설정
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
                new MonoCommand("중립",Command.Neutral,"[*]"),

                new MonoCommand("상",Command.Up,"[↑]"),
                new MonoCommand("하",Command.Down,"[↓]"),
                new MonoCommand("전",Command.Front,"[→]"),
                new MonoCommand("후",Command.Back,"[←]"),

                new MonoCommand("상전",Command.UpFront,"[↗]"),
                new MonoCommand("상후",Command.UpBack,"[↖]"),
                new MonoCommand("하전",Command.DownFront,"[↘]"),
                new MonoCommand("하후",Command.DownBack,"[↙]"),

                new MonoCommand("왼손",Command.LPunch,"[LP]"),
                new MonoCommand("오른손",Command.RPunch,"[RP]"),
                new MonoCommand("왼발",Command.LKick,"[LK]"),
                new MonoCommand("오른발",Command.RKick,"[RK]"),

                new MonoCommand("양손",Command.APunch,"[AP]"),
                new MonoCommand("양발",Command.AKick,"[AK]"),

                new MonoCommand("특수키",Command.Special,"[S]")
            };
        private Dictionary<Command, MonoCommand> commandDict = new Dictionary<Command, MonoCommand>(); // 커맨드 딕셔너리

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

        // 콤보 버퍼 시간 계산을 위한 변수
        
        private float comboBufferTime = 0.5f;
        // Start is called before the first frame update
        void Start()
        {
            // 커맨드 리스트를 딕셔너리로 변환
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
            //방향키 입력 처리
            CheckInputStick(GetInputDir_KeyBoard());

            //액션키 입력 처리
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
                //액션 관련 입력에 변화 확인
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
                        Debug.Log("듀얼");

                        delayedActionCommand = dualCmd;

                        UpdateAcitonCommand();
                    }
                    else
                    {
                         Debug.Log("덮어쓰기");
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
                CheckCombo(); //콤보 확인   
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
        //GetInputDir_KeyBoard : PC용 방향 처리 함수
        //상하좌우 방향입력 받아 가상초이스틱으로 처리
        //중립과 8방향 중 변화가 감지되면
        bool GetInputDir_KeyBoard()
        {
            bool hasInput = false;
            foreach (var inputkey in dirKey)
            {
                //이동 관련 입력에 변화 확인
                if (Input.GetKeyDown(inputkey) || Input.GetKeyUp(inputkey))
                {
                    InputDirCommand();
                }
            }
            
            //입력을 Lerp하여 가상조이스틱 움직임 처리
            //부드럽게 입력을 처리하여 키보드 입력 갯수 제한을 보정함 
            dirInputNow = Vector2.Lerp(dirInputNow, dirInput, Time.deltaTime * inputSpeed);
            if (Vector2.Distance(dirInputNow, dirInput) < 0.2f) dirInputNow = dirInput;

            if (useDebugLog && Stick != null)
            {
                Stick.localPosition = dirInputNow * stickDist;
            }

            //커맨드 처리를 위해 스틱의 입력변화 확인하는 부분
            //입력을 마지막 입력과 비교하여 새로운 입력이 나오면 True반환
            Command newInput = AngleToDir(dirInputNow);

            if (lastInputStick != newInput)
            {
                lastInputStick = newInput;
                hasInput = true;
            }

            return hasInput;
        }
        //방향키 입력을 처리하는 부분
        //방향 입력이 들어오면 기억 후 잠시 지연 후에 커맨드 확정됨
        //지연 중에 새 입력이 발견되면,
        //--- 대각선보정 옵션에 따라서 지연 입력을 확정하고 다음 입력 지연시작
        void CheckInputStick( bool inputChecked)
        {
            if (inputChecked)
            {
                Command newInput = lastInputStick;

                //지연 중에 새로운 입력이 들어온 경우 
                if (onDelayed_dir)
                {
                    //대각선 보정 중인 경우,
                    //--- 지연된 옵션 대신 새입력을 바로 처리함
                    if (useDiagoalCol && CheckDiagoalCol(newInput))
                    {
                        if(useDebugLog)
                            debug_InputKey += commandDict[delayedDirCommand].code;
                        delayedDirCommand = newInput; //새 입력으로 지연 입력을 대체함   
                    }

                    //지연된 입력 확정
                    UpdateDirCommand();
                }
                    
                //새로운 입력에 대한 지연 시작
                if (newInput != delayedDirCommand)
                {
                    delayedDirCommand = newInput;
                    delayCheck_dir = Time.time;

                    onDelayed_dir = true;
                }
            }
            //지연 입력이 있을 경우 잠시 대기 후, 입력 확정
            if(onDelayed_dir && Time.time - delayCheck_dir > delayTime)
            {
                UpdateDirCommand();
            }
        }
        //지연된 방향 입력을 확정하는 함수
        void UpdateDirCommand()
        {
            lastDirCommand = delayedDirCommand;
            //중립 커맨드 사용 여부 옵션에 따라,
            //중립 커맨드의 커맨트 입력 처리 결정
            if(!useNeutral || lastDirCommand != Command.Neutral)
            {
                CommandBuffer.Enqueue(commandDict[lastDirCommand]);
                CheckCombo(); //콤보 확인
            }
                

            if (useDebugLog)
            {
                Debug_Queue();
            }

            onDelayed_dir = false;
        }
        //커맨드 누적을 초기화하는 함수
        public void ClearCommand()
        {
            CommandBuffer.Clear();
            onDelayed_dir = false;
            onDelayed_action = false;

            debug_InputKey = "";
            debug_OutputKey = "";

            Debug_Queue();
        }
        //4개의 PC 방향키 입력을 처리하는 함수 
        //collIsNeutral 옵션에 따라 대응되는 방향 처리 방식이 달라짐
        void InputDirCommand()
        {
            Vector2 inputDir = Vector2.zero;

            //위쪽 입력 추가
            if (Input.GetKeyDown(dirKey[0]))
            {
                //반대쪽 입력 중이라면(충돌)
                if (Input.GetKey(dirKey[1]))
                    inputDir.y = collIsNeutral ? 0 : -1;
                else
                    inputDir.y = 1;

            }
            //위쪽 입력 제거
            else if ( Input.GetKeyUp(dirKey[0]))
            {
                //반대쪽 입력 중이라면
                if (Input.GetKey(dirKey[1]))
                    inputDir.y = -1;
                else
                    inputDir.y = 0;

            }

            //아래쪽 입력 추가
            if (Input.GetKeyDown(dirKey[1]))
            {
                //반대쪽 입력 중이라면(충돌)
                if (Input.GetKey(dirKey[0]))
                    inputDir.y = collIsNeutral ? 0 : 1;
                else
                    inputDir.y = -1;

            }
            //아래쪽 입력 제거
            else if (Input.GetKeyUp(dirKey[1]))
            {
                //반대쪽 입력 중이라면
                if (Input.GetKey(dirKey[0]))
                    inputDir.y = 1;
                else
                    inputDir.y = 0;

            }

            //오른쪽 입력 추가
            if (Input.GetKeyDown(dirKey[2]))
            {
                //반대쪽 입력 중이라면(충돌)
                if (Input.GetKey(dirKey[3]))
                    inputDir.x = collIsNeutral ? 0 : 1;
                else
                    inputDir.x = 1;

            }
            //오른쪽 입력 제거
            else if (Input.GetKeyUp(dirKey[2]))
            {
                //반대쪽 입력 중이라면
                if (Input.GetKey(dirKey[3]))
                    inputDir.x = -1;
                else
                    inputDir.x = 0;

            }
            //왼쪽 입력 추가
            if (Input.GetKeyDown(dirKey[3]))
            {
                //반대쪽 입력 중이라면(충돌)
                if (Input.GetKey(dirKey[2]))
                    inputDir.x = collIsNeutral ? 0 : -1;
                else
                    inputDir.x = -1;

            }
            //왼쪽 입력 제거
            else if (Input.GetKeyUp(dirKey[3]))
            {
                //반대쪽 입력 중이라면
                if (Input.GetKey(dirKey[2]))
                    inputDir.x = 1;
                else
                    inputDir.x = 0;

            }

        
            dirInput.Normalize();

            //위치 확인해서 뒤집어 주기
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
        //대각선 보정 조건을 확인하는 함수
        //사용시 대각선 입력이 정확해진다.
        bool CheckDiagoalCol(Command newInput)
        {
            //기본적으로 받은 값을 반환
            bool overlap = false;

            //대각선 입력이 들어온 경우 - 최근 입력과 비교하여 중첩
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

            //십자 위치 입력이 들어온 경우 - 최근 입력과 비교하여 중첩
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

        //벡터 값을 확인해 방향확인을 하는 함수
        Command AngleToDir(Vector2 input)
        {
            if (input.magnitude < 0.3f)
            {
                return Command.Neutral;
            }

            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

            // 각도에 따라 방향 결정
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

    //방향 구분을 위한 enum
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

    //입력 단위 구분을 위한 커맨드
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