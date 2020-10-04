using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using static Controller;

public class EditorLoadModule : MonoBehaviour
{
	private void Awake()
	{
		Button button = GetComponent<Button>();
		button.onClick.AddListener(LoadModuleFromFS);
	}

	/// <summary>
	/// 从文件系统中加载保存的xml文件
	/// </summary>
	public void LoadModuleFromFS()
	{
		try
		{
			// 根据文件生成Game
			string path = ResourceController.OpenFilePanel(
				"Choose a module", ResourceController.ModulePath, "xml");
			// 玩家选择了文件,则加载游戏
			if (path != "")
			{
				int worldStartX = 2;
				int worldStartY = 2;
				XMLModule module = Serializer.Deserialized<XMLModule>(path);
				bool canPlace = CanPlace(module, worldStartX, worldStartY);
				// 如果能防止就放置, 否则报错
				if (canPlace)
				{
					DisplayModule(module, 0, 0);
					Load(module, worldStartX, worldStartY);
				}
				else
				{
					ResourceController.DisplayDialog("", "Cannot Put Moudle in x:" + worldStartX + " y:" +worldStartY + "!", "ok");
				}
			}
		}
		// xml文件错误,显示错误弹窗
		catch (System.Exception e)
		{
			ResourceController.DisplayDialog("", "Module File Error!", "ok");
		}
	}

	// 从(worldStartX, worldStartY)开始展示module 
	public void DisplayModule(XMLModule module, int worldStartX, int worldStartY)
	{
		// 加载单位信息
		for (int moduleX = 0; moduleX < module.xNum; moduleX++)
		{
			for (int moduleY = 0; moduleY < module.yNum; moduleY++)
			{
				XMLUnit xmlUnit = module.Grid[moduleX, moduleY];
				if(xmlUnit != null)
				{
					int worldX = worldStartX + moduleX;
					int worldY = worldStartY + moduleY;
					// 修改xmlUnit的坐标
					xmlUnit.x = worldX;
					xmlUnit.y = worldY;

					Unit unit = EditorLoad.XML2Unit(xmlUnit);
					GameObject clone = CorpseFactory.CreateTransparentGraphicClone(unit.gameObject);
					Destroy(unit.gameObject);
					// 保留一会删除重绘制
					Destroy(clone, 4f);
				}
			}
		}
	}

	// 加载module对象使其左下角在世界坐标(x,y)
	// 将模组中每一个物品放到世界网格对应格中
	private void Load(XMLModule module, int worldStartX, int worldStartY)
	{
		// 加载单位信息
		for (int moduleX = 0; moduleX < module.xNum; moduleX++)
		{
			for (int moduleY = 0; moduleY < module.yNum; moduleY++)
			{
				// 如果xmlUnit存在
				XMLUnit xmlUnit = module.Grid[moduleX, moduleY];
				if (xmlUnit != null)
				{
					int worldX = worldStartX + moduleX;
					int worldY = worldStartY + moduleY;
					// 修改xmlUnit的坐标到Editor坐标网并加载
					xmlUnit.x = worldX;
					xmlUnit.y = worldY;
					EditorLoad.Load(xmlUnit);
				}
			}
		}
	}

	// 检测是否能放入,true:可以
	private bool CanPlace(XMLModule module, int worldStartX, int worldStartY)
	{
		// module内的相对坐标区域
		for (int moduleX = 0; moduleX < module.xNum; moduleX++)
		{
			for (int moduleY = 0; moduleY < module.yNum; moduleY++)
			{
				int worldX = worldStartX + moduleX;
				int worldY = worldStartY + moduleY;

				// 模组中的某格存在物品 && 世界坐标对应格超出或存在物品
				bool mappingGridInvalid = (!editorController.IsLegalCoord(worldX, worldY)
						|| editorController.Grid[worldX, worldY] != null);
				if (module.Grid[moduleX, moduleY] != null && mappingGridInvalid)
				{
					return false;
				}
			}
		}
		return true;
	}
}
