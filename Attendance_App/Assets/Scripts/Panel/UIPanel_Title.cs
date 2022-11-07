using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel_Title : UIPanel
{
    [SerializeField]
    private Text title;

    [SerializeField]
    private GameObject backBtn;
    private bool bBackClicked = false;

    void Awake()
    {
        uiType = UIManager.UI_type.UI_TITLE;
    }

    protected override void Init()
    {
        base.Init();

        backBtn.SetActive(false);
    }

    public void ChangeTitle(string text)
    {
        if(title == null)
        {
            return;
        }

        title.text = text;
    }

    public void OpenSetting()
    {
        return;

        if (UIManager.Instance.IsOpen(UIManager.UI_type.UI_SETTING))
        {
            return;
        }

        UIManager.Instance.OpenUI(UIManager.UI_type.UI_SETTING, true);
    }
    
    public void GoBack()
    {
        // notice 관련 ui가 열려있는 경우에는 뒤로가기 막음 (공지의 전달력 위해서)
        if(UIManager.Instance.CurUI.UiType == UIManager.UI_type.UI_OK_CANCEL
            || UIManager.Instance.CurUI.UiType == UIManager.UI_type.UI_NOTICE)
        {
            return;
        }

        if(bBackClicked)
        {
            return;
        }

        bBackClicked = true;
        if(UIManager.Instance.CloseUI())
        {
            bBackClicked = false;
        }

    }

    public void ActiveBackButton(bool bActive)
    {
        if(backBtn == null)
        {
            return;
        }

        bBackClicked = false;
        backBtn.SetActive(bActive);
    }
}
