using UnityEngine;
using System.Collections;

public class TargetingController : MonoBehaviour
{
    public TargetUI FirstTarget;
    public TargetUI SecondTarget;

    private TowerButtonBehavior first;
    private TowerButtonBehavior second;

    void Start()
    {
        UIPointerResponder.PlayerDeselectedTower += OnTowerDeselected;
        UIPointerResponder.PlayerHoveredTower += OnTowerHover;
        UIPointerResponder.PlayerUnhoveredTower += OnTowerUnhover;
        UIPointerResponder.PlayerSelectedTower += OnTowerSelected;
    }
    
    void OnDestroy()
    {
        UIPointerResponder.PlayerDeselectedTower -= OnTowerDeselected;
        UIPointerResponder.PlayerHoveredTower -= OnTowerHover;
        UIPointerResponder.PlayerUnhoveredTower -= OnTowerUnhover;
        UIPointerResponder.PlayerSelectedTower -= OnTowerSelected;
    }

    void OnTowerSelected(TowerButtonBehavior btn)
    {
        if (first == null)
        {
            FirstTarget.SetToTowerButton(btn);
            first = btn;
        }
    }

    void OnTowerDeselected(TowerButtonBehavior btn)
    {
        if (btn == first)
        {
            FirstTarget.Hide();
            first = null;
        }
    }

    void OnTowerHover(TowerButtonBehavior btn)
    {
        if (second != btn)
        {
            second = btn;
            SecondTarget.SetToTowerButton(btn);
        }
    }

    void OnTowerUnhover(TowerButtonBehavior btn)
    {
        SecondTarget.Hide();
        second = null;
    }
}
