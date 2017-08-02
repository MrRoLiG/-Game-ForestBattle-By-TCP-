using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListRoomRequest : BaseRequest {

    private RoomListPanel roomListPanel;

    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.ListRoom;

        roomListPanel = GetComponent<RoomListPanel>();

        base.Awake();
    }


    public override void SendRequest()
    {
        base.SendRequest("r");
    }


    public override void OnResponse(string data)
    {
        List<UserData> userDataList = new List<UserData>();
        if (data != "0")
        {
            string[] roomArray = data.Split('|');
            foreach (string roomTemp in roomArray)
            {
                string[] strs = roomTemp.Split(',');
                UserData userData = new UserData(int.Parse(strs[0]), strs[1], int.Parse(strs[2]), int.Parse(strs[3]));
                userDataList.Add(userData);
            }
        }
        roomListPanel.LoadRoomItemAsync(userDataList);
    }
}
