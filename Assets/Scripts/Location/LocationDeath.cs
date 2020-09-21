using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationDeath : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();

        if (unit != null && unit.isAlive)
        {
            unit.health = 0;
        }
    }
}
