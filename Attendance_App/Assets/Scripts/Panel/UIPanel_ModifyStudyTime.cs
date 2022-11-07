using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Events;

public class UIPanel_ModifyStudyTime : UIPanel
{
    private UIPanel_ModifyStudent uiParent;

    [SerializeField]
    private SpriteAtlas atlas;

    [SerializeField]
    private Image icon;
    [SerializeField]
    private TMPro.TMP_Text nameText;

    [SerializeField]
    UI_StudyTimeModify uiTimeModifyPrefab;
    [SerializeField]
    private Transform timeRoot;

    private List<UI_StudyTimeModify> timeModList = new List<UI_StudyTimeModify>();

    private void Awake()
    {
        uiType = UIManager.UI_type.UI_MOD_STUDY_TIME;
    }

    public void SetStudentInfo(StudentInfo info, UIPanel_ModifyStudent ui)
    {
        icon.sprite = atlas.GetSprite(info.sprite);
        nameText.text = info.name;

        CreateTimeList(info.studyTime);
        uiParent = ui;
    }

    private void CreateTimeList(List<System.DateTime> timeList)
    {
        if (timeList == null)
        {
            return;
        }

        for (int i=0; i< timeList.Count; i++)
        {
            CreateStudyTime(timeList[i]);
        }
    }

    private void CreateStudyTime(System.DateTime dt)
    {
        UI_StudyTimeModify timeMod = GameObject.Instantiate<UI_StudyTimeModify>(uiTimeModifyPrefab, timeRoot);
        timeMod.Init(dt, this);
        
        timeModList.Add(timeMod);
    }

    public void AddStudyTime()
    {
        System.DateTime curTime = System.DateTime.Now;
        CreateStudyTime(curTime);
    }

    public void DeleteStudyTime()
    {
        int idx = -1;
        for (int i = 0; i < timeModList.Count; i++)
        {
            if (timeModList[i].IsDelete)
            {
                idx = i;
                break;
            }
        }

        DestroyImmediate(timeModList[idx].gameObject);
        timeModList.RemoveAt(idx);
    }

    public void Save()
    {
        if (isClicked)
        {
            return;
        }

        isClicked = true;

        List<System.DateTime>  studyTime = new List<System.DateTime>(timeModList.Count);
        for(int i=0; i< timeModList.Count; i++)
        {
            studyTime.Add(timeModList[i].StudyTime());
        }

        studyTime.Sort(TimeSort);

        DestroyAllTimeMod();

        uiParent.ModifyStudyTime(studyTime);
        if(UIManager.Instance.CloseUI())
        {
            isClicked = false;
        }
    }

    public int TimeSort(System.DateTime dt1, System.DateTime dt2)
    {
        if(dt1.DayOfWeek > dt2.DayOfWeek)
        {
            return 1;
        }
        else if(dt1.DayOfWeek < dt2.DayOfWeek)
        {
            return -1;
        }

        if (dt1.Hour > dt2.Hour)
        {
            return 1;
        }
        else if (dt1.Hour < dt2.Hour)
        {
            return -1;
        }

        if (dt1.Minute > dt2.Minute)
        {
            return 1;
        }
        else if (dt1.Minute < dt2.Minute)
        {
            return -1;
        }

        return 0;
    }

    public void Quit()
    {
        if (isClicked)
        {
            return;
        }

        DestroyAllTimeMod();

        isClicked = true;
        if(UIManager.Instance.CloseUI())
        {
            isClicked = false;
        }
    }

    private void DestroyAllTimeMod()
    {
        for (int i = 0; i < timeModList.Count; i++)
        {
            DestroyImmediate(timeModList[i].gameObject);
        }
        timeModList.Clear();
    }
}
