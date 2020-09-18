using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    public Unit unit;

    public float value;
    public float valueMax;

    public float valueBack = 0;
    private float valueSpeed = 0.5f;
    private Vector2 offset = new Vector2(0, 0.48f);

    void Update()
    {
        if (unit == null || !unit.isAlive)
        {
            Destroy(gameObject);
        }
        else
        {
            if (unit.health == unit.healthMax)
            {
                transform.localScale = Vector2.zero;
            }
            else
            {
                transform.localScale = Vector2.one;

                value = unit.health;
                valueMax = unit.healthMax;
                if (valueBack < value)
                {
                    valueBack = value;
                }
                else if (valueBack > value)
                {
                    valueBack -= valueSpeed * valueMax * Time.deltaTime;
                }
                transform.position = (Vector2)unit.transform.position + offset;

                float frontScale = value / valueMax;
                transform.GetChild(1).localScale = new Vector3(frontScale, 1, 1);
                transform.GetChild(1).transform.position = transform.position + new Vector3(((frontScale - 1) / 2) * 0.64f, 0, 0);

                float backScale = valueBack / valueMax;
                transform.GetChild(2).localScale = new Vector3(backScale, 1, 1);
                transform.GetChild(2).transform.position = transform.position + new Vector3(((backScale - 1) / 2) * 0.64f, 0, 0);
            }
        }
    }
}
