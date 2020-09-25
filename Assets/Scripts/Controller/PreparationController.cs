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

    // 连续购买（是购买并安放的，而非移动网格中现有的）
    private bool buyContinuous;

    // 放置位置背景颜色深度
    public float colorAlpha = 0.2f;

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
        ShowGrid();
    }

    void Update()
    {
        if (mouseUnit != null)
        {
            mouseUnit.transform.position = MouseController.MouseWorldPosition();

            // R键旋转
            if (Input.GetKeyDown(KeyCode.R))
            {
                Block block = mouseUnit as Block;

                if (block != null)
                {
                    block.Rotate();
                }
            }
        }
    }

    public void LeftClickUnit(Unit unit)
    {
        if (gameController.gamePhase == GameController.GamePhase.Preparation)
        {
            if (mouseUnit != null)
            {
                if (mouseUnit == unit)
                {
                    // 安放鼠标上的物体到网格中
                    CheckAbsorption(unit);
                }
                else
                {
                    // 将网格物体与鼠标物体交换
                }
            }
            else
            {
                // 移动网格中的物体
                buyContinuous = false;
                Pick(unit);
            }
        }
    }

    public void RightClickUnit(Unit unit)
    {
        if (gameController.gamePhase == GameController.GamePhase.Preparation)
        {
            Sell(unit);
        }
    }

    void ShowGrid()
    {
        // 显示网格
        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < yNum; y++)
            {
                GameObject squareObj = Instantiate(square);
                squareObj.transform.position = CoordToPosition(x, y);
                squareObj.transform.parent = transform;
                squareObj.GetComponent<SpriteRenderer>().sortingLayerName = "Area";

                if ((x + y) % 2 == 0)
                {
                    squareObj.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, colorAlpha / 2);
                }
                else
                {
                    squareObj.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, colorAlpha);
                }
            }
        }
    }

    // 购买
    public void Buy(GameObject prefab)
    {
        buyContinuous = true;

        Unit unit = prefab.GetComponent<Unit>();

        if (mouseUnit == null)
        {
            if (gameController.playerMoney >= unit.price)
            {
                gameController.playerMoney -= unit.price;

                Unit unitBought = Instantiate(prefab).GetComponent<Unit>();
                unitBought.name = prefab.name;

                Pick(unitBought);
            }
            else
            {
                gameController.MoneyNotEnough();
            }
        }
        else
        {
            if (gameController.playerMoney + mouseUnit.price >= unit.price)
            {
                gameController.playerMoney += mouseUnit.price - unit.price;
                Destroy(mouseUnit.gameObject);

                Unit unitBought = Instantiate(prefab).GetComponent<Unit>();
                unitBought.name = prefab.name;

                Pick(unitBought);
            }
            else
            {
                gameController.MoneyNotEnough();
            }
        }
    }

    // 出售
    public void Sell(Unit unit)
    {
        gameController.playerMoney += unit.price;
        Destroy(unit.gameObject);

        int rand = Random.Range(0, resourceController.audiosDelete.Length);
        AudioSource.PlayClipAtPoint(resourceController.audiosDelete[rand], unit.transform.position);
    }

    // 拾起
    public void Pick(Unit unit)
    {
        int x = unit.gridX;
        int y = unit.gridY;
        if (x != -1 && y != -1)
        {
            grid[x, y] = null;
            unit.gridX = unit.gridY = -1;
        }
        unit.SetSpriteLayer("Pick");
        mouseUnit = unit;
    }

    // 安放，吸附到最近的网格
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
                mouseUnit = null;

                // 如果是购买并安放的（而非移动网格中现有的），而且钱够，继续购买
                if (buyContinuous && gameController.playerMoney >= unit.price)
                {
                    Buy(unit.gameObject);
                }

                // 安放
                grid[x, y] = unit;
                unit.gridX = x;
                unit.gridY = y;
                unit.transform.position = CoordToPosition(x, y);
                unit.transform.parent = gameController.playerObjects.transform;
                unit.SetSpriteLayer("Unit");

                int rand = Random.Range(0, resourceController.audiosPut.Length);
                AudioSource.PlayClipAtPoint(resourceController.audiosPut[rand], unit.transform.position);
            }
        }
    }

    // 根据Unit的网格坐标安放
    public void PutAllUnits()
    {
        ArrayList unitObjects = new ArrayList();
        unitObjects.AddRange(GameObject.FindGameObjectsWithTag("Ball"));
        unitObjects.AddRange(GameObject.FindGameObjectsWithTag("Block"));
        foreach (GameObject unitObject in unitObjects)
        {
            Unit unit = unitObject.GetComponent<Unit>();
            if (unit.player == Unit.Player.Player && unit.gridX != -1 && unit.gridY != -1)
            {
                grid[unit.gridX, unit.gridY] = unit;
            }
        }
    }

    // 连接所有网格中的Block
    public void LinkAllBlocksInGrid()
    {
        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < yNum; y++)
            {
                Block block = grid[x, y] as Block;
                if (block != null)
                {
                    for (int direction = 0; direction < 4; direction++)
                    {
                        Block another = GetNeighborBlock(x, y, direction);
                        if (another != null && CanLink(block, another, direction))
                        {
                            Link(block, another, direction);
                        }
                    }
                }
            }
        }
    }

    // 连接所有Block
    public void LinkAllBlocks()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject gameObject in gameObjects)
        {
            Block block = gameObject.GetComponent<Block>();
            for (int direction = 0; direction < 4; direction++)
            {
                if (block.blocksLinked[direction] == null && block.IsLinkAvailable(direction))
                {
                    Block another = GetLinkableBlockByDirection(block, direction);

                    // 连接
                    if (another != null)
                    {
                        Link(block, another, direction);
                    }
                }
            }
        }
    }

    // 检测该方向范围内是否有可连接的Block
    Block GetLinkableBlockByDirection(Block block, int direction)
    {
        // 检测连接距离
        float distanceCheck = 0.3f;

        int directionNeg = GetDirectionNegative(direction);

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject gameObject in gameObjects)
        {
            Block another = gameObject.GetComponent<Block>();

            if (another != block &&
                // 所属同一名玩家
                block.player == another.player &&
                // Another的位置可以连接
                another.IsLinkAvailable(directionNeg) == true &&
                // Another的位置没有被占用
                another.blocksLinked[directionNeg] == null)
            {
                // Another的连接点
                Vector2 absorptionPoint = LinkPoint(another, directionNeg);

                // Block与连接点的距离
                float distance = ((Vector2)block.transform.position - absorptionPoint).magnitude;

                // 满足吸附距离
                if (distance <= distanceCheck)
                {
                    // 返回满足的Another
                    return another;
                }
            }
        }
        return null;
    }

    // 按照方向返回Block的连接点
    public Vector2 LinkPoint(Block block, int direction)
    {
        Vector2 point = block.transform.position;
        switch (direction)
        {
            case 0:
                point += Vector2.right * 2 * block.radius;
                break;
            case 1:
                point += Vector2.up * 2 * block.radius;
                break;
            case 2:
                point += Vector2.left * 2 * block.radius;
                break;
            case 3:
                point += Vector2.down * 2 * block.radius;
                break;
        }
        return point;
    }

    // 连接两Block
    void Link(Block block, Block another, int direction)
    {
        int directionNeg = GetDirectionNegative(direction);

        block.LinkTo(another, direction);
        another.LinkTo(block, directionNeg);
    }

    // 检测两Block是否能连接
    bool CanLink(Block block, Block another, int direction)
    {
        int directionNeg = GetDirectionNegative(direction);
        // Block的相应方向是可连接的
        bool linkAvailable = block.IsLinkAvailable(direction) && another.IsLinkAvailable(directionNeg);
        // Block的相应方向未连接
        bool unlinked = block.blocksLinked[direction] == null && another.blocksLinked[directionNeg] == null;
        return linkAvailable && unlinked;
    }

    // 根据方向获取相邻的Block
    Block GetNeighborBlock(int x, int y, int direction)
    {
        // 超出边界
        if (direction == 0 && x == xNum - 1 || direction == 1 && y == yNum - 1 || direction == 2 && x == 0 || direction == 3 && y == 0) 
        {
            return null;
        }
        else
        {
            int anotherX = x + dir[direction, 0];
            int anotherY = y + dir[direction, 1];
            return grid[anotherX, anotherY] as Block;
        }
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

    // 清除MouseUnit
    public void ClearMouseUnit()
    {
        if (mouseUnit != null)
        {
            gameController.playerMoney += mouseUnit.price;
            Destroy(mouseUnit.gameObject);
        }
    }
}
