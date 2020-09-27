using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class EditorLight : MonoBehaviour
{
    Slider slider;

    public void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(value =>
        {
            GameObject.Find("Global Light").GetComponent<Light2D>().intensity = value;
        });
    }
}
