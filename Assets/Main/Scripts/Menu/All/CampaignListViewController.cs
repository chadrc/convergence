using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class CampaignListViewController : MonoBehaviour, IListInfo
{
    public GameObject CampaignListItemPrefab;
    public GameObject CampaignListParent;
    public GameObject CampaignLevelListParent;
    public CanvasGroup CampaignLevelListPanel;
    public Button StartBtn;
    public Text InfoText;
    public LevelList[] Campaigns;

    private ListItem currentCampaign;
    private int curCampIndex;
    private ListItem currentLevel;

    private float initialY;
    private Vector2 initialPos;
    private Vector2 destPos;

    RectTransform rect { get { return GetComponent<RectTransform>(); } }

	// Use this for initialization
	void Start ()
    {
        initialPos = rect.anchoredPosition;
        destPos = initialPos - new Vector2(500, 0);
        int i = 0;
        foreach (var c in Campaigns)
        {
            var obj = GameObject.Instantiate(CampaignListItemPrefab) as GameObject;
            var listItem = obj.GetComponent<ListItem>();
            if (listItem == null)
            {
                Debug.LogError("No ListItem attached to CampaignListItemPrefab.");
                return;
            }

            listItem.Selected += ListItemSelected;
            listItem.SetIndex(this, i);
            obj.transform.SetParent(CampaignListParent.transform);
            obj.transform.localScale = Vector3.one;
            i++;
        }
        var parRect = CampaignLevelListPanel.transform as RectTransform;
        initialY = parRect.anchoredPosition.y;
    }

    public void Open()
    {
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(GetComponent<CanvasGroup>(), destPos, initialPos, .2f, 1.0f));
    }

    public void Close()
    {
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(GetComponent<CanvasGroup>(), initialPos, destPos, .2f, -1.0f));
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        CampaignLevelListPanel.alpha = 0.0f;
        currentCampaign = null;
        currentLevel = null;
        StartBtn.gameObject.SetActive(false);
    }

    public void StartBtnPressed()
    {
        if (Game.HasSelectedLevel())
        {
            SceneManager.LoadScene("Level");
        }
    }

    public void LevelSelected(ListItem item)
    {
        var levelList = GetLevelList(currentCampaign.ListIndex);
        if (item.ListIndex >= 0 && item.ListIndex < levelList.Levels.Count)
        {
            currentLevel = item;
            Game.SetSelectedLevel(item.ListIndex);
            //Debug.Log("Level Set: " + item.ListIndex);
            StartBtn.gameObject.SetActive(true);
        }
    }

    public void ListItemSelected(ListItem item)
    {
        foreach(Transform t in CampaignLevelListParent.transform)
        {
            GameObject.Destroy(t.gameObject);
        }

        var campaignBtnRect = item.transform as RectTransform;
        var levelList = GetLevelList(item.ListIndex);
        if (levelList == null)
        {
            Debug.LogError("Invalid index.");
            return;
        }

        Game.SetLevelList(levelList);
        //Debug.Log("Campaign Set: " + levelList.Levels.Count);
        var info = new LevelListInfo(levelList);
        info.InfoTextDelegate += SetInfoText;
        curCampIndex = item.ListIndex;
        currentCampaign = item;
        RectTransform lastRect = null;
        int i = 0;
        foreach (var l in levelList.Levels)
        {
            var obj = GameObject.Instantiate(CampaignListItemPrefab) as GameObject;
            var listItem = obj.GetComponent<ListItem>();
            if (listItem == null)
            {
                Debug.LogError("No ListItem attached to CampaignListItemPrefab.");
                return;
            }

            listItem.Selected += LevelSelected;
            listItem.SetIndex(info, i);
            obj.transform.SetParent(CampaignLevelListParent.transform);
            obj.transform.localScale = Vector3.one;
            i++;
            lastRect = listItem.transform as RectTransform;
        }

        // Position Menu
        float yDif = Mathf.Abs(lastRect.position.y - campaignBtnRect.position.y);
        var parRect = CampaignLevelListPanel.transform as RectTransform;

        float newY = (53.0f * (i-(curCampIndex+1)));

        var destPos = new Vector2(parRect.anchoredPosition.x, initialY + newY);
        var startPos = destPos - new Vector2(500.0f, 0);
        //parRect.anchoredPosition = destPos;
        //Debug.Log("New Y: " + newY);

        CampaignLevelListPanel.GetComponent<CanvasGroup>().alpha = 0;
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(CampaignLevelListPanel.GetComponent<CanvasGroup>(), startPos, destPos, .2f, 1.0f));
        //CampaignLevelListPanel.alpha = 1.0f;
        StartBtn.gameObject.SetActive(false);
    }

    public LevelList GetLevelList(int index)
    {
        if (Campaigns.Length > 0 && index >= 0 && index < Campaigns.Length)
        {
            return Campaigns[index];
        }
        return null;
    }

    #region IListInfo Interface

    public bool ListItemEnabled(int index)
    {
        return index <= 2;
    }

    public string GetLabelForListItem(int index)
    {
        var item = GetLevelList(index);
        return item == null ? "" : item.Name;
    }
    
    public void SetInfoText(string text)
    {
        InfoText.text = text;
    }

    public string GetDiscriptionForListItem(int index)
    {
        var item = GetLevelList(index);
        return item == null ? "" : item.Description;
    }

    #endregion
}
