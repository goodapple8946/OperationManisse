using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

	private GameObject mainCamera;

	private ResourceController resourceController;

	void Start()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
		gamePhaseBackup = GamePhase.Preparation;
		mainCamera = GameObject.Find("Main Camera");
		resourceController = GameObject.Find("Resource Controller").GetComponent<ResourceController>();
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

	// 将CameraController是否播放声音反转
	public void UIAudio()
	{
		AudioListener audioListener = mainCamera.GetComponent<AudioListener>();
		Image buttonImage 
			= EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
		if (audioListener.enabled)
		{
			buttonImage.sprite = resourceController.mute;
		}
		else
		{
			buttonImage.sprite = resourceController.unmute;
		}
		audioListener.enabled = !audioListener.enabled;
	}

	public void UIToLevel()
    {
        SceneManager.LoadScene("Level Panel");
	}
}
