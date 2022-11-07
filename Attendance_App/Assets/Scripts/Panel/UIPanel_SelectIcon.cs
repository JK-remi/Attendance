using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Events;

public class UIPanel_SelectIcon : UIPanel
{
    [SerializeField]
    private SpriteAtlas atlas;
    private const string defaultSprite = "jjang1";
    private string originSprite;
    private string modifyedSprite;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Transform selected;
    private Dictionary<string, Transform> iconDic;

    [SerializeField]
    private Transform scrollParent;
    [SerializeField]
    private UI_IconSet iconSetPrefab;

    private UnityAction action;

    private void Awake()
    {
        uiType = UIManager.UI_type.UI_SELECT_ICON;
        CreateIconSelect();
    }

    protected override void Init()
    {
        base.Init();

        action = null;
        modifyedSprite = string.Empty;
        SetIcon(PlayerPrefs.GetString(UIManager.ICON_NAME));
    }

    public void SetAction(UnityAction ua)
    {
        action = ua;
    }

    private void SetIcon(string iconName)
    {
        if(iconName == string.Empty)
        {
            icon.sprite = atlas.GetSprite(defaultSprite);
            PointSelectedIcon(defaultSprite);
            originSprite = defaultSprite;
        }
        else
        {
            icon.sprite = atlas.GetSprite(iconName);
            PointSelectedIcon(iconName);
            originSprite = iconName;
        }
    }

    private void CreateIconSelect()
    {
        if(atlas == null || atlas.spriteCount  == 0)
        {
            return;
        }

        iconDic = new Dictionary<string, Transform>(atlas.spriteCount);

        const int iconPerSet = 3;
        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);

        List<Sprite> spriteList = new List<Sprite>(sprites);
        spriteList.Sort(delegate (Sprite s1, Sprite s2)
        {
            return s1.name.CompareTo(s2.name);
        });

        UI_IconSet iconSet;
        for(int i=0; ; i++)
        {
            iconSet = GameObject.Instantiate<UI_IconSet>(iconSetPrefab, scrollParent);
            iconSet.SetParent(this);
            for (int j=0; j < iconPerSet; j++)
            {
                int idx = i * iconPerSet + j;
                if (idx >= atlas.spriteCount)
                {
                    iconSet.iconSet[j].gameObject.SetActive(false);
                    continue;
                }

                iconSet.iconSet[j].sprite = spriteList[idx];
                iconDic.Add(Utils.SubClone(spriteList[idx].name), iconSet.iconSet[j].transform);
            }

            if((i + 1) * iconPerSet >= atlas.spriteCount)
            {
                break;
            }
        }
    }

    private void PointSelectedIcon(string name)
    {
        Transform selectedPos = iconDic[name];
        if(selectedPos == null)
        {
            return;
        }

        selected.SetParent(selectedPos);
        selected.localPosition = Vector3.zero;
    }

    public void ChangeIcon(string name)
    {
        Sprite s = atlas.GetSprite(name);
        if(s == null)
        {
            return;
        }

        icon.sprite = s;
        PointSelectedIcon(name);
        modifyedSprite = name;
    }

    public void Save()
    {
        if (isClicked)
        {
            return;
        }

        isClicked = true;
        if(action != null)
        {
            PlayerPrefs.SetString(UIManager.ICON_NAME, modifyedSprite);
            action.Invoke();
        }

        if (UIManager.Instance.CloseUI())
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
        PlayerPrefs.SetString(UIManager.ICON_NAME, originSprite);

        if (UIManager.Instance.CloseUI())
        {
            isClicked = false;
        }
    }
}
