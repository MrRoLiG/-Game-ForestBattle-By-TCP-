using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class UpdateRoomRequest : BaseRequest {

    private RoomPanel roomPanel;

    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.UpdateRoom;
        roomPanel = GetComponent<RoomPanel>();

        base.Awake();
    }


    public override void OnResponse(string data)
    {
        //base.OnResponse(data);
        

        UserData userData1 = null;
        UserData userData2 = null;
        string[] userDataArray = data.Split('|');
        userData1 = new UserData(userDataArray[0]);
        if (userDataArray.Length > 1)
        {
            userData2 = new UserData(userDataArray[1]);
        }
        roomPanel.SetAllPlayerResAsync(userData1, userData2);
    }
}
