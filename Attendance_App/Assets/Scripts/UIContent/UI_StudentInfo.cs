using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Events;
public class UI_StudentInfo : MonoBehaviour
{
    private ulong _id;
    public ulong ID
    {
        get { return _id; }
    }
    UIPanel_Attendance uiParent;

    [SerializeField]
    private Image profileImg;
    [SerializeField]
    private Text nameTxt;

    [SerializeField]
    private SpriteAtlas atlas;

    bool isClicked = false;

    public void Init(ulong id, UIPanel_Attendance ui)
    {
        StudentInfo info = DBManager.Instance.Find(id);

        profileImg.sprite = atlas.GetSprite(info.sprite);
        nameTxt.text = info.name;

        _id = id;
        uiParent = ui;

        isClicked = false;
    }

    public void OpenProfile()
    {
        if(isClicked)
        {
            return;
        }

        isClicked = true;
        if(UIManager.Instance.OpenUI(UIManager.UI_type.UI_STUDENT_INFO, true))
        {
            ((UIPanel_StudentInfo)UIManager.Instance.CurUI).SetStudentInfo(_id);
            isClicked = false;
        }
    }

    public void Modify()
    {
        if (isClicked)
        {
            return;
        }

        isClicked = true;
        if (UIManager.Instance.OpenUI(UIManager.UI_type.UI_MOD_STUDENT_INFO, true))
        {
            ((UIPanel_ModifyStudent)UIManager.Instance.CurUI).SetStudentInfo(_id, uiParent.UpdateModifiedStudent);
            isClicked = false;
        }
    }

    public void Delete()
    {
        if (isClicked)
        {
            return;
        }

        isClicked = true;
        if (UIManager.Instance.OpenUI(UIManager.UI_type.UI_OK_CANCEL, true))
        {
            // ok cancel ui의 ok button event에 DeleteStudent() 연결
            ((UIPanel_OK_Cancel)UIManager.Instance.CurUI).SetNotice("정말 삭제 하시겠습니까?", new UnityAction(DeleteStudent));
            isClicked = false;
        }
    }

    private void DeleteStudent()
    {
        uiParent.RemoveStudent(this);
    }
}
