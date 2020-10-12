using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static UnityEngine.Object;
using static Controller;


public class Grid
{
	// 每个网格的大小
	public static readonly float GRID_SIZE = 0.6f;

	// 原点位置
	public Vector2 OriginPos { get; private set; }

	// 所有创造出的网格的父物体
	private GameObject gridObj;

	private Unit[,] unitArr;

	/// <summary>
	/// 创建新的网格
	/// 并向其中填充坐标在其内的units
	/// </summary>
	public Grid(int xCount, int yCount, Vector2 originPos, Unit[] units)
		: this(xCount, yCount, originPos)
	{ 
		foreach (Unit unit in units)
		{
			// 填充本应在网格中的
			if (InGrid(unit))
			{
				unitArr[unit.coord.x, unit.coord.y] = unit;
			}
			// TODO: GameCONtroller里面是否有删除？
			//else
			//{
			//	Destroy(unit.gameObject);
			//}
		}
	}

	/// <summary>
	/// 从originPos绘制空网格
	/// </summary>
	public Grid(int xCount, int yCount, Vector2 originPos)
	{
		// 初始化网格绑定的游戏对象
		gridObj = new GameObject("Grid Objects");
		// 初始化unitArr
		unitArr = new Unit[xCount, yCount];
		this.OriginPos = originPos;

		// 绘制网格
		for (int x = 0; x < xCount; x++)
		{
			for (int y = 0; y < yCount; y++)
			{
				GameObject squareObj = Instantiate(resourceController.square, gridObj.transform);
				squareObj.transform.position = Coord2WorldPos(new Coord(x, y));
				squareObj.GetComponent<SpriteRenderer>().sortingLayerName = "Area";
				// 根据所在网格设置alpha
				// 放置位置背景颜色深度
				const float COLOR_ALPHA = 0.2f;
				Color color = Color.white;
				color.a = ((x + y) % 2 == 0) ? (COLOR_ALPHA / 2) : COLOR_ALPHA;
				squareObj.GetComponent<SpriteRenderer>().color = color;
			}
		}
	}

	// 返回grid中,离世界坐标pos最近的网格的(x, y)
	// 返回null: 找不到
	public Coord GetClosestCoord(Vector2 pos)
	{
		Coord ret = Coord.OUTSIDE;
		// 检测的最大范围
		const float MAX_DISTANCE = 0.6f;
		float minDistance = MAX_DISTANCE;
		for (int x = 0; x < GetX(); x++)
		{
			for (int y = 0; y < GetY(); y++)
			{
				Vector2 gridWorldPos = Coord2WorldPos(new Coord(x, y));
				float distance = (pos - gridWorldPos).magnitude;
				// 更新最近的点
				if (distance < minDistance)
				{
					minDistance = distance;
					ret = new Coord(x, y);
				}
			}
		}
		return ret;
	}

	// 根据x,y
	// 返回绝对坐标, 需要在初始化完后使用
	public Vector2 Coord2WorldPos(Coord coord)
	{
		return Coord2LocalPos(coord) + OriginPos;
	}

	/// <summary>
	///	连接grids中的所有Block 
	/// </summary>
	public void LinkBlocks()
	{
		for (int x = 0; x < GetX(); x++)
		{
			for (int y = 0; y < GetY(); y++)
			{
				Block block = unitArr[x, y] as Block;
				// 是block
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

	public Unit Get(Coord coord)
	{
		Debug.Assert(InGrid(coord));
		return unitArr[coord.x, coord.y];
	}

	public void Set(Coord coord, Unit unit)
	{
		Debug.Assert(InGrid(coord));
		unitArr[coord.x, coord.y] = unit;
	}

	// 清空网格
	public void ClearGrid()
	{
		foreach (Unit unit in unitArr)
		{
			if (unit != null)
			{
				Destroy(unit.gameObject);
			}
		}
	}

	// TODO:
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

	// 在网格中
	public bool InGrid(Unit unit)
	{
		return InGrid(unit.coord);
	}

	// 坐标是否在网格中
	public bool InGrid(Coord coord)
	{
		return coord.x >= 0 && coord.x < GetX()
			&& coord.y >= 0 && coord.y < GetY();
	}

	/// <summary>
	/// 更改网格的显示状态
	/// </summary>
	public void SetShow(bool status)
	{
		gridObj.SetActive(status);
	}

	public List<Unit> GetUnits()
	{
		return unitArr.OfType<Unit>().ToList();
	}

	/// <summary>
	/// 获取右上角的世界坐标
	/// </summary>
	public Vector2 GetRightTopPos()
	{
		Vector2 gridSize = new Vector2(GRID_SIZE * GetX(), GRID_SIZE * GetY());
		return OriginPos + gridSize;
	}

	//--------------------- 内部工具函数 ----------------------//

	// 根据x,y
	// 返回相对坐标
	Vector2 Coord2LocalPos(Coord coord)
	{
		return new Vector2(
			GRID_SIZE * (coord.x + 0.5f),
			GRID_SIZE * (coord.y + 0.5f));
	}

	// 连接两Block
	void Link(Block block, Block another, int direction)
	{
		int directionNeg = Negative(direction);

		block.LinkTo(another, direction);
		another.LinkTo(block, directionNeg);
	}

	// 检测两Block是否能连接
	bool CanLink(Block block, Block another, int direction)
	{
		int dirNeg = Negative(direction);
		// Block的玩家的相同
		bool samePlayer = (block.player == another.player);
		// Block的相应方向是可连接的
		bool linkAvailable = block.IsLinkAvailable(direction) 
			&& another.IsLinkAvailable(dirNeg);

		return samePlayer && linkAvailable;
	}

	// 根据方向获取(x,y)相邻的Block
	Block GetNeighborBlock(int x, int y, int direction)
	{
		// 超出边界
		if ((direction == 0 && x == GetX() - 1)
			|| (direction == 1 && y == GetY() - 1)
			|| (direction == 2 && x == 0)
			|| (direction == 3 && y == 0))
		{
			return null;
		}

		int anotherX = x + DIR4[direction, 0];
		int anotherY = y + DIR4[direction, 1];
		return unitArr[anotherX, anotherY] as Block;
	}

	int Negative(int direction)
	{
		return (direction + 2) % 4;
	}

	// 网格最大X
	int GetX()
	{
		return unitArr.GetLength(0);
	}

	// 网格最大Y
	int GetY()
	{
		return unitArr.GetLength(1);
	}
}

public struct Coord
{
	public int x;
	public int y;
	public static readonly Coord OUTSIDE = new Coord(-1, -1);

	public Coord(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public static Coord operator +(Coord c1, Coord c2)
	{
		return new Coord(c1.x + c2.x, c1.y + c2.y);
	}

	public static Coord operator -(Coord c1, Coord c2)
	{
		return new Coord(c1.x - c2.x, c1.y - c2.y);
	}

	public static bool operator ==(Coord c1, Coord c2)
	{
		return c1.Equals(c2);
	}

	public static bool operator !=(Coord c1, Coord c2)
	{
		return !c1.Equals(c2);
	}

	public override bool Equals(System.Object another)
	{
		if (!(another is Coord))
		{
			return false;
		}

		Coord anotherC = (Coord)another;
		return x == anotherC.x && y == anotherC.y;
	}
}
