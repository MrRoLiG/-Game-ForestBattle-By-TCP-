using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Common;

public class RoomListPanel : BasePanel {

    private RectTransform BattleRes;
    private RectTransform RoomList;

    private VerticalLayoutGroup roomLayout;
    private GameObject roomItemPrefab;

    private List<UserData> userDataList = null;
    private ListRoomRequest listRoomRequest;
    private CreateRoomRequest createRoomRequest;
    private JoinRoomRequest joinRoomRequest;

    private UserData userData1=null;
    private UserData userData2=null;

    public void Start()
    {
        BattleRes = transform.Find("BattleRes").GetComponent<RectTransform>();
        RoomList = transform.Find("RoomList").GetComponent<RectTransform>();

        transform.Find("RoomList/CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick);
        transform.Find("RoomList/CreateRoomButton").GetComponent<Button>().onClick.AddListener(OnCreateRoomClick);
        transform.Find("RoomList/RefreshRoomListButton").GetComponent<Button>().onClick.AddListener(OnRefreshRoomListClick);

        roomLayout = transform.Find("RoomList/ScrollRec/LayOut").GetComponent<VerticalLayoutGroup>();
        roomItemPrefab = Resources.Load("UIPanel/RoomItem") as GameObject;

        listRoomRequest = GetComponent<ListRoomRequest>();
        createRoomRequest = GetComponent<CreateRoomRequest>();
        joinRoomRequest = GetComponent<JoinRoomRequest>();

        EnterAnim();
    }

    public void Update()
    {
        if (userDataList != null)
        {
            LoadRoomItem(userDataList);
            userDataList = null;
        }
        if (userData1 != null && userData2 != null)
        {
            BasePanel basePanel = uiManager.PushPanel(UIPanelType.Room);
            (basePanel as RoomPanel).SetAllPlayerResAsync(userData1, userData2);

            userData1 = null;
            userData2 = null;
        }
    }

    /// <summary>
    /// 重写进入界面
    /// </summary>
    public override void OnEnter()
    {
        if (BattleRes != null)
        {
            EnterAnim();
        }
        if (listRoomRequest == null)
        {
            listRoomRequest = GetComponent<ListRoomRequest>();
        }
        SetBattleRes();
        listRoomRequest.SendRequest();
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
        listRoomRequest.SendRequest();
    }

    /// <summary>
    /// 界面进入动画
    /// </summary>
    public void EnterAnim()
    {
        gameObject.SetActive(true);
        
        BattleRes.localPosition = new Vector3(-1000, 0, 0);
        RoomList.localPosition = new Vector3(1000, 0, 0);
        BattleRes.DOLocalMoveX(-260, 0.5f);
        RoomList.DOLocalMoveX(121, 0.5f);
    }

    /// <summary>
    /// 界面移出动画
    /// </summary>
    public void HideAnim()
    {
        BattleRes.DOLocalMoveX(-1000, 0.5f);
        RoomList.DOLocalMoveX(1000, 0.5f).OnComplete(()=>gameObject.SetActive(false));
        //使用Lambda表达式
    }

    /// <summary>
    /// 给该界面上的关闭按钮添加一个触发事件
    /// </summary>
    public void OnCloseClick()
    {
        PlayClickSound();
        uiManager.PopPanel();
    }

    /// <summary>
    /// 给该界面上的创建房间按钮添加一个触发事件
    /// </summary>
    public void OnCreateRoomClick()
    {
        PlayClickSound();
        BasePanel basePanel = uiManager.PushPanel(UIPanelType.Room);

        createRoomRequest.SetPanel(basePanel);
        createRoomRequest.SendRequest();
    }
    
    /// <summary>
    /// 给该界面上的刷新房间列表按钮添加一个触发事件
    /// </summary>
    public void OnRefreshRoomListClick()
    {
        PlayClickSound();
        listRoomRequest.SendRequest();
    }

    /// <summary>
    /// 设置个人信息版的显示方法
    /// </summary>
    public void SetBattleRes()
    {
        UserData userData = facade.GetUserData();
        //Debug.Log(userData.Username);
        transform.Find("BattleRes/UserName").GetComponent<Text>().text = userData.Username;
        transform.Find("BattleRes/TotalCount").GetComponent<Text>().text = "TotalCount:\n"+userData.TotalCount.ToString();
        transform.Find("BattleRes/WinCount").GetComponent<Text>().text = "WinCount:\n"+userData.WinCount.ToString();
    }

    /// <summary>
    /// 载入房间（异步）
    /// </summary>
    /// <param name="userDataList"></param>
    public void LoadRoomItemAsync(List<UserData> userDataList)
    {
        this.userDataList = userDataList;
    }

    /// <summary>
    /// 载入房间
    /// </summary>
    /// <param name="count"></param>
    private void LoadRoomItem(List<UserData> userDataList)
    {
        RoomItem[] roomItemArray = roomLayout.GetComponentsInChildren<RoomItem>();
        foreach(RoomItem roomItemTemp in roomItemArray)
        {
            roomItemTemp.DestroySelf();
        }
        //在加载房间列表的时候，先销毁以前所有的房间列表

        int count = userDataList.Count;
        for(int i = 0; i < count; i++)
        {
            GameObject roomItem = GameObject.Instantiate(roomItemPrefab);
            roomItem.transform.SetParent(roomLayout.transform);

            UserData userData = userDataList[i];
            roomItem.GetComponent<RoomItem>().SetRoomInfo(userData.Username, userData.TotalCount, userData.WinCount,userData.Id,this);
        }
        int roomCount = GetComponentsInChildren<RoomItem>().Length;
        Vector2 size = roomLayout.GetComponent<RectTransform>().sizeDelta;
        roomLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, roomCount * (roomItemPrefab.GetComponent<RectTransform>().sizeDelta.y+roomLayout.spacing));
    }

    /// <summary>
    /// 给加入房间按钮添加一个触发事件
    /// </summary>
    /// <param name="id"></param>
    public void OnJoinClick(int id)
    {
        joinRoomRequest.SendRequest(id);
    }

    /// <summary>
    /// 该界面对请求加入房间的处理
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="userData1"></param>
    /// <param name="userData2"></param>
    public void OnJoinResponse(ReturnCode returnCode,UserData userData1,UserData userData2)
    {
        switch (returnCode)
        {
            case ReturnCode.NotFound:
                uiManager.ShowMessageAsync("房间不存在！无法加入！");
                break;
            case ReturnCode.Fail:
                uiManager.ShowMessageAsync("房间已满!无法加入！");
                break;
            case ReturnCode.Success:
                this.userData1 = userData1;
                this.userData2 = userData2;
                break;
        }
    }
    
    /// <summary>
    /// 客户端针对战绩信息更新处理
    /// </summary>
    public void OnUpdateResultResponse(int totalCount,int winCount)
    {
        facade.UpdateResult(totalCount, winCount);
        SetBattleRes();
        //需要改为异步进行
        //TODO
    }
}
