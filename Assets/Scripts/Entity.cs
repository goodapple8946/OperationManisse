using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private GameController gameController;
    public Rigidbody2D body;

    // 将GameObject强制类型转换为Entity
    public static explicit operator Entity(GameObject gameObject)
    {
        return gameObject.GetComponent<Entity>();
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 将目标Block放入该Entity，并且对双方Entity进行合并
    /// </summary>
    /// <param name="block">目标Block</param>
    public void AddBlock(Block block)
    {
        // 目标Block所处的目标Entity
        Entity entity = block.entity;

        // 将目标Entity合并入该Entity
        AttachEntity(entity);
    }

    /// <summary>
    /// 将目标Entity合并入该Entity
    /// </summary>
    /// <param name="entity">目标Entity</param>
    public void AttachEntity(Entity entity)
    {
        // 目标Entity中所有的Block
        Block[] blocks = entity.GetComponentsInChildren<Block>();

        // 将目标Entity中所有的Block放入该Entity
        foreach (Block block in blocks)
        {
            block.SetEntity(entity);
        }
    }

    /// <summary>
    /// 对Entity中每一个Block进行吸附检测
    /// </summary>
    public void BlockAdsorptionCheck()
    {
        // 临时吸附半径
        // 当有一个Block的一个方向成功吸附后，吸附要求应该变得严格
        float adsorptionDistanceTemp = Block.adsorptionDistance;

        // 吸附成功的Block，与它进行过的位移
        Block blockAbsorbed = null;
        Vector2 blockMovement = Vector2.zero;

        // 遍历所有Block，尝试吸附一个Block，移动剩余Block
        Block[] blocks = GetComponentsInChildren<Block>();
        foreach (Block block in blocks)
        {
            // 当有一个Block成功吸附后，其他Block不再需要检测，只需要进行该Block的同等位移操作后吸附即可
            if (blockAbsorbed == null)
            {
                // 遍历四个检测方向
                for (int i = 0; i < 4; i++)
                {
                    // 检测方向
                    Block.LinkDirection direction = (Block.LinkDirection)i;

                    // 吸附检测
                    Block blockResult = block.AdsorptionCheckDirection(direction, adsorptionDistanceTemp);

                    // 吸附并连接
                    if (blockResult != null)
                    {
                        blockAbsorbed = block;
                        blockMovement = block.Absorb(blockResult, direction);
                        adsorptionDistanceTemp = Block.adsorptionDistanceStrict;
                        break;
                    }
                }
            }
        }

        // 如果有Block成功吸附
        if(blockAbsorbed != null)
        {
            // 遍历所有其他Block，进行位移
            foreach (Block block in blocks)
            {
                if (block != blockAbsorbed)
                {
                    // 进行相同的位移
                    block.transform.position += (Vector3)blockMovement;
                }
            }

            // 遍历所有Block，进行吸附
            foreach (Block block in blocks)
            {
                // 遍历四个检测方向
                for (int i = 0; i < 4; i++)
                {
                    // 检测方向
                    Block.LinkDirection direction = (Block.LinkDirection)i;

                    // 吸附检测
                    Block blockResult = block.AdsorptionCheckDirection(direction, adsorptionDistanceTemp);

                    // 吸附并连接
                    if (blockResult != null)
                    {
                        block.Absorb(blockResult, direction);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 修改显示层级
    /// </summary>
    /// <param name="layer">显示层级</param>
    public void SetLayer(int layer)
    {
        Block[] blocks = GetComponentsInChildren<Block>();
        foreach (Block block in blocks)
        {
            block.SetLayer(layer);
        }
    }

    // 拖动
    private void Drag()
    {
        transform.position += (Vector3)MouseController.offset;
    }

    private void OnMouseDown()
    {
        if (GameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            SetLayer(1);
            if (body != null)
            {
                body.bodyType = RigidbodyType2D.Static;
            }
        }
    }

    private void OnMouseDrag()
    {
        if (GameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            Drag();
        }
    }

    private void OnMouseUp()
    {
        if (GameController.gamePhase == GameController.GamePhase.Preparation)
        {
            BlockAdsorptionCheck();
            SetLayer(0);
            if (body != null)
            {
                body.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }   
}
