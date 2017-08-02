using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomRequest : BaseRequest {

    private RoomListPanel roomListPanel;

    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.JoinRoom;

        roomListPanel = GetComponent<RoomListPanel>();

        base.Awake();
    }

    public void SendRequest(int id)
    {
        base.SendRequest(id.ToString());
    }


    public override void OnResponse(string data)
    {
        //base.OnResponse(data);

        string[] strs = data.Split('-');
        string[] strs2 = strs[0].Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs2[0]);

        UserData userData1 = null;
        UserData userData2 = null;

        if (returnCode == ReturnCode.Success)
        {
            string[] userDataArray = strs[1].Split('|');
            userData1 = new UserData(userDataArray[0]);
            userData2 = new UserData(userDataArray[1]);

            RoleType roleType = (RoleType)int.Parse(strs2[1]);
            facade.SetCurrentRoleType(roleType);
        }
        roomListPanel.OnJoinResponse(returnCode, userData1, userData2);

    }
}
