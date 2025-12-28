using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class DropDragCtrl : MonoBehaviour
{
    [SerializeField] private Image _imgFoodDrag;
    [SerializeField] private float _timeCheckSuggest;

    private FoodSlot _currentFood, _cacheFood;
    private bool _hasDrag;
    private Vector3 _offset;
    private float _countTime;
    void Update()
    {
        _countTime += Time.deltaTime;

        if(_countTime >= _timeCheckSuggest)
        {
            _countTime = 0f;
            GameManagers.Instance?.OnCheckAndShake();
        }
        if (Input.GetMouseButtonDown(0)) //check khi kich chuot
        {
            _currentFood = Utils.GetRayCastUI<FoodSlot>(Input.mousePosition); // check o vi tri kich chuot xem co UI gan class FoodSlot khong
            if (_currentFood != null && _currentFood.HasFood)
            {
                _hasDrag = true;
                _cacheFood = _currentFood; // o goc
                _imgFoodDrag.gameObject.SetActive(true);
                _imgFoodDrag.sprite = _currentFood.GetSpriteFood;
                _imgFoodDrag.transform.position = _currentFood.transform.position; // gan vi tri
                
                _currentFood.OnActiveFood(false);//Tat hinh anh o o goc de tao cam giac da nhac len
                
                Vector3 mouseWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);// tinh offset
                _offset = mouseWordPos - _currentFood.transform.position;
            }
        }
        if (_hasDrag)
        {
            Vector3 mouseWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 foodPos = mouseWordPos + _offset;
            foodPos.z = 0f;
            _imgFoodDrag.transform.position = foodPos;
            _countTime = 0f;

            FoodSlot slot = Utils.GetRayCastUI<FoodSlot>(Input.mousePosition);

            if (slot != null)
            {
                if (!slot.HasFood)//vi tri item chua co food
                {
                    if (_cacheFood == null || _cacheFood.GetInstanceID() != slot.GetInstanceID())
                    {
                        _cacheFood?.OnHideFood();
                        _cacheFood = slot;
                        _cacheFood.OnFadeFood();
                        _cacheFood.OnSetSlot(_currentFood.GetSpriteFood);
                    }
                }
                else// vi tri tro chuot da co food
                {
                    FoodSlot slotAvailable = slot.GetSlotNull;
                    if (slotAvailable != null)
                    {
                        _cacheFood?.OnHideFood();
                        _cacheFood = slotAvailable;
                        _cacheFood.OnFadeFood();
                        _cacheFood.OnSetSlot(_currentFood.GetSpriteFood);
                    }
                    else
                    {
                        this.OnClearCacheSlot();
                    }
                }
            }
            else
            {
                if (_cacheFood != null)
                {
                    _cacheFood.OnHideFood();
                    _cacheFood = null;
                }
            }
        }

        if(Input.GetMouseButtonUp(0) && _hasDrag)
        {
            if(_cacheFood != null) // xu ly fill item
            {
                _imgFoodDrag.transform.DOMove(_cacheFood.transform.position, 0.15f).OnComplete(() =>
                {
                    _imgFoodDrag.gameObject.SetActive(false);
                    _cacheFood.OnSetSlot(_currentFood.GetSpriteFood);
                    _cacheFood.OnActiveFood(true);
                    _cacheFood.OnCheckMerge();
                    _currentFood?.OnCheckPrepareTray();
                    _cacheFood = null;
                    _currentFood = null; 
                });
            }
            else // xu ly tro ve vi tri ban dau
            {
                _imgFoodDrag.transform.DOMove(_currentFood.transform.position, 0.3f).OnComplete(() =>
                {
                    _imgFoodDrag.gameObject.SetActive(false);
                    _currentFood.OnActiveFood(true);

                });
            }
            _hasDrag = false;
        }
    }
    private void OnClearCacheSlot()
    {
        if (_cacheFood != null && _cacheFood.GetInstanceID() != _currentFood.GetInstanceID())
        {
                _cacheFood.OnShowNormal();
                _cacheFood = null;
        }
    }
}
