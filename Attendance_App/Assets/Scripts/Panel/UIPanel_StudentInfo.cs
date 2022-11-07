using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Events;

public class UIPanel_StudentInfo : UIPanel
{
    private StudentInfo target;
    public StudentInfo Target
    {
        get { return target; }
    }

    [SerializeField]
    private SpriteAtlas atlas;

    [SerializeField]
    private Image icon;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text phoneText;
    [SerializeField]
    private Text startText;
    [SerializeField]
    private Text daysText;
    [SerializeField]
    private Text supplementText;
    [SerializeField]
    private RectTransform contentTrans;
    [SerializeField]
    private GameObject supplementBtn;
    [SerializeField]
    private Text studyInfo;

    private string originTitle;

    private void Awake()
    {
        uiType = UIManager.UI_type.UI_STUDENT_INFO;
        originTitle = Title;
    }

    protected override void Init()
    {
        base.Init();
        target = null;
    }

    public void SetStudentInfo(ulong id)
    {
        StudentInfo info = DBManager.Instance.Find(id);
        if (info == null)
        {
            return;
        }

        icon.sprite = atlas.GetSprite(info.sprite);

        nameText.text = info.name;
        phoneText.text = info.phoneNumber;

        startText.text = info.startDT.ToString("yyyy-MMM-dd") + "일";
        daysText.text = Utils.GetSpanDays(System.DateTime.Now, info.startDT).ToString() + " days";
        studyInfo.text = info.GetStudyDayInfo();
        studyInfo.text += info.GetSupDayInfo();

        // scroll view의 content 크기 조정
        string[] str = studyInfo.text.Split('\n');
        if(str != null)
        {
            contentTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, str.Length * studyInfo.fontSize + studyInfo.fontSize / 2);
        }
        
        if (info.supplementTime == null || info.supplementTime.Count == 0)
        {
            supplementText.text = "0 회";
            supplementBtn.SetActive(false);
        }
        else
        {
            supplementText.text = info.supplementTime.Count.ToString() + " 회";
            supplementBtn.SetActive(true);
        }

        ModifyTitle(info.name);
        target = StudentInfo.Copy(info);
    }

    private void ModifyTitle(string name)
    {
        strTitle = '\'' + name + '\'' + originTitle;
        UIManager.Instance.SetTitle(Title);
    }

    public void OpenSupplement()
    {
        if(target.GetSupplementCount() == 0)
        {
            return;
        }

        if (isClicked)
        {
            return;
        }

        isClicked = true;
        if (UIManager.Instance.OpenUI(UIManager.UI_type.UI_MOD_SUPPLEMENT, true))
        {
            ((UIPanel_ModifySupplement)UIManager.Instance.CurUI).SetStudentInfo(this);
            isClicked = false;
        }
    }

    public void ModifySupplement(Dictionary<System.DateTime, System.DateTime> supList)
    {
        target.supplementTime = supList;
        DBManager.Instance.Modify(target);
    }

    public void OK()
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
