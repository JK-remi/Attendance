using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPanel_Attendance : UIPanel
{
    enum eSort
    {
        NameD,
        NameU,
        StartD,
        StartU,
        Study
    }

    [SerializeField]
    private UI_StudentInfo infoPrefab;

    [SerializeField]
    private Transform contentParent;
    private List<UI_StudentInfo> infoUIList;

    [SerializeField]
    private TMPro.TMP_Dropdown SortDd;

    [SerializeField]
    private TMPro.TMP_InputField searchInput;

    private void Awake()
    {
        uiType = UIManager.UI_type.UI_ATTENDANCE;
    }

    private void Start()
    {
        CreateStudentInfo();
        Sort();
    }
    private void CreateStudentInfo()
    {
        List<ulong> infoList = new List<ulong>(DBManager.Instance.IDs);
        infoUIList = new List<UI_StudentInfo>(infoList.Count);

        for (int i = 0; i < infoList.Count; i++)
        {
            AddUI_StudentInfo(infoList[i]);
        }
    }

    private void AddUI_StudentInfo(ulong id)
    {
        UI_StudentInfo info = GameObject.Instantiate<UI_StudentInfo>(infoPrefab, contentParent);
        info.Init(id, this);
        infoUIList.Add(info);
    }

    public void AddStudent()
    {
        if(isClicked)
        {
            return;
        }

        isClicked = true;

        if(UIManager.Instance.OpenUI(UIManager.UI_type.UI_MOD_STUDENT_INFO, true))
        {
            ((UIPanel_ModifyStudent)UIManager.Instance.CurUI).SetStudentInfo(0, ActualAddStudent);
            isClicked = false;
        }
    }

    public void ActualAddStudent()
    {
        StudentInfo info = DBManager.Instance.ModifiedStudent;
        AddUI_StudentInfo(info.id);

        Sort();
    }

    public void UpdateModifiedStudent()
    {
        StudentInfo info = DBManager.Instance.ModifiedStudent;
        for(int i=0; i<infoUIList.Count; i++)
        {
            if(infoUIList[i].ID == info.id)
            {
                infoUIList[i].Init(info.id, this);
                break;
            }
        }

        Sort();
    }

    public void RemoveStudent(UI_StudentInfo uiInfo)
    {
        DBManager.Instance.Remove(uiInfo.ID);
        infoUIList.Remove(uiInfo);
        DestroyImmediate(uiInfo.gameObject);
    }

    public void Search()
    {
        StudentInfo info;

        for (int i = 0; i < infoUIList.Count; i++)
        {
            infoUIList[i].gameObject.SetActive(true);
            
            if (searchInput.text == string.Empty)
            {
                continue;
            }

            info = DBManager.Instance.Find(infoUIList[i].ID);
            if(info == null || !info.name.Contains(searchInput.text))
            {
                infoUIList[i].gameObject.SetActive(false);
            }
        }
    }

    public void Sort()
    {
        List<StudentInfo> infoList = new List<StudentInfo>(infoUIList.Count);
        for(int i=0; i< infoUIList.Count; i++)
        {
            infoList.Add(DBManager.Instance.Find(infoUIList[i].ID));
        }
        List<StudentInfo> sortedList = null;

        switch ((eSort)SortDd.value)
        {
            case eSort.NameD:
                {
                    sortedList = infoList.OrderBy(x => x.name).ToList();
                    break;
                }
            case eSort.NameU:
                {
                    sortedList = infoList.OrderByDescending(x => x.name).ToList();
                    break;
                }
            case eSort.StartD:
                {
                    sortedList = infoList.OrderBy(x => x.startDT).ToList();
                    break;
                }
            case eSort.StartU:
                {
                    sortedList = infoList.OrderByDescending(x => x.startDT).ToList();
                    break;
                }
            case eSort.Study:
                {
                    sortedList = infoList.OrderBy(x => x.ClosestStudyTime(System.DateTime.Now)).ToList();
                    break;
                }
        }

        for(int i=0; i<infoUIList.Count; i++)
        {
            infoUIList[i].Init(sortedList[i].id, this);
        }
    }
}
