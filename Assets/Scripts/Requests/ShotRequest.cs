using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class ShotRequest : BaseRequest {

    public PlayerManager playerManager;

    private bool isShot = false;

    private RoleType roleType;
    private Vector3 position;
    private Vector3 rotation;

    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Shot;

        base.Awake();
    }

    public void Update()
    {
        if (isShot)
        {
            playerManager.RemoteShot(roleType, position, rotation);
            isShot = false;
        }
    }


    public void SendRequest(RoleType roleType,Vector3 position,Vector3 rotation)
    {
        string data = string.Format("{0}|{1},{2},{3}|{4},{5},{6}", (int)roleType, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z);

        base.SendRequest(data);
    }

    public override void OnResponse(string data)
    {
        string[] strs = data.Split('|');
        RoleType roleType = (RoleType)int.Parse(strs[0]);
        Vector3 position = UnityTools.Parse(strs[1]);
        Vector3 rotation = UnityTools.Parse(strs[2]);

        isShot = true;
        this.roleType = roleType;
        this.position = position;
        this.rotation = rotation;
    }
}
