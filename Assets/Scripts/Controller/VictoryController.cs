using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryController : MonoBehaviour
{
    public enum VictoryCondition { KillAllBalls, KillTargets };

    public VictoryCondition victoryCondition;

    private Unit[] targets;

    public void Init()
    {
        // KillTargets：击杀所有名称为"Target"的Ball
        if (victoryCondition == VictoryCondition.KillTargets)
        {
            ArrayList units = new ArrayList();
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ball");
            foreach (GameObject gameObject in gameObjects)
            {
                Unit unit = gameObject.GetComponent<Unit>();
                if (unit.isAlive && !unit.isSelling && unit.player == 2 && unit.gameObject.name == "Target")
                {
                    units.Add(unit);
                }
            }
            targets = (Unit[])units.ToArray(typeof(Unit));
        }

        // KillAllBalls：击杀所有Ball
        if (victoryCondition == VictoryCondition.KillAllBalls)
        {
            ArrayList units = new ArrayList();
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ball");
            foreach (GameObject gameObject in gameObjects)
            {
                Unit unit = gameObject.GetComponent<Unit>();
                if (unit.isAlive && !unit.isSelling && unit.player == 2)
                {
                    units.Add(unit);
                }
            }
            targets = (Unit[])units.ToArray(typeof(Unit));
        }
    }

    public bool IsVictory()
    {
        switch (victoryCondition)
        {
            case VictoryCondition.KillAllBalls:
                return KillTargets();
            case VictoryCondition.KillTargets:
                return KillTargets();
        }
        return false;
    }

    bool KillTargets()
    {
        foreach(Unit target in targets)
        {
            if (target != null && target.isAlive)
            {
                return false;
            }
        }
        return true;
    }
}
