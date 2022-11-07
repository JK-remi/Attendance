using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Events;

public class UIPanel_ModifySupplement : UIPanel
{
    private UIPanel_StudentInfo uiParent;

    [SerializeField]
    private SpriteAtlas atlas;

    [SerializeField]
    private Image icon;
    [SerializeField]
    private TMPro.TMP_Text nameText;

    [SerializeField]
    private UI_SupplementModify uiModSupPrefab;
    [SerializeField]
    private Transform timeRoot;

    private List<UI_SupplementModify> modSupList = new List<UI_SupplementModify>();

    private void Awake()
    {
        uiType = UIManager.UI_type.UI_MOD_SUPPLEMENT;
    }

    public void SetStudentInfo(UIPanel_StudentInfo ui)
    {
        icon.sprite = atlas.GetSprite(ui.Target.sprite);
        nameText.text = ui.Target.name;

        CreateTimeList(ui.Target.supplementTime);
        uiParent = ui;
    }

    private void CreateTimeList(Dictionary<System.DateTime, System.DateTime> supList)
    {
        foreach(System.DateTime key in supList.Keys)
        {
            CreateSupplement(key, supList[key]);
        }
    }

    private void CreateSupplement(System.DateTime key, System.DateTime val)
    {
        UI_SupplementModify timeMod = GameObject.Instantiate<UI_SupplementModify>(uiModSupPrefab, timeRoot);
        timeMod.Init(key, val, this);

        modSupList.Add(timeMod);
    }

    public void DeleteSupplementTime()
    {
        int idx = -1;
        for (int i = 0; i < modSupList.Count; i++)
        {
            if (modSupList[i].IsDelete)
            {
                // 보충 list에서 해당 time remove
                if(uiParent.Target.supplementTime.ContainsKey(modSupList[i].Key))
                {
                    uiParent.Target.supplementTime.Remove(modSupList[i].Key);
                }

                idx = i;
                break;
            }
        }
        
        DestroyImmediate(modSupList[idx].gameObject);
        modSupList.RemoveAt(idx);
    }

    public void Save()
    {
        if (isClicked)
        {
            return;
        }

        isClicked = true;

        Dictionary<System.DateTime, System.DateTime> supTime = new Dictionary<System.DateTime, System.DateTime>(modSupList.Count);
        for (int i = 0; i < modSupList.Count; i++)
        {
            supTime.Add(modSupList[i].Key, modSupList[i].SupplementTime());
        }

        DestroyAllSupplement();

        uiParent.ModifySupplement(supTime);
        if (UIManager.Instance.CloseUI())
        {
            uiParent.SetStudentInfo(uiParent.Target.id);
            isClicked = false;
        }
    }

    public void Quit()
    {
        if (isClicked)
        {
            return;
        }

        DestroyAllSupplement();

        isClicked = true;
        if (UIManager.Instance.CloseUI())
        {
            isClicked = false;
        }
    }

    private void DestroyAllSupplement()
    {
        for (int i = 0; i < modSupList.Count; i++)
        {
            DestroyImmediate(modSupList[i].gameObject);
        }
        modSupList.Clear();
    }
}
