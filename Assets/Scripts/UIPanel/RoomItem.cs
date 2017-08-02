using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour {

    public Text userName;
    public Text totalCount;
    public Text winCount;
    public Button joinButton;

    private int id;

    private RoomListPanel roomListPanel;

	// Use this for initialization
	void Start () {
        if (joinButton != null)
        {
            joinButton.onClick.AddListener(OnJoinClick);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 在外界设置房间信息
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="totalCount"></param>
    /// <param name="winCount"></param>
    public void SetRoomInfo(string userName,int totalCount,int winCount,int id,RoomListPanel roomListPanel)
    {
        SetRoomInfo(userName, totalCount.ToString(), winCount.ToString(), id, roomListPanel);
    }
    public void SetRoomInfo(string userName, string totalCount, string winCount,int id,RoomListPanel roomListPanel)
    {
        this.userName.text = userName;
        this.totalCount.text = "TotalCount:\n" + totalCount;
        this.winCount.text = "WinCount:\n" + winCount;
        this.id = id;
        this.roomListPanel = roomListPanel;
    }

    /// <summary>
    /// 给加入按钮添加一个触发事件
    /// </summary>
    private void OnJoinClick()
    {
        roomListPanel.OnJoinClick(id);
    }


    public void DestroySelf()
    {
        GameObject.Destroy(this.gameObject);
    }
}
