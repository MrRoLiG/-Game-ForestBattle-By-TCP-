using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class LoginRequest : BaseRequest {

    private LoginPanel loginPanel;

    public override void Awake()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Login;
        loginPanel = GetComponent<LoginPanel>();
        base.Awake();
    }

    /// <summary>
    /// 发送请求，通过GameFacade来处理,经用户名和密码用','组拼
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public void SendRequest(string username,string password)
    {
        string data = username + "," + password;
        base.SendRequest(data);
    }
    /// <summary>
    /// 客户端得到响应
    /// </summary>
    /// <param name="data">接收到的从服务器端发来的数据</param>
    /// 数据格式：((int)ReturnCode.Success).ToString(), user.Username, result.TotalCount, result.WinCount
    /// 状态码,用户名,总局数,赢局数
    public override void OnResponse(string data)
    {
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        loginPanel.OnLoginResponse(returnCode);
        if (returnCode == ReturnCode.Success)
        {
            string userName = strs[1];
            int totalCount = int.Parse(strs[2]);
            int winCount = int.Parse(strs[3]);

            UserData userData = new UserData(userName, totalCount, winCount);
            facade.SetUserData(userData);
        }
    }
}
