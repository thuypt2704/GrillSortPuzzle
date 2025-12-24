using UnityEngine;
using UnityEngine.UI;

public class DropDragCtrl : MonoBehaviour
{
    [SerializeField] private Image _imgFoodDrag;
    private FoodSlot _currentFood, _cacheFood;
    private bool _hasDrag;
    private Vector3 _offset;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //check khi kich chuot
        {
            _currentFood = Utils.GetRayCastUI<FoodSlot>(Input.mousePosition); // check o vi tri kich chuot xem co UI gan class FoodSlot khong
            if (_currentFood != null && _currentFood.HasFood)
            {
                _hasDrag = true;
                _cacheFood = _currentFood; // o goc
                _imgFoodDrag.gameObject.SetActive(true);
                _imgFoodDrag.sprite = _currentFood.GetSpriteFood;
                //_imgFoodDrag.transform.position = _currentFood.transform.position; // gan vi tri

                //Tat hinh anh o o goc de tao cam giac da nhac len
                _currentFood.OnActiveFood(false);
                // tinh offset
                Vector3 mouseWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _offset = mouseWordPos - _currentFood.transform.position;

                _currentFood.OnActiveFood(false);
            }
        }
        if (_hasDrag)
        {
            Vector3 mouseWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 foodPos = mouseWordPos + _offset;
            foodPos.z = 0f;
            _imgFoodDrag.transform.position = foodPos;

            FoodSlot slot = Utils.GetRayCastUI<FoodSlot>(Input.mousePosition);

            //if(slot != null)
            //{
            //    if (!slot.HasFood)//vi tri item chua co food
            //    {

            //        if (_cacheFood.GetInstanceID() != slot.GetInstanceID())
            //        {
            //            _cacheFood.OnHideFood();
            //            _cacheFood = slot;
            //            _cacheFood.OnFadeFood();
            //            _cacheFood.OnSetSlot(_currentFood.GetSpriteFood);
            //        }
            //    }
            //    else// vi tri tro chuot da co food
            //    {
            //        FoodSlot slotAvailable = slot.GetSlotNull;
            //        if(slotAvailable != null)
            //        {
            //            _cacheFood.OnHideFood();
            //            _cacheFood = slotAvailable;
            //            _cacheFood.OnFadeFood();
            //            _cacheFood.OnSetSlot(_currentFood.GetSpriteFood);
            //        }
            //    }
            //}
            //else
            //{
            //    if (_cacheFood != null)
            //    {
            //        _cacheFood.OnHideFood();
            //        _cacheFood = null;
            //    }
            //}

            if(slot != null && slot != _cacheFood)
            {
                //tra lai trang thai cu cho o cache truoc do
                if (_cacheFood != _currentFood) _cacheFood.OnHideFood();
                //Cap nhat cache moi
                _cacheFood = slot;
                if (!_cacheFood.HasFood)
                {
                    _cacheFood.OnSetSlot(_imgFoodDrag.sprite);
                    _cacheFood.OnFadeFood(); // Hien thi bong mo
                }

            }
        }
        if(Input.GetMouseButtonUp(0) && _hasDrag)
        {
            _imgFoodDrag.gameObject.SetActive(false);
            //_currentFood.OnActiveFood(true);
            //_currentFood = null;
            _hasDrag = false;
            if(_cacheFood != null && _cacheFood != _currentFood && !_cacheFood.HasFood)
            {
                _cacheFood.OnSetSlot(_imgFoodDrag.sprite);
                _cacheFood.OnShowNormal();
                _currentFood.OnHideFood(); // Xoa o cu
            }
            else
            {
                _currentFood.OnShowNormal();
                if (_cacheFood != _currentFood && _cacheFood != null)
                    _cacheFood.OnHideFood();
            }
        }
    }
}
