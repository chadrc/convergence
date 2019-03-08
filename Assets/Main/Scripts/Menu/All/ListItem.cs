using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class ListItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text Label;

    public int ListIndex { get; private set; }
    public event System.Action<ListItem> Selected;
    
    public IListInfo Info { get; private set; } 

    public void SetIndex(IListInfo info, int listIndex)
    {
        Info = info;
        ListIndex = listIndex;

        if (!Info.ListItemEnabled(ListIndex))
        {
            GetComponent<Button>().interactable = false;
            Label.text = "Locked";
        }
        else
        {
            Label.text = Info.GetLabelForListItem(ListIndex);
        }
    }

    public void ItemSelected()
    {
        if (Selected != null)
        {
            Selected(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Info.SetInfoText(Info.GetDiscriptionForListItem(ListIndex));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Info.SetInfoText("");
    }
}
