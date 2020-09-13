using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // 这个按钮可以在哪个游戏阶段使用
    public GameController.GamePhase[] interactablePhases;

    private GameController gameController;

    void Start()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    void Update()
    {
        InteractableCheck();
    }

    // 检测这个阶段是否可用
    void InteractableCheck()
    {
        foreach (GameController.GamePhase interactablePhase in interactablePhases)
        {
            if (interactablePhase == gameController.gamePhase)
            {
                GetComponent<Button>().interactable = true;
                return;
            }
        }
        GetComponent<Button>().interactable = false;
    }

    public void UIOption()
    {
        Application.Quit();
    }
    public void UIReset()
    {
        gameController.Reset();
    }

    public void UICamera()
    {
        CameraController cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        cameraController.follow = !cameraController.follow;
    }

    public void UIStop()
    {
        gameController.StopGame();
    }

    public void UIStart()
    {
        gameController.StartGame();
    }
}
