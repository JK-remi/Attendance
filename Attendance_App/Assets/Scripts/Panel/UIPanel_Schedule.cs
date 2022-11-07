using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel_Schedule : UIPanel
{
    [SerializeField]
    private Text year;
    [SerializeField]
    private Text month;

    [SerializeField]
    private List<DayPreview> dayList;

    [SerializeField]
    private Color todayColor;

    private System.DateTime curDT;

    private void Awake()
    {
        uiType = UIManager.UI_type.UI_SCHEDULER;
    }

    protected override void Init()
    {
        base.Init();

        SetMonth(System.DateTime.Now);
    }

    public void SetMonth(System.DateTime dt)
    {
        curDT = dt;

        year.text = dt.Year.ToString();
        month.text = ((Utils.eMonth)dt.Month).ToString();

        System.DateTime date = new System.DateTime(dt.Year, dt.Month, 1);
        int lastDay = System.DateTime.DaysInMonth(dt.Year, dt.Month);

        int dayCount = 0;
        for (int i = 0; i < dayList.Count; i++)
        {
            if((dayCount == 0 && i < (int)date.DayOfWeek)
                || dayCount >= lastDay)
            {
                dayList[i].Init(date, string.Empty, false);
                continue;
            }

            System.DateTime settingDT = date.AddDays(dayCount++);
            dayList[i].Init(settingDT, CreateDayPreview(settingDT), true);
            if(Utils.GetSpanDays(settingDT, System.DateTime.Now) == 0)
            {
                dayList[i].ChangeColor(todayColor);
            }
        }
    }

    private string CreateDayPreview(System.DateTime dt)
    {
        List<StudyTimeInfo> studyList = DBManager.Instance.GetStudyList(dt);
        string str = "";

        for(int i=0; i<studyList.Count; i++)
        {
            str += studyList[i].Name;
            if (i == studyList.Count - 1)
            {
                break;
            }

            str += "\n";
        }

        return str;
    }

    public void ChangeMonth(int addMonth)
    {
        System.DateTime changeDT = curDT.AddMonths(addMonth);
        SetMonth(changeDT);
    }

    public void OpenDayPreview(DayPreview preview)
    {
        if(isClicked)
        {
            return;
        }
        isClicked = true;

        if (UIManager.Instance.OpenUI(UIManager.UI_type.UI_DAY_INFO, true))
        {
            ((UIPanel_StudyDayInfo)UIManager.Instance.CurUI).SetDayInfo(preview.DT);
            isClicked = false;
        }
    }
}
