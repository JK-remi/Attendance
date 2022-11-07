using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIPanel_OK_Cancel : UIPanel
{
    [SerializeField]
    private Text notice;
    private UnityAction action;

    private void Awake()
    {
        uiType = UIManager.UI_type.UI_OK_CANCEL;
    }

    protected override void Init()
    {
        base.Init();

        action = null;
    }

    public void SetNotice(string text, UnityAction okAction)
    {
        action = okAction;
        notice.text = text;
    }

    public void OK()
    {
        if(isClicked)
        {
            return;
        }

        isClicked = true;
        if(action != null)
        {
            action.Invoke();
        }
        if(UIManager.Instance.CloseUI())
        {
            isClicked = false;
        }
    }

    public void Quit()
    {
        if(isClicked)
        {
            return;
        }

        isClicked = true;
        if(UIManager.Instance.CloseUI())
        {
            isClicked = false;
        }
    }
}
