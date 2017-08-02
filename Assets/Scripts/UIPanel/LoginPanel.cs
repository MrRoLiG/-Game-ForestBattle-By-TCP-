using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Common;

public class LoginPanel : BasePanel {

    private Button closeButton;

    private InputField usernameIF;
    private InputField passwordIF;

    private LoginRequest loginRequest;

    public void Start()
    {
        usernameIF = transform.Find("UserNameInputField").GetComponent<InputField>();
        passwordIF = transform.Find("PasswordInputField").GetComponent<InputField>();
        closeButton = transform.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(OnCloseClick);

        transform.Find("LoginButton").GetComponent<Button>().onClick.AddListener(OnLoginClick);
        transform.Find("RegisterButton").GetComponent<Button>().onClick.AddListener(OnRegisterClick);

        loginRequest = GetComponent<LoginRequest>();
    }

    /// <summary>
    /// 重写界面显示方法
    /// </summary>
    public override void OnEnter()
    {
        EnterAnim();
    }
    /// <summary>
    /// 重写界面暂停方法
    /// </summary>
    public override void OnPause()
    {
        HideAnim();
    }
    /// <summary>
    /// 重写界面继续方法
    /// </summary>
    public override void OnResume()
    {
        EnterAnim();
    }
    /// <summary>
    /// 重写离开界面方法
    /// </summary>
    public override void OnExit()
    {
        HideAnim();
    }

    /// <summary>
    /// 给登陆界面的关闭按钮添加一个点击事件
    /// </summary>
    private void OnCloseClick()
    {
        PlayClickSound();
        uiManager.PopPanel();
    }

    /// <summary>
    /// 给登陆按钮添加一个点击事件
    /// </summary>
    private void OnLoginClick()
    {
        string message = "";
        if (string.IsNullOrEmpty(usernameIF.text))
        {
            message += "用户名不能为空！";
        }
        if (string.IsNullOrEmpty(passwordIF.text))
        {
            message += "密码不能为空！";
        }
        if (message != "")
        {
            uiManager.ShowMessage(message);
            return;
        }
        else
        {
            loginRequest.SendRequest(usernameIF.text, passwordIF.text);
            //发送到服务器端进行注册
        }
    }

    /// <summary>
    /// 处理判断是否登陆成功的方法
    /// </summary>
    /// <param name="returnCode"></param>
    public void OnLoginResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            Debug.Log("登陆成功");
            uiManager.ShowMessageAsync("登陆成功！");
            uiManager.PushPanelAsync(UIPanelType.RoomList);
        }
        else
        {
            uiManager.ShowMessageAsync("用户名或密码错误，无法登陆！请重新输入！！");
        }
    }

    /// <summary>
    /// 给注册按钮添加一个点击事件
    /// </summary>
    private void OnRegisterClick()
    {
        PlayClickSound();
        uiManager.PushPanel(UIPanelType.Register);
    }

    /// <summary>
    /// 界面初始化动画播放方法
    /// </summary>
    private void EnterAnim()
    {
        gameObject.SetActive(true);

        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.2f);

        transform.localPosition = new Vector3(1000, 0, 0);
        transform.DOLocalMove(Vector3.zero, 0.2f);

    }

    /// <summary>
    /// 界面离开动画播放方法
    /// </summary>
    private void HideAnim()
    {
        transform.DOScale(0, 0.5f);
        
        transform.DOLocalMoveX(1000, 0.5f).OnComplete(()=>gameObject.SetActive(false));
    }
}
