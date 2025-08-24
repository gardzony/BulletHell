using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static G;

public class CardUI : MonoBehaviour
{
    public int CardID;

    [SerializeField] private float animationSpeed = 0.25f;
    [SerializeField] private CanvasGroup bodyAlphaGroup;
    [SerializeField] private RectTransform bodyTransform;
    [SerializeField] private Button button;
    [SerializeField] private Image cardIcon;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image lockButtonImage;
    [SerializeField] private Sprite lockCloseIcon;
    [SerializeField] private Sprite lockOpenIcon;

    public bool IsLocked = false;
    private int _price;
    private Sequence _animation;
    private Vector2 _targetBodyPosition;
    private Vector2 _startShift;

    private void Awake()
    {
        bodyAlphaGroup.alpha = 0;
        _targetBodyPosition = bodyTransform.anchoredPosition;
        _startShift = new Vector2(_targetBodyPosition.x, -Screen.height / 2);
    }

    private void Start()
    {
        Shop.Instance.OnMoneyChange += CanBuy;
    }

    public YieldInstruction Show()
    {
        KillCurrentAnimationIsActive();
        _animation = DOTween.Sequence();

        return _animation
            .Append(bodyAlphaGroup.DOFade(1, animationSpeed).From(0))
            .Join(bodyTransform.DOAnchorPos(_targetBodyPosition, animationSpeed).From(_startShift))
            .Join(bodyTransform.DORotate(new Vector3(0, 360, 0), animationSpeed, RotateMode.FastBeyond360))
            .Append(button.transform.DOScale(1, animationSpeed/2f).From(0).SetEase(Ease.OutBounce))
            .SetUpdate(true)
            .WaitForCompletion();
    }

    public YieldInstruction Hide()
    {
        KillCurrentAnimationIsActive();
        _animation = DOTween.Sequence();

        return _animation
            .Append(transform.DOScale(0, animationSpeed).From())
            .WaitForCompletion();
    }

    private void KillCurrentAnimationIsActive()
    {
        if (_animation != null && _animation.active)
        {
            _animation.Complete(true);
        }
    }

    public void SetCardAttributes(Sprite icon, string name, string description, int price, Rarity rarity)
    {
        GetComponent<Image>().color = GetRarityColor(rarity);
        cardIcon.sprite = icon;
        cardName.text = name;
        _price = price;
        priceText.text = _price.ToString();
        descriptionText.text = description;
        CanBuy();
    }

    public void CanBuy()
    {
        if (Shop.Instance.PlayerMoney < _price) button.interactable = false;
        else button.interactable = true;
        Debug.Log("Check");
    }

    public void Buy()
    {
        if (Shop.Instance.BuyItem(CardID))
        {
            if(IsLocked) ChangeLockState();
            Hide();
        }
    }

    public void ChangeLockState()
    {
        if(IsLocked)
        {
            IsLocked = false;
            lockButtonImage.sprite = lockOpenIcon;
        } else
        {
            IsLocked = true;
            lockButtonImage.sprite = lockCloseIcon;
        }
    }
}
