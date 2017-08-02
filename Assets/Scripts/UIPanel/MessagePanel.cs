using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : BasePanel {

    private Text text;
    private float MessageShowTime = 1;
    //定义消息面板的显示时间
    private string messages = null;


    private void Update()
    {
        if (messages != null)
        {
            ShowMessage(messages);
            messages = null;
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        text = GetComponent<Text>();

        text.enabled = false;
        //默认不显示

        uiManager.InitMessagePanel(this);
    }
    /// <summary>
    /// 显示方法，当有信息需要显示的时候调用
    /// </summary>
    /// <param name="message"></param>
    public void ShowMessage(string message)
    {
        text.CrossFadeAlpha(1, 0.2f, false);

        text.text = message;

        text.enabled = true;

        Invoke("Hide", MessageShowTime);
    }

    public void ShowMessageAsync(string message)
    {
        messages = message;
    }

    /// <summary>
    /// 消息面板的隐藏方法，即消息面板出来一段时间后自动消失
    /// </summary>
    private void Hide()
    {
        text.CrossFadeAlpha(0, 1, false);
    }
}
