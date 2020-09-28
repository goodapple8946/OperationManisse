using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static VictoryController;

public class EditorVictoryCondition : MonoBehaviour
{
    private Dropdown dropdown;

    void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(value =>
        {
            VictoryCondition victoryCondition = (VictoryCondition)value;
            GameObject.Find("Victory Controller").GetComponent<VictoryController>().victoryCondition = victoryCondition;
        });
    }
}
