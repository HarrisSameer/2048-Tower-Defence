using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMeshPro;
    [SerializeField] private float _fadeTime = 1f;

    public void Init(string text, Vector2 position)
    {
        _textMeshPro.text = text;
        transform.position = position;
        FadeOut();
    }

    private void FadeOut()
    {
        _textMeshPro.DOFade(0, _fadeTime).OnComplete(() => Destroy(gameObject));
    }
}
