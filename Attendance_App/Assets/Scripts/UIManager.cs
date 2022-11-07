using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public const string ICON_NAME = "IconName";

    public enum UI_type
    {
        [Description("Prefabs/TimeManager/Panel_Main")]
        UI_MAIN = 0,

        [Description("Prefabs/TimeManager/Panel_Schedule")]
        UI_SCHEDULER,
        [Description("Prefabs/TimeManager/Panel_DayInfo")]
        UI_DAY_INFO,

        [Description("Prefabs/TimeManager/Panel_Attendance")]
        UI_ATTENDANCE,
        [Description("Prefabs/TimeManager/Panel_StudentInfo")]
        UI_STUDENT_INFO,
        [Description("Prefabs/TimeManager/Panel_Mod_Student")]
        UI_MOD_STUDENT_INFO,
        [Description("Prefabs/TimeManager/Panel_Select_Icon")]
        UI_SELECT_ICON,
        [Description("Prefabs/TimeManager/Panel_Mod_Time")]
        UI_MOD_STUDY_TIME,

        [Description("Prefabs/TimeManager/Panel_Mod_Sup")]
        UI_MOD_SUPPLEMENT,
        [Description("Prefabs/TimeManager/Panel_Select_Sup")]
        UI_SELECT_SUP_DAY,
        [Description("Prefabs/TimeManager/Panel_DayInfoMini")]
        UI_SUP_DAY_INFO,

        [Description("")]
        UI_SETTING,

        [Description("Prefabs/TimeManager/Panel_OkCancel")]
        UI_OK_CANCEL,
        [Description("")]
        UI_NOTICE,

        [Description("Prefabs/TimeManager/Panel_Title")]
        UI_TITLE,
        END
    }

    private static UIManager _instance = null;
    public static UIManager Instance
    {
        get
        {
            _instance = GameObject.FindObjectOfType<UIManager>();
            if (_instance == null)
            {
                GameObject container = new GameObject("UIManager");
                _instance = container.AddComponent<UIManager>();

                Canvas parentCanvas = GameObject.FindObjectOfType<Canvas>();
                if (parentCanvas != null)
                {
                    _instance.parentTrans = parentCanvas.transform;
                }
                else
                {
                    _instance.parentTrans = container.transform;
                }
            }

            return _instance;
        }
    }

    private Dictionary<UI_type, UIPanel> uiDic;

    private Transform parentTrans = null;
    private Stack<UIPanel> _uiStorage = null;

    public UIPanel CurUI
    {
        get 
        {
            if(_uiStorage == null || _uiStorage.Count == 0)
            {
                return null;
            }
            
            return _uiStorage.Peek(); 
        }
    }

    private void Awake()
    {
        if(uiDic == null)
        {
            uiDic = new Dictionary<UI_type, UIPanel>();
        }

        if(_uiStorage == null)
        {
            _uiStorage = new Stack<UIPanel>();
        }

        if(parentTrans == null)
        {
            GameObject canvasObj = GameObject.Find("Canvas");
            if(canvasObj == null)
            {
                Canvas canvas = this.GetComponent<Canvas>();
                if(canvas != null)
                {
                    canvasObj = canvas.gameObject;
                }
                else
                {
                    canvasObj = this.gameObject;
                }
            }

            parentTrans = canvasObj.transform;
        }

        Load();
    }

    private void Start()
    {
        UIManager.Instance.OpenUI(UI_type.UI_MAIN, true);
    }

    private void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(_uiStorage.Count > 1)
                {
                    uiTitle.GoBack();
                }
                else if(IsOpen(UI_type.UI_OK_CANCEL) == false)
                {
                    if(UIManager.Instance.OpenUI(UI_type.UI_OK_CANCEL, true))
                    {
                        ((UIPanel_OK_Cancel)CurUI).SetNotice("정말 종료하시겠습니까?", QuitApplication);
                    }
                }
            }
        }
    }

    private void Load()
    {
        string emptyString = string.Empty;
        GameObject origin;
        GameObject uiObject;
        UIPanel ui;
        string path = emptyString;

        for(UI_type type = UI_type.UI_MAIN; type < UI_type.END; type++)
        {
            if (uiDic.TryGetValue(type, out ui))
            {
                // it's already exist at ui dictionary
                continue;
            }

            path = Utils.GetEnumDescription(type.GetType(), type.ToString());
            if (path == emptyString)
            {
                Debug.LogWarning("<UIM> [" + type.ToString() + "] has no description(path)");
                continue;
            }

            origin = (GameObject)Resources.Load(path);
            if(origin == null)
            {
                Debug.LogWarning("<UIM> [" + type.ToString() + "] has wrong path. there is no ui exist");
                continue;
            }

            uiObject = GameObject.Instantiate(origin, parentTrans);
            ui = uiObject.GetComponent<UIPanel>();
            if(ui == null)
            {
                Debug.LogWarning("<UIM> [" + type.ToString() + "] has no script (UIPanel) check it");
                continue;
            }

            if(ui.UiType == UI_type.UI_TITLE)
            {
                uiTitle = ui as UIPanel_Title;
            }
            ui.gameObject.SetActive(false);
            uiDic.Add(type, ui);
        }
    }

    public bool OpenUI(UI_type type, bool bActivePreUI)
    {
        if (bActivePreUI == false)
        {
            CloseUI();
        }

        UIPanel ui;
        uiDic.TryGetValue(type, out ui);
        if(ui == null)
        {
            Debug.LogError("<UIM> There is no [" + type.ToString() + "] to open");
            return false;
        }

        ui.UI_OnOff(true);
        _uiStorage.Push(ui);

        ActivateBackButton();

        return true;
    }

    public bool CloseUI()
    {
        UIPanel ui = _uiStorage.Pop();
        if(ui == null)
        {
            Debug.LogError("<UIM> There is no more ui to close!!");
            return false;
        }

        ui.UI_OnOff(false);
        SetTitle(CurUI.Title);

        ActivateBackButton();

        return true;
    }

    public bool IsOpen(UI_type type)
    {
        foreach(UIPanel ui in _uiStorage)
        {
            if(ui.UiType == type)
            {
                return true;
            }
        }

        return false;
    }

    private UIPanel_Title uiTitle = null;
    public void OpenTitle()
    {
        if (uiTitle != null && uiTitle.IsOpen())
        {
            return;
        }

        UIPanel ui;
        uiDic.TryGetValue(UI_type.UI_TITLE, out ui);
        if (ui == null)
        {
            Debug.LogError("<UIM> There is no [" + UI_type.UI_TITLE.ToString() + "] to open");
            return;
        }

        SetTitle(CurUI.Title);

        ui.UI_OnOff(true);
    }

    public void SetTitle(string text)
    {
        if(uiTitle == null && text != string.Empty)
        {
            return;
        }

        uiTitle.ChangeTitle(text);
    }

    private void ActivateBackButton()
    {
        if (uiTitle == null || uiTitle.IsOpen() == false)
        {
            return;
        }

        if (CurUI != null && CurUI.UiType == UI_type.UI_MAIN)
        {
            ((UIPanel_Main)CurUI).ActiveMenu();
            uiTitle.ActiveBackButton(false);
        }
        else
        {
            uiTitle.ActiveBackButton(true);
        }
    }

    private void QuitApplication()
    {
        Application.Quit();
    }
}
