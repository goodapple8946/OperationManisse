﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class VictoryController : MonoBehaviour
{
    private string targetName = "Target";

    [HideInInspector] public VictoryCondition victoryCondition;
    [HideInInspector] public Unit[] targets;
    [HideInInspector] public float holdOnTimeOrigin;
    [HideInInspector] public float holdOnTime;
    [HideInInspector] public bool isVictory;

    public void Init()
    {
        isVictory = false;

        // Kill All：将所有敌人放入Targets
        if (victoryCondition == VictoryCondition.KillAll)
        {
            targets = gameController.GetUnits(Player.Enemy, "Ball");
        }

        // Kill Target：将名称为"Target"的敌人放入Targets
        if (victoryCondition == VictoryCondition.KillTarget)
        {
            ArrayList arr = new ArrayList();

            Unit[] units = gameController.GetUnits();
            foreach (Unit unit in units)
            {
                if (unit.gameObject.name == targetName)
                {
                    arr.Add(unit);
                }
            }
            targets = (Unit[])arr.ToArray(typeof(Unit));
        }

        // Hold On：初始化时间
        holdOnTime = holdOnTimeOrigin;
    }

    void Update()
    {
        if (gameController.gamePhase == GamePhase.Playing)
        {
            switch (victoryCondition)
            {
                case VictoryCondition.KillAll:
                    KillTarget();
                    break;
                case VictoryCondition.KillTarget:
                    KillTarget();
                    break;
                case VictoryCondition.HoldOn:
                    HoldOn();
                    break;
            }
        }
    }

    void KillTarget()
    {
        foreach (Unit target in targets)
        {
            if (target != null && target.IsAlive())
            {
                return;
            }
        }
        isVictory = true;
    }

    void HoldOn()
    {
        holdOnTime -= Time.deltaTime;
        if (holdOnTime <= 0)
        {
            isVictory = true;
        }
    }
}
