using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class TrayItem : MonoBehaviour
{
    private List<Image> _foodList;
    public List<Image> FoodList => _foodList;

    void Awake()
    {
        _foodList = Utils.GetListInChild<Image>(this.transform);
        for(int i = 0; i<_foodList.Count; i++)
        {
            _foodList[i].gameObject.SetActive(false);

            _foodList[i].rectTransform.sizeDelta = new Vector2(6f, 36f);
            _foodList[i].rectTransform.localScale = Vector3.one;
        }
    }
    public void OnSetFood(List<Sprite> items)
    {
        if (items.Count <= _foodList.Count)
        {
            for(int i = 0; i<items.Count; i++)
            {
                Image slot = this.RandomSlot();
                slot.gameObject.SetActive(true);
                slot.sprite = items[i];
                slot.rectTransform.sizeDelta = new Vector2(6f, 36f);
                slot.rectTransform.localScale = Vector3.one;
                //slot.SetNativeSize();
            }
        }
    }
    private Image RandomSlot()
    {
    rerand: int n = Random.Range(0, _foodList.Count);
        if (_foodList[n].gameObject.activeInHierarchy) goto rerand;

        return _foodList[n];
    }
}
