using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameController;

public class UIController : MonoBehaviour
{
    // 这个按钮可以在哪个游戏阶段使用
    public GameController.GamePhase[] interactablePhases;

    private GameController gameController;
	// 点击option时保存gameController的gamePhase
	private GamePhase gamePhaseBackup;

    void Start()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
		gamePhaseBackup = GamePhase.Preparation;
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

	// 展开或叠起菜单
    public void UIOption()
    {
		GameObject menuObject = transform.GetChild(0).gameObject;
		menuObject.SetActive(!menuObject.activeSelf);
		// 第一次点击
		bool openMenu = (gameController.gamePhase != GamePhase.Menu);
		if (openMenu)
		{
			gamePhaseBackup = gameController.gamePhase;
			gameController.gamePhase = GamePhase.Menu;
		}
		// 第二次点击
		else
		{
			gameController.gamePhase = gamePhaseBackup;
		}
		// Application.Quit();
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

	public void UIVoice()
	{
		// TODO:
	}

	public void UIToLevel()
	{
		SceneManager.LoadScene("LevelPanel");
	}
}
