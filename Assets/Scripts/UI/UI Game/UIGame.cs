using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameController;

public class UIGame : MonoBehaviour
{
    private Button ButtonOption;
    private Button ButtonReset;
    private Button ButtonCamera;
    private Button ButtonStop;
    private Button ButtonStart;

    private GameController gameController;

    void Awake()
    {
        ButtonOption = transform.GetChild(0).GetChild(0).GetComponent<Button>();
        ButtonReset = transform.GetChild(0).GetChild(1).GetComponent<Button>();
        ButtonCamera = transform.GetChild(0).GetChild(2).GetComponent<Button>();
        ButtonStop = transform.GetChild(0).GetChild(3).GetComponent<Button>();
        ButtonStart = transform.GetChild(0).GetChild(4).GetComponent<Button>();

        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    public void UpdateActive()
    {
        switch (gamePhase)
        {
            case GamePhase.Preparation:
                ButtonReset.interactable = true;
                ButtonCamera.interactable = true;
                ButtonStop.interactable = false;
                ButtonStart.interactable = true;
                break;
            case GamePhase.Playing:
                ButtonReset.interactable = false;
                ButtonCamera.interactable = true;
                ButtonStop.interactable = true;
                ButtonStart.interactable = false;
                break;
            case GamePhase.Victory:
                ButtonReset.interactable = false;
                ButtonCamera.interactable = false;
                ButtonStop.interactable = false;
                ButtonStart.interactable = false;
                break;
        }
    }
}
