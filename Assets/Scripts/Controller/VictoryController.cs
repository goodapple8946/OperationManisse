using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryController : MonoBehaviour
{
    public enum VictoryCondition { KillBall, EnterLocation }
    public enum TargetOption { All, Target }

    public VictoryCondition victoryCondition;
    public TargetOption targetOption;

    public Unit[] targets;

    public bool isVictory;

    public void Init()
    {
        // KillBall + All
        if (victoryCondition == VictoryCondition.KillBall && targetOption == TargetOption.All)
        {
            ArrayList units = new ArrayList();
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ball");
            foreach (GameObject gameObject in gameObjects)
            {
                Unit unit = gameObject.GetComponent<Unit>();
                if (unit != null && unit.player == Unit.Player.Enemy)
                {
                    units.Add(unit);
                }
            }
            targets = (Unit[])units.ToArray(typeof(Unit));
        }

        // * + Target
        if (targetOption == TargetOption.Target)
        {
            ArrayList units = new ArrayList();

            ArrayList gameObjects = new ArrayList();
            gameObjects.AddRange(GameObject.FindGameObjectsWithTag("Ball"));
            gameObjects.AddRange(GameObject.FindGameObjectsWithTag("Block"));
            foreach (GameObject gameObject in gameObjects)
            {
                Unit unit = gameObject.GetComponent<Unit>();
                if (unit != null && unit.gameObject.name == "Target")
                {
                    units.Add(unit);
                }
            }
            targets = (Unit[])units.ToArray(typeof(Unit));
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
            if (target != null)
            {
                return;
            }
        }
        isVictory = true;
    }
}
