using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Common;

public class RegisterPanel : BasePanel {

    private InputField usernameIF;
    private InputField passwordIF;
    private InputField rePasswordIF;

    private RegisterRequest registerRequest;

    private void Start()
    {
        registerRequest = GetComponent<RegisterRequest>();

        usernameIF = transform.Find("UserNameInputField").GetComponent<InputField>();
        passwordIF = transform.Find("PasswordInputField").GetComponent<InputField>();
        rePasswordIF = transform.Find("ConfirmPasswordInputField").GetComponent<InputField>();

        transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick);
        transform.Find("RegisterButton").GetComponent<Button>().onClick.AddListener(OnRegisterClick);
    }

    /// <summary>
    /// 重写开始界面方法
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        gameObject.SetActive(true);

        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.2f);

        transform.localPosition = new Vector3(1000, 0, 0);
        transform.DOLocalMove(Vector3.zero, 0.2f);
    }

    /// <summary>
    /// 重写离开界面方法
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 给注册界面的关闭按钮添加一个触发事件
    /// </summary>
    private void OnCloseClick()
    {
        PlayClickSound();
        transform.DOScale(0, 0.5f);
        Tweener tweener = transform.DOLocalMove(new Vector3(1000, 0, 0), 0.2f);
        tweener.OnComplete(() => uiManager.PopPanel());
    }

    /// <summary>
    /// 给注册界面的注册按钮添加一个触发事件
    /// </summary>
    private void OnRegisterClick()
    {
        PlayClickSound();
        string message = "";

        if (string.IsNullOrEmpty(usernameIF.text))
        {
            message += "用户名不能为空";
        }
        if (string.IsNullOrEmpty(passwordIF.text))
        {
            message += "\n密码不能为空";
        }
        if (passwordIF.text != rePasswordIF.text)
        {
            message += "\n密码设置不一致";
        }
        if (message != "")
        {
            uiManager.ShowMessage(message);
        }
        else
        {
            registerRequest.SendRequest(usernameIF.text, passwordIF.text);
            //发送到服务器端进行注册
        }
    }

    /// <summary>
    /// 处理判断是否注册成功的方法
    /// </summary>
    /// <param name="returnCode"></param>
    public void OnRegisterResponse(ReturnCode returnCode)
    {
        if(returnCode == ReturnCode.Success)
        {
            uiManager.ShowMessageAsync("注册成功");
        }
        else
        {
            uiManager.ShowMessageAsync("注册失败，用户名重复");
        }
    }
}
