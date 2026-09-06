using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public interface IScreenEffectView
{
    void Flash(Color color, float duration);
    void CancelFlash();
}

public class ScreenEffectView : MonoBehaviour, IScreenEffectView
{
    [SerializeField] private Image _flashImage;

    private void Awake()
    {
        SetColor(Color.white, 0f);
    }

    public void Flash(Color color, float duration)
    {
        if (_flashImage == null) return;

        _flashImage.DOKill();
        SetColor(color, 0f);
        _flashImage.DOFade(1f, duration).SetEase(Ease.InQuad)
            .OnComplete(() => SetAlpha(0f));
    }

    public void CancelFlash()
    {
        if (_flashImage == null) return;

        _flashImage.DOKill();
        SetAlpha(0f);
    }

    private void SetColor(Color color, float alpha)
    {
        color.a = alpha;
        _flashImage.color = color;
    }

    private void SetAlpha(float alpha)
    {
        Color c = _flashImage.color;
        c.a = alpha;
        _flashImage.color = c;
    }
}
