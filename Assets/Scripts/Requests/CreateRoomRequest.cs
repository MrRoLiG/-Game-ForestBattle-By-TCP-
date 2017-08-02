using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoomRequest : BaseRequest {

    private RoomPanel roomPanel;

    /// <summary>
    /// 重写Awake方法进行初始化
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.CreateRoom;
        
        base.Awake();
    }


    public void SetPanel(BasePanel basePanel)
    {
        roomPanel = basePanel as RoomPanel;
    }

    public override void SendRequest()
    {
        base.SendRequest("r");
    }

    /// <summary>
    /// 响应从服务器端过来的创建房间的命令
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        RoleType roleType = (RoleType)int.Parse(strs[1]);
        facade.SetCurrentRoleType(roleType);
        if (returnCode == ReturnCode.Success)
        {
            roomPanel.SetLocalPlayerResAsync();
        }
    }
}
