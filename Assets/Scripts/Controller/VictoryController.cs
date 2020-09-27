﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

public class VictoryController : MonoBehaviour
{
    public enum VictoryCondition { KillBall, EnterLocation, HoldOn }
    public enum TargetOption { All, Target }

    public VictoryCondition victoryCondition;
    public TargetOption targetOption;

    public Unit[] targets;
    private string targetName = "Target";

    public bool isVictory;

    private GameController gameController;

    void Awake()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    public void Init()
    {
        // KillBall + All
        if (victoryCondition == VictoryCondition.KillBall && targetOption == TargetOption.All)
        {
            targets = gameController.GetUnits(Player.Enemy, "Ball");
        }

        // * + Target
        if (targetOption == TargetOption.Target)
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
    }

    void Update()
    {
        switch (victoryCondition)
        {
            case VictoryCondition.KillBall:
                KillBall();
                break;
            case VictoryCondition.EnterLocation:
                break;
        }
    }

    void KillBall()
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
}