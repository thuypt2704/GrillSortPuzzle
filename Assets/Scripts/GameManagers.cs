using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering;
using System.Linq.Expressions;
using System.Collections;

public class GameManagers : MonoBehaviour
{
    private static GameManagers _instance;
    public static GameManagers Instance => _instance;
    [SerializeField] private int _allFood;
    [SerializeField] private int _totalFood;
    [SerializeField] private int _totalGrill;
    [SerializeField] private Transform _gridGrill;
    [SerializeField] private Transform _magnetFX;
    [SerializeField] private List<Image> _magnetList;
    [SerializeField] private ParticleSystem _fxNewGrill;

    private List<GrillStation> _listGrills;
    private float _avgTray; //so luong thuc an trung binh trong mot dia
    private List<Sprite> _totalSpriteFood;

    private void Awake()
    {
        _listGrills = Utils.GetListInChild<GrillStation>(_gridGrill);
        Sprite[] loadedSprite = Resources.LoadAll<Sprite>("item");
        _totalSpriteFood = loadedSprite.ToList();
        _instance = this;
    }

    void Start()
    {
        OnInitLevel();
    }

    private void OnInitLevel()
    {
        List<Sprite> takeFood = _totalSpriteFood.OrderBy(x => Random.value).Take(_totalFood).ToList();
        List<Sprite> useFood = new List<Sprite>();

        for(int i = 0; i < _allFood; i++)
        {
            int n = i % takeFood.Count;
            for (int j = 0; j < 3; j++)
            {
                useFood.Add(takeFood[n]);
            }
        }
        
        _avgTray = Random.Range(1.4f, 2f);
        int totalTray = Mathf.RoundToInt(useFood.Count / _avgTray); //tinh tong so dia

        List<int> trayPerGrill = this.DistributeEvelyn(_totalGrill, totalTray);
        List<int> foodPerGrill = this.DistributeEvelyn(_totalGrill, useFood.Count);
        
        for(int i = 0; i < _listGrills.Count; i++)
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

        for(int i = 0;i < lowCount; i++)
        {
            result.Add(low);
        }
        for (int i = 0; i < hightCount; i++)
        {
            result.Add(high);
        }
        //dao vi tri 
        for(int i = 0; i < result.Count; i++)
        {
            int rand = Random.Range(i, result.Count);
            (result[i], result[rand]) = (result[rand], result[i]);
        }
        return result;
    }

    public void OnMinusFood()
    {
        --_allFood;

        if (_allFood <= 0)
        {
            Debug.Log("Game Complete");
        }
    }

    public void OnCheckAndShake()
    {
        Dictionary<string, List<FoodSlot>> groups = new Dictionary<string, List<FoodSlot>>(); 

        foreach(var grill in _listGrills)
        {
            if(grill.gameObject.activeInHierarchy)
            {
               for(int i = 0; i < grill.TotalSlot.Count; i++)
               {
                    FoodSlot slot = grill.TotalSlot[i];
                    if (slot.HasFood)
                    {
                        string name = slot.GetSpriteFood.name;
                        if(!groups.ContainsKey(name))
                        {
                            groups.Add(name,new List<FoodSlot>());
                        }
                        groups[name].Add(slot);
                    }
               }
            }
        }

        foreach(var kvp in groups)
        {
            if(kvp.Value.Count >= 3)
            {
                for(int i = 0; i < 3; i++)
                {
                    kvp.Value[i].DoShake();
                }
                return;
            }
        }
    }
    public void OnMagnet()
    {
        Dictionary<string, List<Image>> groups = new Dictionary<string, List<Image>>();

        foreach(var grill in _listGrills)
        {
            if (grill.gameObject.activeInHierarchy)
            {
                for(int i = 0; i < grill.TotalSlot.Count; i++)
                {
                    FoodSlot slot = grill.TotalSlot[i];
                    if(slot.HasFood)
                    {
                        string name = slot.GetSpriteFood.name;
                        if (!groups.ContainsKey(name))
                        {
                            groups[name] = new List<Image>();
                        }
                        groups[name].Add(slot.ImgFood);
                    }
                }

                TrayItem tray = grill.GetFirstTray();
                if(tray != null)
                {
                    for (int i = 0; i < tray.FoodList.Count; i++)
                    {
                        Image img = tray.FoodList[i];
                        if (img.gameObject.activeInHierarchy)
                        {
                            string name = img.sprite.name;
                            if (!groups.ContainsKey(name))
                            {
                                groups[name] = new List<Image>();
                            }
                            groups[name].Add(img);
                        }
                    }
                }
            }
        }

        StartCoroutine(IECollect());

        IEnumerator IECollect()
        {
            foreach(var kvp in groups)
            {
                if(kvp.Value.Count >= 3)
                {
                    _magnetFX.DOScale(Vector3.one, 0.4f);
                    yield return new WaitForSeconds(0.3f);

                    for(int i = 0; i < 3; i++)
                    {
                        Image imgDummy = _magnetList[i];
                        Image imgFood = kvp.Value[i];
                        imgDummy.sprite = imgFood.sprite;
                        imgDummy.SetNativeSize();
                        imgDummy.transform.position = imgFood.transform.position;
                        imgDummy.gameObject.SetActive(true);
                        imgFood.gameObject.SetActive(false);
                        imgDummy.color = new Color(1f, 1f, 1f, 1f);

                        Vector3 mid = (imgDummy.transform.position + _magnetFX.position) / 2f;
                        mid += new Vector3(Random.Range(-2f,2f), Random.Range(2f, 2f), 0f);
                        Vector3[] path = new Vector3[] { imgDummy.transform.position, mid, _magnetFX.position };

                        Sequence seq = DOTween.Sequence();
                        seq.Join(imgDummy.transform.DOPath(path, 1.5f, PathType.CatmullRom))
                            .Join(imgDummy.DOColor(new Color(1f, 1f, 1f, 0f), 1.5f))
                            .SetEase(Ease.OutQuad)
                            .OnComplete(() =>
                            {
                                imgDummy.gameObject.SetActive(false);
                                imgDummy.transform.localScale = Vector3.one;
                                imgFood.gameObject.SendMessageUpwards("OnCheckPrepareTray");
                            });
                        yield return new WaitForSeconds(0.1f);
                    }
                    yield return new WaitForSeconds(1f);
                    _magnetFX.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
                    yield break;
                }
            }
        }
         
    }

    public void OnShuffle()
    {
        StartCoroutine(IEShuffle());

        IEnumerator IEShuffle()
        {
            List<Image> result = new List<Image>();
            foreach(var grill in _listGrills)
            {
                if (grill.gameObject.activeInHierarchy)
                {
                    result.AddRange(grill.ListFoodActive());
                    grill.OnShuffleFX();
                }
            }
            yield return new WaitForSeconds(0.25f);
            for(int i = 0; i < result.Count; i++)
            {
                int n = Random.Range(0, result.Count);
                Sprite tmp = result[i].sprite;
                result[i].sprite = result[n].sprite;
                result[n].sprite = tmp;
                result[i].SetNativeSize();
                result[n].SetNativeSize();
            }
        }
    }
    public void OnAddMoreGrill()
    {
        foreach(var grill in _listGrills)
        {
            if (!grill.gameObject.activeInHierarchy)
            {
                grill.gameObject.SetActive(true);
                _fxNewGrill.transform.SetParent(grill.transform);
                _fxNewGrill.transform.localPosition = Vector3.up * 100f;
                _fxNewGrill.Play();
                break;
            }
        }
    }
}

