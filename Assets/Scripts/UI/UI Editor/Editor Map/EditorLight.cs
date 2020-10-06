using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorLight : MonoBehaviour
{
    Slider slider;

    public void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(value =>
        {
			editorController.LightIntensity = value;
        });
    }

	public void UpdateShowing()
	{
		slider.SetValueWithoutNotify(editorController.LightIntensity);
	}
}
