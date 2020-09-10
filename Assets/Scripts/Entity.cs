using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private GameController gameController;
    private Rigidbody2D body;

    public static int count = 0;

    // 将GameObject强制类型转换为Entity
    public static explicit operator Entity(GameObject gameObject)
    {
        return gameObject.GetComponent<Entity>();
    }

    void Start()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        body = GetComponent<Rigidbody2D>();
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

    // 拖动
    private void Drag()
    {
        transform.position += (Vector3)MouseController.offset;
    }
    

    private void OnMouseDrag()
    {
        if (GameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            Drag();
        }
    }

    // 测试：鼠标点击
    public void OnMouseDown()
    {

    }
}
