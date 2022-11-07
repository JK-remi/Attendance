using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_IconSet : MonoBehaviour
{
    private UIPanel_SelectIcon parentUI;

    public List<Image> iconSet;

    public void SetParent(UIPanel_SelectIcon target)
    {
        parentUI = target;
    }

    public void SelectIcon(int idx)
    {
        if(parentUI == null)
        {
            return;
        }

        parentUI.ChangeIcon(Utils.SubClone(iconSet[idx].sprite.name));
    }
}
