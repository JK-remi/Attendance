using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel_Main : UIPanel
{
    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject menu;

    private void Awake()
    {
        uiType = UIManager.UI_type.UI_MAIN;
    }

    protected override void Init()
    {
        base.Init();

        title.SetActive(true);
        menu.SetActive(false);
    }

    public void ActiveMenu()
    {
        UIManager.Instance.OpenTitle();
        UIManager.Instance.SetTitle(Title);

        title.SetActive(false);
        menu.SetActive(true);
    }

    public void OpenSchedule()
    {
        if(isClicked)
        {
            return;
        }

        isClicked = true;
        if(UIManager.Instance.OpenUI(UIManager.UI_type.UI_SCHEDULER, true))
        {
            menu.SetActive(false);
            isClicked = false;
        }
    }

    public void OpenToday()
    {
        if (isClicked)
        {
            return;
        }

        isClicked = true;
        if(UIManager.Instance.OpenUI(UIManager.UI_type.UI_DAY_INFO, true))
        {
            ((UIPanel_StudyDayInfo)UIManager.Instance.CurUI).SetDayInfo(System.DateTime.Now);

            menu.SetActive(false);
            isClicked = false;
        }
    }

    public void OpenStudentList()
    {
        if (isClicked)
        {
            return;
        }

        isClicked = true;
        if(UIManager.Instance.OpenUI(UIManager.UI_type.UI_ATTENDANCE, true))
        {
            menu.SetActive(false);
            isClicked = false;
        }
    }
}
