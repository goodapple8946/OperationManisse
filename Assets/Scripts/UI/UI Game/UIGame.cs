using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class UIGame : MonoBehaviour
{
    private Button buttonOption;
    private Button buttonReset;
    private Button buttonCamera;
    private Button buttonStop;
    private Button buttonStart;

    void Awake()
    {
        const string uiPath = "UI Canvas/UI Game/UI Left Top/";
        buttonOption = GameObject.Find(uiPath + "UI Option").GetComponent<Button>();
        buttonReset  = GameObject.Find(uiPath + "UI Reset" ).GetComponent<Button>();
        buttonCamera = GameObject.Find(uiPath + "UI Camera").GetComponent<Button>();
        buttonStop   = GameObject.Find(uiPath + "UI Stop"  ).GetComponent<Button>();
        buttonStart  = GameObject.Find(uiPath + "UI Start" ).GetComponent<Button>();
    }

    public void UpdateActive(GamePhase gamePhase)
    {
        switch (gamePhase)
        {
            case GamePhase.Preparation:
                buttonOption.interactable = true;
                buttonReset.interactable  = true;
                buttonCamera.interactable = true;
                buttonStop.interactable   = false;
                buttonStart.interactable  = true;
                break;
            case GamePhase.Playing:
                buttonOption.interactable = true;
                buttonReset.interactable  = false;
                buttonCamera.interactable = true;
                buttonStop.interactable   = true;
                buttonStart.interactable  = false;
                break;
            case GamePhase.Victory:
                buttonOption.interactable = true;
                buttonReset.interactable  = false;
                buttonCamera.interactable = false;
                buttonStop.interactable   = false;
                buttonStart.interactable  = false;
                break;
        }
    }
}
