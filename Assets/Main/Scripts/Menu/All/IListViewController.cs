using UnityEngine;
using System.Collections;
using System;

public interface IListViewController
{
}

public interface IListInfo
{
    bool ListItemEnabled(int index);
    string GetLabelForListItem(int index);
    string GetDiscriptionForListItem(int index);
    void SetInfoText(string text);
}

public class LevelListInfo : IListInfo
{
    private LevelList info;
    public System.Action<string> InfoTextDelegate;

    public LevelListInfo(LevelList info)
    {
        this.info = info;
    }

    public string GetLabelForListItem(int index)
    {
        if (index >= 0 && index < info.Levels.Count)
        {
            return info.Levels[index].Name;
        }
        return "";
    }

    public bool ListItemEnabled(int index)
    {
        return true;
    }

    public void SetInfoText(string text)
    {
        if (InfoTextDelegate != null)
        {
            InfoTextDelegate(text);
        }
    }

    public string GetDiscriptionForListItem(int index)
    {
        if (index >= 0 && index < info.Levels.Count)
        {
            return info.Levels[index].Discription;
        }
        return "";
    }
}