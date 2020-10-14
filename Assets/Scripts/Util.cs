using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Controller;

/// <summary>
/// 根据GameObject创建各种类型的克隆
/// </summary>
public static class Util
{

	// 死亡扭矩
	private static readonly float TorqueDeath = 360.0f;

	/// <summary>
	/// 根据origin, 创建一个保留图像,旋转刚体,烧黑的克隆
	/// </summary>
	public static GameObject CreateBurningClone(GameObject origin)
	{
		GameObject clone = CreateMovableClone(origin);
		SetColor(clone, new Color(0.1f, 0.1f, 0.1f, 1.0f));
		return clone;
	}

	/// <summary>
	/// 创建一个半透明移除刚体仅保留图像的刚体
	/// </summary>
	public static GameObject CreateModuleClone(GameObject origin, float alpha)
	{
		GameObject clone = CreateMovableClone(origin);
		// 删除刚体上的Joint
		Remove<Joint2D>(clone);
		Remove<Rigidbody2D>(clone);
		// 设置透明
		SetColor(clone, new Color(1, 1, 1, alpha));
		return clone;
	}

	/// <summary>
	/// 创建一个撞死的克隆
	/// </summary>
	public static GameObject CreatePunchClone(GameObject origin, Vector2 force)
	{
		GameObject clone = CreateMovableClone(origin);
		AddForce(clone, force);
		return clone;
	}

	/// <summary>
	/// 创建一个被杀死的克隆
	/// </summary>
	public static GameObject CreateShootedClone(GameObject origin)
	{
		GameObject clone = CreateMovableClone(origin);
		// 杀死旋转掉落
		AddRotated(clone);
		return clone;
	}

	private static GameObject CreateMovableClone(GameObject origin)
	{
		GameObject clone = CreateClone(origin);
		RemoveFixed(clone);
		Remove<Collider2D>(clone);
		return clone;
	}

	/// <summary>
	/// 创建一个复制品, 删去脚本
	/// </summary>
	private static GameObject CreateClone(GameObject origin)
	{
		// 将origin整体复制
		GameObject clone = GameObject.Instantiate(origin);
		// 将tag改成Untagged就不会被FindEnemy
		clone.tag = "Untagged";
		// 设置层级
		SetSpriteLayer(clone, "Dead");
		// 移除所有脚本
		Remove<MonoBehaviour>(clone);
		return clone;
	}

	/// <summary>
	/// 取消刚体的固定
	/// </summary>
	private static GameObject RemoveFixed(GameObject clone)
	{
		Rigidbody2D cloneBody = clone.GetComponent<Rigidbody2D>();
		cloneBody.constraints = RigidbodyConstraints2D.None;
		// 无空气阻力
		cloneBody.drag = 0;
		cloneBody.angularDrag = 0;
		return clone;
	}

	/// <summary>
	/// 取消刚体固定, 添加旋转扭矩
	/// </summary>
	private static GameObject AddRotated(GameObject clone)
	{
		Rigidbody2D cloneBody = clone.GetComponent<Rigidbody2D>();
		// 死亡扭矩
		cloneBody.AddTorque(TorqueDeath);
		return clone;
	}

	private static GameObject AddForce(GameObject clone, Vector2 force)
	{
		Rigidbody2D cloneBody = clone.GetComponent<Rigidbody2D>();
		// 死亡扭矩
		cloneBody.AddForce(force);
		return clone;
	}

	public static GameObject SetColor(GameObject obj, Color color)
	{
		SpriteRenderer[] renderers =
			obj.transform.GetComponentsInChildren<SpriteRenderer>();

		System.Array.ForEach(renderers,
			renderer => renderer.color = color);
		return obj;
	}

	// 设置图像层级
	public static void SetSpriteLayer(GameObject obj, string layer)
	{
		SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
		if (sprite != null)
		{
			sprite.sortingLayerName = layer;
		}
		SpriteRenderer[] sprites = obj.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteChild in sprites)
		{
			if (spriteChild.sortingLayerName != "Cover" && spriteChild.sortingLayerName != "Outline")
			{
				spriteChild.sortingLayerName = layer;
			}
		}
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

	[System.Diagnostics.Conditional("DEBUG")]
	private static void CheckOrigin(GameObject origin)
	{
		Debug.Assert(origin.GetComponent<Rigidbody2D>() != null, "物体必须为刚体");
	}

	[System.Diagnostics.Conditional("DEBUG")]
	private static void CheckRigid(GameObject obj)
	{
		Debug.Assert(obj.GetComponent<Rigidbody2D>() != null, "物体必须为刚体");
	}
}
