using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterRequest : BaseRequest {

    private RegisterPanel registerPanel;

    public override void Awake()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Register;
        registerPanel = GetComponent<RegisterPanel>();
        base.Awake();
    }
    /// <summary>
    /// 发送请求，通过GameFacade来处理,经用户名和密码和确认密码用','组拼
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="rePassword"></param>
    public void SendRequest(string username,string password)
    {
        string data = username + "," + password ;
        base.SendRequest(data);
    }
    /// <summary>
    /// 得到响应
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        ReturnCode returnCode = (ReturnCode)int.Parse(data);
        registerPanel.OnRegisterResponse(returnCode);
    }
}
