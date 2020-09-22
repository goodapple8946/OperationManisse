using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationVictory : MonoBehaviour
{
    public VictoryController victoryController;

    void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.gameObject.GetComponent<Unit>();

        if (unit != null && unit.player == Unit.Player.Player && victoryController.victoryCondition == VictoryController.VictoryCondition.EnterLocation)
        {
            if (victoryController.targetOption == VictoryController.TargetOption.All)
            {
                victoryController.isVictory = true;
            }
            else if (victoryController.targetOption == VictoryController.TargetOption.Target)
            {
                foreach (Unit target in victoryController.targets)
                {
                    if (target == unit)
                    {
                        victoryController.isVictory = true;
                        break;
                    }
                }
            }
        }
    }
}
