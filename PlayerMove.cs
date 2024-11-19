
using System.Collections;
using System.Collections.Generic;
using CommandSystem;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [HideInInspector] public CommandManager CommandManager;

    public bool isLeftPlayer = true;
    public float moveSpeed = 5f;

    //[HideInInspector]
    public bool onAction = false;
    public ComboCommand nowCombo;
    public List<ComboCommand> ComboList;

    [SerializeField] private float checkDelay;
    [SerializeField] private Transform opponent; // ��� �÷��̾� ���� �߰�

    [SerializeField] private string lastCombo;
    [SerializeField] private int lastAttackDamage;
    [SerializeField] private int lastComboDamage;

    void Start()
    {

    }

    void Update()
    {
        // ��� �÷��̾ �׻� �ٶ󺸵��� ����
        if (opponent != null)
        {
            transform.LookAt(new Vector3(opponent.position.x, transform.position.y, opponent.position.z));
        }

        if (!onAction)
        {
            if (CommandManager != null)
                Move(CommandManager.dirInput.x, CommandManager.dirInput.y);
        }
        else
        {

        }
    }

    void Move(float inputX, float inputY)
    {
        // �̵� �Է� ó��
        float moveX = inputX;
        if (!isLeftPlayer) moveX *= -1f;
        float moveY = inputY;

        Vector3 moveDirection = transform.right * moveY + transform.forward * moveX;
        float speed = moveSpeed;

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        animator.SetFloat("MoveY", moveX);
        animator.SetFloat("MoveX", moveY);
    }

    public void AddCombo(ComboCommand combo)
    {
        ComboList.Add(combo);

        if (!onAction)
        {
            StartCombo();
        }
    }

    void StartCombo()
    {
        nowCombo = ComboList[0];
        ComboList.RemoveAt(0);

        StartCoroutine(ActCombo());
    }

    IEnumerator ActCombo()
    {
        onAction = true;
        animator.SetTrigger(nowCombo.comboTrigger);
        yield return new WaitForSeconds(nowCombo.firstDelay);
        Debug.Log(nowCombo.comboName + "(" + nowCombo.damage + ")");
        yield return new WaitForSeconds(nowCombo.lastDelay);

        if (ComboList != null && ComboList.Count > 0)
        {
            StartCombo();
        }
        else
        {
            if (CommandManager != null)
                CommandManager.ClearCommand();
            onAction = false;
        }
    }
}
