using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyAction { JumpTowardsCore }

    public EnemyAction enemyAction;

    private GameController gameController;

    public Block core;

    void Awake()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    void Start()
    {
        switch (enemyAction)
        {
            case EnemyAction.JumpTowardsCore:
                StartCoroutine(JumpTowardsCore());
                break;
        }
    }

    IEnumerator JumpTowardsCore()
    {
        Vector2 forceJumpRight = new Vector2(50f, 30f);
        Vector2 forceJumpLeft = new Vector2(-50f, 30f);

        while (gameController.gamePhase == GameController.GamePhase.Playing && core != null)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ball");
            foreach (GameObject gameObject in gameObjects)
            {
                Ball ball = gameObject.GetComponent<Ball>();
                if (ball.isAlive && ball.body != null && ball.player == 2 && ball.IsGrounded())
                {
                    if (core.transform.position.x > ball.transform.position.x)
                    {
                        ball.body.AddForce(forceJumpRight);
                    }
                    else
                    {
                        ball.body.AddForce(forceJumpLeft);
                    }
                }
            }
        }

        yield return new WaitForSeconds(3f);
    }
}
