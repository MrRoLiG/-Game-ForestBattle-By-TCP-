using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

public class RoomPanel : BasePanel {

    private Text localPlayerUsername;
    private Text localPlayerTotalCount;
    private Text localPlayerWinCount;

    private Text EnemyPlayerUsername;
    private Text EnemyPlayerTotalCount;
    private Text EnemyPlayerWinCount;

    private Transform bluePanel;
    private Transform redPanel;
    private Transform startButton;
    private Transform exitButton;

    private UserData userData = null;

    private UserData userData1 = null;
    private UserData userData2=null;

    private QuitRoomRequest quitRoomRequest;
    private StartGameRequest startGameRequest;

    private bool isPopPanel = false;
    private bool isPushGamePanel=false;

    /// <summary>
    /// 初始化上面定义的各个组件
    /// </summary>
    private void Start()
    {
        localPlayerUsername = transform.Find("BluePanel/Username").GetComponent<Text>();
        localPlayerTotalCount = transform.Find("BluePanel/TotalCount").GetComponent<Text>();
        localPlayerWinCount = transform.Find("BluePanel/WinCount").GetComponent<Text>();

        EnemyPlayerUsername = transform.Find("RedPanel/Username").GetComponent<Text>();
        EnemyPlayerTotalCount = transform.Find("RedPanel/TotalCount").GetComponent<Text>();
        EnemyPlayerWinCount = transform.Find("RedPanel/WinCount").GetComponent<Text>();

        bluePanel = transform.Find("BluePanel");
        redPanel = transform.Find("RedPanel");
        startButton = transform.Find("StartButton");
        exitButton = transform.Find("ExitButton");

        transform.Find("StartButton").GetComponent<Button>().onClick.AddListener(OnStartClick);
        transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(OnExitClick);

        quitRoomRequest = GetComponent<QuitRoomRequest>();
        startGameRequest = GetComponent<StartGameRequest>();

        EnterAnim();
    }

    private void Update()
    {
        if (userData != null)
        {
            SetLocalPlayerRes(userData.Username, userData.TotalCount.ToString(), userData.WinCount.ToString());
            ClearEnemyPlayerRes();
            userData = null;
        }
        if (userData1 != null)
        {
            SetLocalPlayerRes(userData1.Username, userData1.TotalCount.ToString(), userData1.WinCount.ToString());
            if (userData2 != null)
            {
                SetEnemyPlayerRes(userData2.Username, userData2.TotalCount.ToString(), userData2.WinCount.ToString());
            }
            //如果该房间没有另外一个人加入的时候，显示等待界面
            else
            {
                ClearEnemyPlayerRes();
            }
            userData1 = null;
            userData2 = null;
        }
        if (isPopPanel)
        {
            uiManager.PopPanel();
            isPopPanel = false;
        }
    }

    /// <summary>
    /// 重写进入界面方法
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        if (bluePanel != null)
        {
            EnterAnim();
        }
    }

    /// <summary>
    /// 重写离开界面
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        HideAnim();
    }

    /// <summary>
    /// 重写暂停界面
    /// </summary>
    public override void OnPause()
    {
        base.OnPause();
        HideAnim();
    }

    /// <summary>
    /// 重写继续界面
    /// </summary>
    public override void OnResume()
    {
        base.OnResume();
        EnterAnim();
    }

    /// <summary>
    /// 设置显示本地房主的信息
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="totalCount"></param>
    /// <param name="winCount"></param>
    public void SetLocalPlayerRes(string userName,string totalCount,string winCount)
    {
        localPlayerUsername.text = userName;
        localPlayerTotalCount.text = "TotalCount:\n"+totalCount;
        localPlayerWinCount.text = "WinCount:\n"+winCount;
    }

    /// <summary>
    /// 设置显示本地房主的信息（异步）
    /// </summary>
    public void SetLocalPlayerResAsync()
    {
        userData = facade.GetUserData();
    }
    public void SetAllPlayerResAsync(UserData userData1,UserData userData2)
    {
        this.userData1 = userData1;
        this.userData2 = userData2;
    }

    /// <summary>
    /// 设置显示加入房间者的信息
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="totalCount"></param>
    /// <param name="winCount"></param>
    public void SetEnemyPlayerRes(string userName,string totalCount,string winCount)
    {
        EnemyPlayerUsername.text = userName;
        EnemyPlayerTotalCount.text = "TotalCount:\n" + totalCount;
        EnemyPlayerWinCount.text = "WinCount:\n" + winCount;
    }

    /// <summary>
    /// 设置显示当前房间没有人加入的界面
    /// </summary>
    public void ClearEnemyPlayerRes()
    {
        EnemyPlayerUsername.text = "";
        EnemyPlayerTotalCount.text = "Waiting to join...";
        EnemyPlayerWinCount.text = "";
    }

    /// <summary>
    /// 给本界面的开始战斗的按钮添加一个触发事件
    /// </summary>
    private void OnStartClick()
    {
        startGameRequest.SendRequest();
    }

    /// <summary>
    /// 给本界面的离开房间界面添加一个触发事件
    /// </summary>
    private void OnExitClick()
    {
        quitRoomRequest.SendRequest();
    }

    /// <summary>
    /// 设置进入该界面的动画播放
    /// </summary>
    private void EnterAnim()
    {
        gameObject.SetActive(true);

        bluePanel.localPosition = new Vector3(-1000, 0, 0);
        redPanel.localPosition = new Vector3(1000, 0, 0);

        bluePanel.DOLocalMoveX(-176, 0.5f);
        redPanel.DOLocalMoveX(176, 0.5f);

        startButton.localScale = Vector3.zero;
        startButton.DOScale(1, 0.5f);

        exitButton.localScale = Vector3.zero;
        exitButton.DOScale(1, 0.5f);
    }

    /// <summary>
    /// 设置离开该界面的动画播放
    /// </summary>
    private void HideAnim()
    {
        bluePanel.DOLocalMoveX(-1000, 0.5f);
        redPanel.DOLocalMoveX(1000, 0.5f);

        startButton.DOScale(0, 0.5f);
        exitButton.DOScale(0, 0.5f).OnComplete(()=>gameObject.SetActive(facade));
    }

    /// <summary>
    /// 离开该界面的请求的响应
    /// </summary>
    public void OnExitResponse()
    {
        isPopPanel = true;
    }

    /// <summary>
    /// 点击开始游戏请求的响应
    /// </summary>
    /// <param name="returnCode"></param>
    public void OnStartResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Fail)
        {
            uiManager.ShowMessageAsync("您不是房主，无法开始游戏...");
        }
        else
        {
            //todo
            //开启游戏成功，进入游戏，实例化游戏角色，控制相机视野的跟随
            uiManager.PushPanelAsync(UIPanelType.Game);
            facade.EnterPlayingAsync();
        }
    }
}
