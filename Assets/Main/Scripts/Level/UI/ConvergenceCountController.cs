using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConvergenceCountController : MonoBehaviour
{
    [Header("Default Sprites")]
    public Sprite CounterEndCap;
    public Sprite CounterMid;

    [Header("Filled Sprites")]
    public Sprite CounterEndCapFilled;
    public Sprite CounterMidFilled;

    private GridLayoutGroup grid;
    private int numConvergences;
    private List<Image> images = new List<Image>();
    private int convergenceCount;

	public void Initialize(int count)
	{
		numConvergences = count;
		ConvergenceController.ConvergenceOccurred += OnConvergence;

		if (numConvergences > 0)
		{
			CreateChildImage(CounterEndCap);
		}

		for(int i=0; i<numConvergences-2; i++)
		{
			CreateChildImage(CounterMid);
		}

		if (numConvergences > 1)
		{
			var obj = CreateChildImage(CounterEndCap);
			obj.transform.eulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
		}
	}

    GameObject CreateChildImage(Sprite graphic)
    {
        var obj = new GameObject();
        obj.transform.SetParent(transform);
        obj.transform.localScale = Vector3.one;
        var img = obj.AddComponent<Image>();
        img.sprite = graphic;
        images.Add(img);
        return obj;
    }

    void OnConvergence()
    {
        if (convergenceCount == 0)
        {
            images[0].sprite = CounterEndCapFilled;
        }
        else if(convergenceCount == numConvergences-1)
        {
            images[numConvergences - 1].sprite = CounterEndCapFilled;
        }
		else if (convergenceCount < images.Count)
        {
            images[convergenceCount].sprite = CounterMidFilled;
        }
        convergenceCount++;
    }
}
