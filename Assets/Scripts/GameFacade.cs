using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFacade : MonoBehaviour
{

    #region 单例模式
    /// <summary>
    /// 单例模式
    /// 目的：通过单例模式来调用GameFacade,从而使其他Manager通过GameFacade作为中介来进行交互，降低耦合度
    /// </summary>
    private static GameFacade _instance;
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }
    public static GameFacade Instance
    { get
        {
            if (_instance == null)
            {
                //_instance = GameObject.Find("GameFacade").GetComponent<GameFacade>();
				GameObject Object=GameObject.Find("GameFacade");
				if(Object==null){
					return null;
				}
				_instance=Object.GetComponent<GameFacade>();
            }
            return _instance;
        }
    }
    #endregion

    private UIManager uiManager;
    private AudioManager audioManager;
    private PlayerManager playerManager;
    private CameraManager cameraManager;
    private RequestManager requestManager;
    private ClientManager clientManager;

    private bool isEnterPlaying = false;

    // Use this for initialization
    void Start()
    {
        InitManager();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateManager();
        if (isEnterPlaying)
        {
            EnterPlaying();
            isEnterPlaying = false;
        }
    }

    /// <summary>
    /// 更新，主要用以异步线程处理
    /// </summary>
    public void UpdateManager()
    {
        uiManager.Update();
        audioManager.Update();
        playerManager.Update();
        cameraManager.Update();
        requestManager.Update();
        clientManager.Update();
    }

    /// <summary>
    /// 构造和初始化各个Manager
    /// </summary>
    private void InitManager()
    {
        uiManager = new UIManager(this);
        audioManager = new AudioManager(this);
        playerManager = new PlayerManager(this);
        cameraManager = new CameraManager(this);
        requestManager = new RequestManager(this);
        clientManager = new ClientManager(this);

        uiManager.OnInit();
        audioManager.OnInit();
        playerManager.OnInit();
        cameraManager.OnInit();
        requestManager.OnInit();
        clientManager.OnInit();
    }
    
    /// <summary>
    /// 各个Manager的销毁
    /// </summary>
    private void DestroyManager()
    {
        uiManager.OnDestroy();
        audioManager.OnDestroy();
        playerManager.OnDestroy();
        cameraManager.OnDestroy();
        requestManager.OnDestroy();
        clientManager.OnDestroy();
    }

    /// <summary>
    /// 在GameFacade销毁的时候，销毁各个Manager
    /// </summary>
    private void OnDestroy()
    {
        DestroyManager();
    }
    /// <summary>
    /// 通过单例模式来调用AddRequest将actionCode及对应的baseRequest一同添加到requestDic中
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="baseRequest"></param>
    public void AddRequest(ActionCode actionCode, BaseRequest baseRequest)
    {
        requestManager.AddRequest(actionCode, baseRequest);
    }

    /// <summary>
    /// 通过单例模式来调用AddRequest将actionCode及对应的baseRequest从requestDic中移除
    /// </summary>
    /// <param name="requestCode"></param>
    public void RemoveRequest(ActionCode actionCode)
    {
        requestManager.RemoveRequest(actionCode);
    }

    /// <summary>
    /// 通过单例模式的GameFacade来调用RequestManager中的HandleResponse
    /// 即对服务器端发送过来的命令进行响应
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    public void HandleResponse(ActionCode actionCode, string data)
    {
        requestManager.HandleResponse(actionCode, data);
    }

    /// <summary>
    /// 在别的模块显示提示信息面板
    /// </summary>
    /// <param name="message"></param>
    public void ShowMessage(string message)
    {
        uiManager.ShowMessage(message);
    }

    /// <summary>
    /// 发送请求方法,GameFacade作为中介
    /// </summary>
    /// <param name="requestCode"></param>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        Debug.Log("unity客户端向服务器端发送请求...");
        clientManager.SendRequest(requestCode, actionCode, data);
    }

    /// <summary>
    /// 播放背景音乐方法，通过外部facade调用
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayBgSound(string soundName)
    {
        audioManager.PlayBgSound(soundName);
    }

    /// <summary>
    /// 播放游戏其他音乐，通过外部facade调用调用
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayNormalSound(string soundName)
    {
        audioManager.PlayNormalSound(soundName);
    }

    /// <summary>
    /// 定义一个中介方法，能够使GameFacade设置UserData
    /// </summary>
    /// <param name="userData"></param>
    public void SetUserData(UserData userData)
    {
        playerManager.UserData = userData;
    }

    /// <summary>
    /// 定义一个得到UserData的方法，以供外界访问需要
    /// </summary>
    /// <returns></returns>
    public UserData GetUserData()
    {
        return playerManager.UserData;
    }

    /// <summary>
    /// 设置当前的角色类型
    /// </summary>
    /// <param name="roleType"></param>
    public void SetCurrentRoleType(RoleType roleType)
    {
        playerManager.SetCurrentRoleType(roleType);
    }

    /// <summary>
    /// 得到当前游戏物体
    /// </summary>
    public GameObject GetCurrentRoleGameObject()
    {
        return playerManager.GetCurrentRoleGameObject();
    }

    /// <summary>
    /// 进入游戏开始游玩
    /// 生成角色以及处理相机的跟随
    /// </summary>
    public void EnterPlaying()
    {
        playerManager.SpawnRoles();
        cameraManager.FollowRole();
    }

    /// <summary>
    /// 进入游戏开始游玩
    /// 生成角色以及处理相机的跟随( 异步 )
    /// </summary>
    public void EnterPlayingAsync()
    {
        isEnterPlaying = true;
    }

    /// <summary>
    /// 在facade中添加一个StartPlaying方法,其与EnterPlaying有相差3秒的间隔，即进入游戏3秒后可以开始控制角色进行游玩
    /// </summary>
    public void StartPlaying()
    {
        playerManager.AddControlScripts();
        playerManager.CreateSyncRequest();
    }

    /// <summary>
    /// 发送伤害
    /// </summary>
    /// <param name="damage"></param>
    public void SendAttack(int damage)
    {
        playerManager.SendAttack(damage);
    }

    /// <summary>
    /// 游戏结束后的相关处理，视野处理和界面切换
    /// </summary>
    public void GameOver()
    {
        cameraManager.WalkThroughScene();
        playerManager.GameOver();
    }

    /// <summary>
    /// 在facade中定义一个与playerManager中一样的更新战绩方法，以便调用
    /// </summary>
    /// <param name="totalCount"></param>
    /// <param name="winCount"></param>
    public void UpdateResult(int totalCount,int winCount)
    {
        playerManager.UpdateResult(totalCount, winCount);
    }
}