using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Controller;

/// <summary>
/// 根据GameObject创建各种类型的克隆
/// </summary>
public static class CorpseFactory
{

	// 死亡扭矩
	private static readonly float TorqueDeath = 1000.0f;

	/// <summary>
	/// 根据origin, 创建一个保留图像,旋转刚体,烧黑的克隆
	/// </summary>
	public static GameObject CreateBurningClone(GameObject origin)
	{
		GameObject clone = CreateUnfixedRigidClone(origin);

		ChangeColor(clone, new Color(0.1f, 0.1f, 0.1f, 1.0f));
		
		return clone;
	}

	/// <summary>
	/// 创建一个半透明移除刚体仅保留图像的刚体
	/// </summary>
	public static GameObject CreateTransparentGraphicClone(GameObject origin)
	{
		GameObject clone = CreateGraphicFixedRigidClone(origin);
		Remove<Rigidbody2D>(clone);
		ChangeColor(clone, new Color(1, 1, 1, 0.5f));
		return clone;
	}

	/// <summary>
	/// 创建一个不固定的刚体克隆
	/// </summary>
	public static GameObject CreateUnfixedRigidClone(GameObject origin)
	{
		GameObject clone = CreateGraphicFixedRigidClone(origin);
		RemoveRigidFixed(clone);
		return clone;
	}

	/// <summary>
	/// 创建一个旋转刚体克隆
	/// </summary>
	public static GameObject CreateRotatedRigidClone(GameObject origin)
	{
		GameObject clone = CreateUnfixedRigidClone(origin);
		AddRotatedRemoveFixed(clone);
		return clone;
	}

	/// <summary>
	/// 创建一个删去脚本和碰撞体, 仅保留图像和固定刚体的克隆
	/// </summary>
	public static GameObject CreateGraphicFixedRigidClone(GameObject origin)
	{
		// 将origin整体复制
		Transform clone = GameObject.Instantiate(origin.transform);
		// 将tag改成Untagged就不会被FindEnemy
		clone.tag = "Untagged";

		// 移除所有脚本和碰撞体组件
		Remove<MonoBehaviour>(clone.gameObject);
		Remove<Collider2D>(clone.gameObject);

		return clone.gameObject;
	}

	/// <summary>
	/// 取消物体某种类型的所有组件
	/// </summary>
	private static GameObject Remove<T>(GameObject obj) where T : Component
	{
		T[] components = obj.GetComponentsInChildren<T>();
		System.Array.ForEach(components, component => GameObject.Destroy(component));
		return obj;
	}

	/// <summary>
	/// 取消刚体的固定
	/// </summary>
	private static GameObject RemoveRigidFixed(GameObject clone)
	{
		Rigidbody2D cloneBody = clone.GetComponent<Rigidbody2D>();
		cloneBody.constraints = RigidbodyConstraints2D.None;
		// 无阻力
		cloneBody.drag = 0;
		cloneBody.angularDrag = 0;
		return clone;
	}

	/// <summary>
	/// 取消刚体固定, 添加旋转扭矩
	/// </summary>
	private static GameObject AddRotatedRemoveFixed(GameObject clone)
	{
		RemoveRigidFixed(clone);

		Rigidbody2D cloneBody = clone.GetComponent<Rigidbody2D>();
		// 死亡扭矩
		cloneBody.AddTorque(TorqueDeath);

		return clone;
	}

	private static GameObject ChangeColor(GameObject clone, Color color)
	{
		SpriteRenderer[] renderers = 
			clone.transform.GetComponentsInChildren<SpriteRenderer>();

		System.Array.ForEach(renderers,
			renderer => renderer.color = color);
		return clone;
	}

	[System.Diagnostics.Conditional("DEBUG")]
	private static void CheckOrigin(GameObject origin)
	{
		Debug.Assert(origin.GetComponent<Rigidbody2D>() != null, "物体必须为刚体");
	}
}
