using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller: MonoBehaviour
{
    public static GameController     gameController;
    public static EditorController   editorController;
    public static MouseController    mouseController;
    public static ResourceController resourceController;
    public static VictoryController  victoryController;
    public static ShopController     shopController;

    public enum Player           { Neutral, Player, Enemy }
    public enum Layer            { Default, TransparentFX, IgnoreRaycast, Water = 4, UI, PlayerBall = 8, PlayerBlock, PlayerMissile, EnemyBall, EnemyBlock, EnemyMissile, Background, Ground }
    public enum GamePhase        { Editor, Preparation, Playing, Victory }
    public enum VictoryCondition { None, KillAll, KillTarget, EnterLocation, HoldOn }
    public enum EditorMode       { Unit, Background, Location }

    void Awake()
    {
        gameController     = GameObject.Find("Controller/Game Controller").    GetComponent<GameController>();
        editorController   = GameObject.Find("Controller/Editor Controller").  GetComponent<EditorController>();
        mouseController    = GameObject.Find("Controller/Mouse Controller").   GetComponent<MouseController>();
        resourceController = GameObject.Find("Controller/Resource Controller").GetComponent<ResourceController>();
        victoryController  = GameObject.Find("Controller/Victory Controller"). GetComponent<VictoryController>();
        shopController     = GameObject.Find("Controller/Shop Controller").    GetComponent<ShopController>();
    }
}