using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class MoveRequest : BaseRequest {

    public Transform localPlayerTransform;
    public PlayerMove localPlayerMove;
    
    private int syncRate = 30;

    public Transform remotePlayerTransform;
    //需要同步的角色
    public Animator remotePlayerAnim;
    //通过animator设置forward

    private bool isSyncRemotePlayer = false;
    private Vector3 position;
    private Vector3 rotation;
    private float forward;

    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Move;

        base.Awake();
    }

    public void Start()
    {
        InvokeRepeating("SyncLocalPlayer", 2.0f, 1.0f / syncRate);
    }

    public void FixedUpdate()
    {
        if (isSyncRemotePlayer)
        {
            SyncRemotePlayer();
            isSyncRemotePlayer = false;
        }
    }

    /// <summary>
    /// 将客户端的角色的各个位置信息发送给服务器端
    /// </summary>
    /// <param name="positionX"></param>
    /// <param name="positionY"></param>
    /// <param name="positionZ"></param>
    /// <param name="rotationX"></param>
    /// <param name="rotationY"></param>
    /// <param name="rotationZ"></param>
    /// <param name="forword"></param>
    public void SendRequest(float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ,float forward)
    {
        string data = string.Format("{0},{1},{2}|{3},{4},{5}|{6}", positionX, positionY, positionZ, rotationX, rotationY, rotationZ, forward);
        base.SendRequest(data);
        //发送
    }

    public override void OnResponse(string data)
    {
        string[] strs = data.Split('|');
        position = UnityTools.Parse(strs[0]);
        rotation = UnityTools.Parse(strs[1]);
        forward = float.Parse(strs[2]);

        isSyncRemotePlayer = true;
    }

    /// <summary>
    /// 同步当前玩家的位置动作信息，通过这个发送给服务器端完成在其他客户端上的同步
    /// </summary>
    public void SyncLocalPlayer()
    {
        SendRequest(localPlayerTransform.position.x, localPlayerTransform.position.y, localPlayerTransform.position.z, localPlayerTransform.eulerAngles.x, localPlayerTransform.eulerAngles.y, localPlayerTransform.eulerAngles.z, localPlayerMove.forward);
    } 

    /// <summary>
    /// 
    /// </summary>
    public void SyncRemotePlayer()
    {
        remotePlayerTransform.position = position;
        remotePlayerTransform.eulerAngles = rotation;
        remotePlayerAnim.SetFloat("Forward", forward);
    }

    /// <summary>
    /// 设置当前对象的transform组件和Animator组件
    /// </summary>
    /// <param name="localPlayerTransform"></param>
    /// <param name="localPlayerMove"></param>
    public MoveRequest SetLocalPlayer(Transform localPlayerTransform, PlayerMove localPlayerMove)
    {
        this.localPlayerTransform = localPlayerTransform;
        this.localPlayerMove = localPlayerMove;

        return this;
    }
    
    /// <summary>
    /// 设置需要同步的对象transform组件和Animator组件
    /// </summary>
    /// <param name="remotePlayerTransform"></param>
    public MoveRequest SetRemotePlayer(Transform remotePlayerTransform)
    {
        this.remotePlayerTransform = remotePlayerTransform;
        this.remotePlayerAnim = remotePlayerTransform.GetComponent<Animator>();

        return this;
    }
}
