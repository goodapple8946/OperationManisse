using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class HPBar : MonoBehaviour
{
    // 前血条，受到伤害时立刻减少的血条
    private Transform frontTf;
    // 后血条，受到伤害时缓慢减少的血条
    private Transform backTf;

    // 当前值（前血条）
    public float value;
    // 当前值（后血条）
    public float valueBack = 0;
    // 最大值
    public float valueMax;
    // 后血条减少速度
    private float valueSpeed = 1f;
    // 血条相对单位的偏移
    private Vector2 offset = new Vector2(0, 0.32f);

    // Prefab
    public Sprite frontNeutral;
    public Sprite frontPlayer;
    public Sprite frontEnemy;
    public Sprite back;

    // 依附的单位
    [HideInInspector] public Unit unit;

    // 上次记录的所属玩家
    private Player player = Player.Neutral;

    private void Awake()
    {
        frontTf = transform.Find("HP Bar Front");
        backTf = transform.Find("HP Bar Back");
    }

    void Start()
    {
        valueMax = unit.healthMax;
        backTf.GetComponent<SpriteRenderer>().sprite = back;
    }

    void Update()
    {
        // 如果依附的单位没了，摧毁自己
        if (unit == null)
        {
            Destroy(gameObject);
            return;
        }

        // 如果单位由于某种原因，所属玩家发生改变，需要改变前血条颜色（Prefab）
        if (unit.player != player)
        {
            player = unit.player;
            frontTf.GetComponent<SpriteRenderer>().sprite = GetFrontSpriteByPlayer(player);
        }

        // 显示血条的条件：首先不能是鼠标上的物体，然后满足：单位血量不是满的 或 Editor强制显血选项开启了
        bool showHP =
            unit != editorController.MouseObject as Unit && 
            (unit.health != unit.healthMax || editorController.IsShowingHP);

        // 如果不满足显血条件
        if (!showHP)
        {
            // 隐藏血条
            transform.localScale = Vector2.zero;
            return;
        }

        // 显示血条
        transform.localScale = Vector2.one;

        // 设置血条当前值为单位生命值
        value = unit.health;

        // 后血条的值缓慢减少至前血条的值，但是不能低于前血条的值
        if (valueBack < value)
        {
            valueBack = value;
        }
        else if (valueBack > value)
        {
            valueBack -= valueSpeed * valueMax * Time.deltaTime;
        }

        // 血条位置更新
        transform.position = (Vector2)unit.transform.position + offset;

        // 前血条
        float frontScale = value / valueMax;
        frontTf.localScale = new Vector3(frontScale, 1, 1);
        frontTf.position = GetPositionByScale(frontScale);

        // 后血条
        float backScale = valueBack / valueMax;
        backTf.localScale = new Vector3(backScale, 1, 1);
        backTf.position = GetPositionByScale(backScale);
    }

    // 根据玩家获取前血条对应Prefab（仅颜色不同）
    private Sprite GetFrontSpriteByPlayer(Player player)
    {
        switch (unit.player)
        {
            case Player.Neutral:
                return frontNeutral;
            case Player.Player:
                return frontPlayer;
            case Player.Enemy:
                return frontEnemy;
            default:
                return frontNeutral;
        }
    }

    // 根据血条宽度比例（剩余血量百分比），计算血条应当所处的位置
    private Vector2 GetPositionByScale(float scale)
    {
        // 根据血条宽度比例，计算出的X偏移量
        float xDelta = ((scale - 1) / 2) * 0.64f;

        return (Vector2)transform.position + new Vector2(xDelta, 0);
    }
}