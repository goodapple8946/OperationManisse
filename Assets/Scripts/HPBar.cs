using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

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

    private new SpriteRenderer renderer;
    private EditorController editorController;

    void Awake()
    {
        renderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();
    }

    void Start()
    {
        valueMax = unit.healthMax;
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = back;
    }

    void Update()
    {
        if (unit == null)
        {
            Destroy(gameObject);
        }
        else
        {
            if ((unit.health != unit.healthMax || editorController.IsShowingHP) &&
                unit != editorController.mouseObject as Unit)
            {
                if (gameController.gamePhase == GamePhase.Editor || !init)
                {
                    switch (unit.player)
                    {
                        case Player.Neutral:
                            renderer.sprite = frontNeutral;
                            break;
                        case Player.Player:
                            renderer.sprite = frontPlayer;
                            break;
                        case Player.Enemy:
                            renderer.sprite = frontEnemy;
                            break;
                    }
                    init = true;
                }

                transform.localScale = Vector2.one;

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
            else
            {
                transform.localScale = Vector2.zero;
            }
        }
    }
}
