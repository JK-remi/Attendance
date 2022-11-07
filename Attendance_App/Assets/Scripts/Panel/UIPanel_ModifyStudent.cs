using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Events;

public class UIPanel_ModifyStudent : UIPanel
{
    private StudentInfo target;

    private string originTitle;

    [SerializeField]
    private SpriteAtlas atlas;
    private const string defaultSprite = "a1";

    [SerializeField]
    private Image icon;
    [SerializeField]
    private TMPro.TMP_InputField nameText;
    [SerializeField]
    private TMPro.TMP_InputField phoneText;
    [SerializeField]
    private Text studyInfo;

    private UnityAction eventAction;    // add or modify 시 attendance 정보 변경 method 호출

    private void Awake()
    {
        uiType = UIManager.UI_type.UI_MOD_STUDENT_INFO;
        originTitle = Title;
    }

    protected override void Init()
    {
        base.Init();
        target = null;
        eventAction = null;
    }

    public void SetStudentInfo(ulong id, UnityAction action)
    {
        StudentInfo info = DBManager.Instance.Find(id);

        if (info == null)
        {
            target = new StudentInfo();

            icon.sprite = atlas.GetSprite(defaultSprite);

            nameText.text = string.Empty;
            ModifyTitle(string.Empty);

            phoneText.text = string.Empty;
            studyInfo.text = string.Empty;

            target.startDT = System.DateTime.Now;
            target.id = DBManager.Instance.MakeID(System.DateTime.Now);
            target.sprite = defaultSprite;
        }
        else
        {
            icon.sprite = atlas.GetSprite(info.sprite);

            nameText.text = info.name;
            ModifyTitle(info.name);

            phoneText.text = info.phoneNumber;
            studyInfo.text = info.GetStudyDayInfo();
            
            target = StudentInfo.Copy(info);
        }

        eventAction = action;
    }

    private void ModifyTitle(string name)
    {
        if(name == string.Empty)
        {
            strTitle = originTitle;
            return;
        }

        string startStr = "볆";
        string descStr = " 설정";
        strTitle = startStr + name + startStr + descStr;

        UIManager.Instance.SetTitle(Title);
    }

    public void OpenIconSetting()
    {
        if (isClicked)
        {
            return;
        }

        isClicked = true;

        PlayerPrefs.SetString(UIManager.ICON_NAME, Utils.SubClone(icon.sprite.name));
        if(UIManager.Instance.OpenUI(UIManager.UI_type.UI_SELECT_ICON, true))
        {
            ((UIPanel_SelectIcon)UIManager.Instance.CurUI).SetAction(ChangeIcon);
            isClicked = false;
        }
    }

    public void ChangeIcon()
    {
        target.sprite = PlayerPrefs.GetString(UIManager.ICON_NAME);

        Sprite s = atlas.GetSprite(target.sprite);
        icon.sprite = s;
    }

    public void ChangeName()
    {
        if(target == null)
        {
            return;
        }

        target.name = nameText.text;
    }

    public void ChangePhone()
    {
        if(target == null)
        {
            return;
        }

        target.phoneNumber = phoneText.text;
    }

    public void OpenStudyTime()
    {
        UIManager.Instance.OpenUI(UIManager.UI_type.UI_MOD_STUDY_TIME, true);
        ((UIPanel_ModifyStudyTime)UIManager.Instance.CurUI).SetStudentInfo(target, this);
    }

    public void ModifyStudyTime(List<System.DateTime> timeList)
    {
        target.studyTime = timeList;
        studyInfo.text = target.GetStudyDayInfo();
    }

    public void Save()
    {
        if (isClicked)
        {
            return;
        }

        isClicked = true;

        DBManager.Instance.Modify(target);
        if (eventAction != null)
        {
            eventAction.Invoke();
        }

        if(UIManager.Instance.CloseUI())
        {
            isClicked = false;
        }
    }

    public void Quit()
    {
        if (isClicked)
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
