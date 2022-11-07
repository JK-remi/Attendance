using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    protected UIManager.UI_type uiType;
    public UIManager.UI_type UiType
    {
        get
        {
            return uiType;
        }
    }

    [SerializeField]
    protected string strTitle;
    public string Title
    {
        get { return strTitle; }
    }


    protected bool isClicked = false;

    public bool IsOpen()
    {
        return this.gameObject.activeSelf;
    }

    public void UI_OnOff(bool bOn)
    {
        if(bOn)
        {
            Init();
        }
        else
        {
            Exit();
        }
    }

    virtual protected void Init()
    {
        this.gameObject.SetActive(true);
        UIManager.Instance.SetTitle(Title);
        isClicked = false;
    }

    virtual protected void Exit()
    {
        this.gameObject.SetActive(false);
    }
}
