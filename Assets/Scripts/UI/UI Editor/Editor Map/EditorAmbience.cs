using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorAmbience : EditorUI
{
    private Dropdown dropdown;
    
    void Start()
    {
        dropdown = GetComponent<Dropdown>();

		// 根据天气创建选项
		List<string> options = new List<string>();
		Array.ForEach(resourceController.ambiences,
			ambience => options.Add(ambience.gameObject.name));
		dropdown.AddOptions(options);

		// 设置editorController当前的环境
		dropdown.onValueChanged.AddListener(value =>
        {
			editorController.CurrAmbience = resourceController.ambiences[value];
        });
    }

    public override void UpdateShowing()
    {
        // TODO
    }
}
