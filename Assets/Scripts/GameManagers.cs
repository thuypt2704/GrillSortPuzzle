using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public class GameManagers : MonoBehaviour
{
    [SerializeField] private int _totalFood;
    [SerializeField] private int _totalGrill;
    [SerializeField] private Transform _gridGrill;


    private List<GrillStation> _listGrills;
    private float _avgTray; //so luong thuc an trung binh trong mot dia
    private List<Sprite> _totalSpriteFood;

    private void Awake()
    {
        _listGrills = Utils.GetListInChild<GrillStation>(_gridGrill);
        Sprite[] loadedSprite = Resources.LoadAll<Sprite>("item");
        _totalSpriteFood = loadedSprite.ToList();
    }

    void Start()
    {
        OnInitLevel();
    }

    private void OnInitLevel()
    {
        List<Sprite> takeFood = _totalSpriteFood.OrderBy(x => Random.value).Take(_totalFood).ToList();
        List<Sprite> useFood = new List<Sprite>();
        for(int i = 0; i< takeFood.Count; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                useFood.Add(takeFood[i]);
            }
        }
        //random, trao doi vi tri cua cac item
        for(int i=0; i < useFood.Count; i++)
        {
            int rand = Random.Range(0, useFood.Count);
            (useFood[i], useFood[rand]) = (useFood[rand], useFood[i]); // hamn nay doi vi tri i hien tai cua vong lap va vi tri random

        }

        _avgTray = Random.Range(1.5f, 2.5f);
        int totalTray = Mathf.RoundToInt(useFood.Count / _avgTray); //tinh tong so dia


        List<int> trayPerGrill = this.DistributeEvelyn(_totalGrill, totalTray);
        List<int> foodPerGrill = this.DistributeEvelyn(_totalGrill, useFood.Count);
        
        for(int i = 0; i< _listGrills.Count; i++)
        {
            bool activeGrill = i < _totalGrill;
            _listGrills[i].gameObject.SetActive(activeGrill);

            if (activeGrill)
            {
                List<Sprite> lisFood = Utils.TakeAndRemoveRandom<Sprite>(useFood, foodPerGrill[i]);
                _listGrills[i].OnInitGrill(trayPerGrill[i], lisFood);
            }
        }

    }
    //Chia deu so khay
    private List<int> DistributeEvelyn(int grillCount, int totalTrays)
    {
        List<int> result = new List<int>();
        //tinh trung binh so luong dia
        float avg = (float)totalTrays / grillCount;// 3.5
        int low = Mathf.FloorToInt(avg);//3
        int high = Mathf.CeilToInt(avg);//4

        int hightCount = totalTrays - low * grillCount; // tinh so bep nhieu khay hon
        int lowCount = grillCount - hightCount;

        for(int i = 0;i<lowCount; i++)
        {
            result.Add(low);
        }
        for (int i = 0; i < hightCount; i++)
        {
            result.Add(high);
        }
        //dao vi tri 
        for(int i = 0; i<result.Count; i++)
        {
            int rand = Random.Range(i, result.Count);
            (result[i], result[rand]) = (result[rand], result[i]);
        }
        return result;
    }
}
