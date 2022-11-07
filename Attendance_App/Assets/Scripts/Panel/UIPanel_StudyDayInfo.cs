using System.Collections.Generic;
using UnityEngine;

public class UIPanel_StudyDayInfo : UIPanel
{
    [SerializeField]
    private UI_StudyTimeInfo timeInfoPrefab;
    [SerializeField]
    private Transform uiParent;
    private List<UI_StudyTimeInfo> timeInfoList = new List<UI_StudyTimeInfo>();

    [SerializeField]
    private TMPro.TMP_Text dateText;
    [SerializeField]
    private GameObject emptyNotice;

    private const string beforeStr = "어제";
    private const string todayStr = "오늘";
    private const string afterStr = "내일";
    private string originTitle;

    private void Awake()
    {
        uiType = UIManager.UI_type.UI_DAY_INFO;
        originTitle = Title;
    }

    public void SetDayInfo(System.DateTime dt)
    {
        int daySpan = Utils.GetSpanDays(System.DateTime.Now, dt);
        if (daySpan == 0)
        {
            strTitle = todayStr + originTitle;
        }
        else if(daySpan > 0)
        {
            strTitle = beforeStr + originTitle;
        }
        else
        {
            strTitle = afterStr + originTitle;
        }
        UIManager.Instance.SetTitle(Title);

        dateText.text = dt.ToString("yyyy년 MM월 dd일 (ddd)");

        List<StudyTimeInfo> studyInfos = DBManager.Instance.GetStudyList(dt);

        emptyNotice.SetActive(studyInfos.Count == 0);

        bool isMakeUI = studyInfos.Count > timeInfoList.Count;
        int length = isMakeUI ? studyInfos.Count : timeInfoList.Count;
        for(int i = 0; i < length; i++)
        {
            if (isMakeUI)
            {
                if(i < timeInfoList.Count)
                {
                    timeInfoList[i].Init(studyInfos[i]);
                    timeInfoList[i].gameObject.SetActive(true);
                }
                else
                {
                    UI_StudyTimeInfo info = GameObject.Instantiate<UI_StudyTimeInfo>(timeInfoPrefab, uiParent);
                    info.Init(studyInfos[i]);

                    timeInfoList.Add(info);
                }
            }
            else
            {
                if(i < studyInfos.Count)
                {
                    timeInfoList[i].Init(studyInfos[i]);
                    timeInfoList[i].gameObject.SetActive(true);
                }
                else
                {
                    timeInfoList[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
