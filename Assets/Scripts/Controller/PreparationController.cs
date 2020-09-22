using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationController : MonoBehaviour
{
    // 玩家放置物体的网格
    private Unit[,] grid;

    // 网格尺寸
    public int xNum;
    public int yNum;

    // 每个网格的大小
    private float gridSize = 0.6f;

    // 整个网格的左下角的坐标
    public Vector2 origin;

    // 根据方向获取坐标偏移
    private int[,] dir = { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };

    // Unit吸附的最大范围
    float distanceAbsorption = 0.6f;

    // 当前鼠标持有的Unit
    public Unit mouseUnit;

    public GameObject square;
    private GameController gameController;
    private ResourceController resourceController;

    void Awake()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        resourceController = GameObject.Find("Resource Controller").GetComponent<ResourceController>();

        grid = new Unit[xNum, yNum];
    }

    void Start()
    {
        // Debug: 每个网格的中心点
        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < yNum; y++)
            {
                GameObject squareObj = Instantiate(square);
                squareObj.transform.position = CoordToPosition(x, y);
                squareObj.transform.parent = transform;
            }
        }
    }

    void Update()
    {
        if (mouseUnit != null)
        {
            mouseUnit.transform.position = MouseController.MouseWorldPosition();
        }
    }

    public void LeftClickUnit(Unit unit)
    {
        if (gameController.gamePhase == GameController.GamePhase.Preparation)
        {
            CheckAbsorption(unit);
        }
    }

    public void RightClickUnit(Unit unit)
    {
        if (gameController.gamePhase == GameController.GamePhase.Preparation)
        {
            Sell(unit);
        }
    }

    // 购买
    void Buy(GameObject prefab)
    {
        Unit unit = prefab.GetComponent<Unit>();

        if (gameController.playerMoney >= unit.price)
        {
            gameController.playerMoney -= unit.price;
            mouseUnit = Instantiate(prefab).GetComponent<Unit>();
            mouseUnit.name = prefab.name;
            mouseUnit.transform.parent = gameController.playerObjects.transform;
        }
    }

    // 出售
    void Sell(Unit unit)
    {
        gameController.playerMoney += unit.price;
        Destroy(unit.gameObject);

        int rand = Random.Range(0, resourceController.audiosDelete.Length);
        AudioSource.PlayClipAtPoint(resourceController.audiosDelete[rand], unit.transform.position);
    }

    // 开始游戏
    public void StartGame()
    {
        // 清除鼠标上的Unit
        if (mouseUnit != null)
        {
            Destroy(mouseUnit);
        }
    }

    // 安置，吸附到最近的网格
    public void CheckAbsorption(Unit unit)
    {
        Vector2 pos = unit.transform.position;
        int[] coord = PositionToClosestCoord(pos);
        if (coord != null)
        {
            int x = coord[0];
            int y = coord[1];

            if (grid[x, y] == null)
            {
                // 如果钱够，继续购买
                if (gameController.playerMoney >= unit.price)
                {
                    Buy(unit.gameObject);
                }
                else
                {
                    mouseUnit = null;
                }

                grid[x, y] = unit;
                unit.gridX = x;
                unit.gridY = y;
                unit.transform.position = CoordToPosition(x, y);
            }
        }
    }

    // 连接所有Block
    public void LinkAllBlocks()
    {
        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < yNum; y++)
            {
                Block block = grid[x, y] as Block;
                if (block != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Block another = GetNeighborBlock(x, y, i);
                        if (another != null && CanLink(block, another, i))
                        {
                            Link(block, another, i);
                        }
                    }
                }
            }
        }
    }

    // 连接两Block
    void Link(Block block, Block another, int direction)
    {
        FixedJoint2D joint;
        int directionNeg = GetDirectionNegative(direction);

        // Block连接Another
        joint = block.gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = another.body;
        block.blocksLinked[direction] = another;
        block.joints[direction] = joint;

        // Another连接Block
        joint = another.gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = block.body;
        another.blocksLinked[directionNeg] = block;
        another.joints[directionNeg] = joint;
    }

    // 检测两Block是否能连接
    bool CanLink(Block block, Block another, int direction)
    {
        int directionNeg = GetDirectionNegative(direction);
        // Block的相应方向提供连接性
        bool linkAvailable = block.IsLinkAvailable(direction) && another.IsLinkAvailable(directionNeg);
        // Block的相应方向未连接
        bool unlinked = block.blocksLinked[direction] == null && another.blocksLinked[directionNeg] == null;
        return linkAvailable && unlinked;
    }

    // 根据方向获取相邻的Block
    Block GetNeighborBlock(int x, int y, int direction)
    {
        int anotherX = x + dir[direction, 0];
        int anotherY = y + dir[direction, 1];
        return grid[anotherX, anotherY] as Block;
    }

    // 离Position最近的网格的(x, y)
    int[] PositionToClosestCoord(Vector2 position)
    {
        int[] ret = null;
        float distanceClosest = distanceAbsorption;
        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < yNum; y++)
            {
                float distance = (CoordToPosition(x, y) - position).magnitude;
                if (distance < distanceClosest)
                {
                    distanceClosest = distance;
                    ret = new int[] { x, y };
                }
            }
        }
        return ret;
    }

    // 根据Grid[x][y]获取中心坐标
    Vector2 CoordToPosition(int x, int y)
    {
        Vector2 ret = new Vector2(gridSize * (x + 0.5f), gridSize * (y + 0.5f));
        return ret + origin;
    }

    protected int GetDirectionNegative(int direction)
    {
        return (direction + 2) % 4;
    }

    // 清除Grid[x][y]
    public void Clear(int x, int y)
    {
        grid[x, y] = null;
    }
}
