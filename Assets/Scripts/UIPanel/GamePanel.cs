using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

public class GamePanel : BasePanel {

    private Text timer;
    private Button victoryButton;
    private Button defeatButton;
    private Button exitButton;

    private QuitBattleRequest quitBattleRequest;

    private int time = -1;

    private void Start()
    {
        timer = transform.Find("Timer").GetComponent<Text>();
        victoryButton = transform.Find("VictoryButton").GetComponent<Button>();
        victoryButton.onClick.AddListener(OnResultClick);
        victoryButton.gameObject.SetActive(false);
        defeatButton = transform.Find("DefeatButton").GetComponent<Button>();
        defeatButton.onClick.AddListener(OnResultClick);
        defeatButton.gameObject.SetActive(false);

        exitButton = transform.Find("ExitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(OnExitClick);
        exitButton.gameObject.SetActive(false);

        timer.gameObject.SetActive(false);

        quitBattleRequest = GetComponent<QuitBattleRequest>();
    }

    public void Update()
    {
        if (time > -1)
        {
            ShowTime(time);
            time = -1;
        }
    }

    public override void OnEnter()
    {
        gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        victoryButton.gameObject.SetActive(false);
        defeatButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 游戏开始的时候进行倒计时
    /// </summary>
    /// <param name="time"></param>
    public void ShowTime(int time)
    {
        if (time == 3)
        {
            exitButton.gameObject.SetActive(true);
        }
        timer.gameObject.SetActive(true);

        timer.text = time.ToString();

        timer.transform.localScale = Vector3.one;
        Color tempColor = timer.color;
        tempColor.a = 1;
        timer.color = tempColor;

        timer.transform.DOScale(2, 0.3f).SetDelay(0.3f);
        timer.DOFade(0, 0.3f).SetDelay(0.3f).OnComplete(()=>timer.gameObject.SetActive(false));

        facade.PlayNormalSound(AudioManager.Sound_Alert);
    }

    /// <summary>
    /// 游戏开始的时候进行倒计时(异步)
    /// </summary>
    /// <param name="time"></param>
    public void ShowTimeAsync(int time)
    {
        this.time = time;
    }

    /// <summary>
    /// 当游戏结束的时候做出的响应
    /// </summary>
    /// <param name="returnCode"></param>
    public void OnGameOverResponse(ReturnCode returnCode)
    {
        Button tempButton=null;
        switch (returnCode)
        {
            case ReturnCode.Success:
                tempButton = victoryButton;
                break;
            case ReturnCode.Fail:
                tempButton = defeatButton;
                break;
        }
        tempButton.gameObject.SetActive(true);
        tempButton.transform.localScale = Vector3.zero;
        tempButton.transform.DOScale(1, 0.5f);
    }

    /// <summary>
    /// 为结束战斗回到房间列表添加触发方法
    /// </summary>
    private void OnResultClick()
    {
        uiManager.PopPanel();
        uiManager.PopPanel();
        //这里需要弹出两次才能到房间列表的页面，只弹出一次只能到达等待加入界面
        facade.GameOver();
    }

    /// <summary>
    /// 给退出按钮添加一个触发事件
    /// </summary>
    private void OnExitClick()
    {
        quitBattleRequest.SendRequest();
    }

    /// <summary>
    /// 客户端对退出按钮按下的响应
    /// </summary>
    public void OnExitResponse()
    {
        OnResultClick();
    }
}
