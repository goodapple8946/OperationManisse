using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

public class HPBar : MonoBehaviour
{
    public Unit unit;

    public float value;
    public float valueMax;

    public float valueBack = 0;
    private float valueSpeed = 1f;
    private Vector2 offset = new Vector2(0, 0.32f);
    private bool init = false;

    public Sprite frontNeutral;
    public Sprite frontPlayer;
    public Sprite frontEnemy;
    public Sprite back;

    void Update()
    {
        if (unit == null || !unit.IsAlive())
        {
            Destroy(gameObject);
        }
        else
        {
            if (unit.health != unit.healthMax)
            {
                if (!init)
                {
                    transform.localScale = Vector2.one;
                    valueMax = unit.healthMax;
                    if (unit.player == Player.Neutral)
                    {
                        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = frontNeutral;
                    }
                    else if (unit.player == Player.Player)
                    {
                        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = frontPlayer;
                    }
                    else if (unit.player == Player.Enemy)
                    {
                        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = frontEnemy;
                    }
                    transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = back;

                    init = true;
                }

                value = unit.health;
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
