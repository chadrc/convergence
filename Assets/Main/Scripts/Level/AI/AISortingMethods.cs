using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AISortingMethods
{
   

    public static List<List<TowerBehavior>> InsertionSortDistance(AIBehavior AI, Dictionary<int, List<TowerBehavior>> dictionary)
    {
        List<List<TowerBehavior>> currentTowers = new List<List<TowerBehavior>>(dictionary.Values);
        if(currentTowers.Count > 1)
        {
            for(int i = 1; i < currentTowers.Count;i++)
            {
                List<TowerBehavior> listInQuestion = new List<TowerBehavior>();
                listInQuestion.AddRange(currentTowers[i]);
                int previous = i - 1;
                while(previous >=0 && DistanceRoundInt(Vector3.Distance(currentTowers[previous][0].transform.position,AI.myTower.transform.position)) > 
                    DistanceRoundInt(Vector3.Distance(listInQuestion[0].transform.position,AI.myTower.transform.position)))
                {   
                    currentTowers[previous + 1].Clear();
                    currentTowers[previous + 1].AddRange(currentTowers[previous]);
                    previous--;
                }

                currentTowers[previous + 1].Clear();
                currentTowers[previous + 1].AddRange(listInQuestion);
            }

            return currentTowers;
        }

        //if count is 1
        return currentTowers;
      
   
    }

    public static List<List<TowerBehavior>> InsertionSortUnits(AIBehavior AI, Dictionary<int, List<TowerBehavior>> dictionary)
    {
        List<List<TowerBehavior>> currentTowers = new List<List<TowerBehavior>>(dictionary.Values);
        if (currentTowers.Count > 1)
        {
            for (int i = 1; i < currentTowers.Count; i++)
            {
                List<TowerBehavior> listInQuestion = new List<TowerBehavior>();
                listInQuestion.AddRange(currentTowers[i]);
                int previous = i - 1;
                while (previous >= 0 && currentTowers[previous][0].StationedUnits > listInQuestion[0].StationedUnits)
                {
                    currentTowers[previous + 1].Clear();
                    currentTowers[previous + 1].AddRange(currentTowers[previous]);
                    previous--;
                }

                currentTowers[previous + 1].Clear();
                currentTowers[previous + 1].AddRange(listInQuestion);
            }

            return currentTowers;
        }

        //if count is 1
        return currentTowers;

    }

    private static int DistanceRoundInt(float distance)
    {
        if (distance - Mathf.Floor(distance) >= 0.5f)
            return Mathf.CeilToInt(distance);

        return Mathf.FloorToInt(distance);
    }

    //proven to work
    private static void SwapLists(int index1, int index2, ref List<List<TowerBehavior>> container)
    {
        //store first index in temp variable
        List<TowerBehavior> temp = new List<TowerBehavior>();
        temp.AddRange(container[index1]);
        //set the first index equal to the second index
        container[index1].Clear();
        container[index1].AddRange(container[index2]);
        //set the second index equal to the temp variable(the original index 1 variable)
        container[index2].Clear();
        container[index2].AddRange(temp);
    }     
}

//Modified Quick Sorts for the AI, decided to use Insertion Sort instead due to the low
//number of lists that end up in the container.
//insertion sort is faster than quicksort if there are a lower number of objects in
//the container.
//The overhead of the recursion in Quick sort costs more even though it is more efficient
#region QuickSortDistance
/*public static List<List<TowerBehavior>> SortBasedOffDistance(AIBehavior AI, Dictionary<int, List<TowerBehavior>> dictionary)
{
    currentTowers = new List<List<TowerBehavior>>(dictionary.Values);
    currentAi = AI;
    QuickSortDistance(0, currentTowers.Count - 1);
    return currentTowers;

}

private static void QuickSortDistance(int startIndex, int endindex)
{
    if (startIndex < endindex)
    {
        int m = PartitionDistance(startIndex, endindex);
        QuickSortDistance(startIndex, m - 1);
        QuickSortDistance(m + 1, endindex);
    }
}

private static int PartitionDistance(int start, int end)
{
    int pivot = (start + end) / 2;
    SwapLists(pivot, end);


    int right = end - 1;
    int left = start;

    int distanceToCheck = DistanceRoundInt(Vector3.Distance(currentTowers[end][0].transform.position, currentAi.myTower.transform.position));

    do
    {
        while (left < end - 1 && DistanceRoundInt(Vector3.Distance(currentTowers[left][0].transform.position, currentAi.myTower.transform.position)) < distanceToCheck)
        {
            left++;
        }

        while (right >= 0 && DistanceRoundInt(Vector3.Distance(currentTowers[right][0].transform.position, currentAi.myTower.transform.position)) > distanceToCheck)
        {
            right--;
        }

        if (left <= right)
        {
            SwapLists(left, right);
        }
    } while (left <= right);

    SwapLists(left, end);
    return left;

}*/
#endregion
#region QuickSortUnits
/*

private static void QuickSortUnits(int startIndex, int endIndex)
{
    if (startIndex < endIndex)
    {
        int m = ParitionUnits(0, currentTowers.Count - 1);
        QuickSortUnits(startIndex, m - 1);
        QuickSortUnits(m + 1, endIndex);
    }
}

private static int ParitionUnits(int start, int finish)
{
    int pivot = (start + finish) / 2;
    SwapLists(pivot, finish);

    //finish is now our pivot
    int right = finish - 1;
    int left = start;

    int unitsToCheck = currentTowers[finish][0].StationedUnits;

    do
    {
        while (left < currentTowers.Count && currentTowers[left][0].StationedUnits < unitsToCheck)
        {
            left++;
        }

        while (right >= 0 && currentTowers[right][0].StationedUnits > unitsToCheck)
        {
            right--;
        }

        if (left <= right)
        {
            SwapLists(left, right);
        }
    } while (left <= right);

    SwapLists(left, finish);
    return left;

}*/


#endregion
/*  public static void SwapListsTest()
    {
        List<int> first = new List<int>();
        for (int i = 0; i < 10; i++)
            first.Add(i);

        Debug.Log("First has positives");


        List<int> second = new List<int>();
        for (int i = -9; i <= 0; i++)
            second.Add(i);

        Debug.Log("second has negatives");

        List<List<int>> testContainer = new List<List<int>>();
        testContainer.Add(first);
        testContainer.Add(second);


        List<int> temp = new List<int>();
        temp.AddRange(testContainer[0]);

        testContainer[0].Clear();
        testContainer[0].AddRange(testContainer[1]);

        testContainer[1].Clear();
        testContainer[1].AddRange(temp);

        Debug.Log("first now equals: ");
        foreach (int i in testContainer[0])
            Debug.Log(i);

        Debug.Log("second now equals: ");
        foreach (int i in testContainer[1])
            Debug.Log(i);
    }
*/


