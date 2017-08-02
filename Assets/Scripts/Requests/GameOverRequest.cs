using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class GameOverRequest : BaseRequest {

    private GamePanel gamePanel;
    private ReturnCode returnCode;

    private bool isGameOver = false;

    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.GameOver;

        gamePanel = GetComponent<GamePanel>();

        base.Awake();
    }

    public void Update()
    {
        if (isGameOver)
        {
            gamePanel.OnGameOverResponse(returnCode);
            isGameOver = false;
        }
    }

    public override void OnResponse(string data)
    {
        returnCode = (ReturnCode)int.Parse(data);
        isGameOver = true;
    }
}
