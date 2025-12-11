using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandAnimator : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private CardView[] _cards;
    [SerializeField] private OverlayView _overlay;

    [SerializeField] private RectTransform _playButtonRect;

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private RectTransform _tableRoot;

    [SerializeField] private float _zoomedSize = 4.5f; // целевой размер при зуме
    [SerializeField] private float _zoomDuration = 0.2f;

    [SerializeField] private RectTransform[] _cardCenterSlots;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clickClip;
    [SerializeField] private AudioClip _wooshClip;
    [SerializeField] private AudioClip _coinClip;

    [SerializeField] private ComboView _comboView;

    [SerializeField] private ScoreView _scoreView;
    [SerializeField] private RectTransform _scoreTextRect;
    private int _currentScore;

    [SerializeField] private GameObject _scorePopupPrefab;

    [SerializeField] private FinalScoreView _finalScoreView;
    private int _lastHandScore;

    [SerializeField] private RectTransform _scoreTracerRect;
    [SerializeField] private CanvasGroup _scoreTracerGroup;

    private float[] _cardStartRotations;
    private Vector2[] _cardStartPositions;

    private bool _isButtonAnimating;
    private bool _isSequenceRunning;

    void Awake()
    {
        _playButton.onClick.AddListener(OnPlayClicked);

        _cardStartPositions = new Vector2[_cards.Length];
        _cardStartRotations = new float[_cards.Length];

        for (int i = 0; i < _cards.Length; i++)
        {
            if (_cards[i] == null) continue;

            _cardStartPositions[i] = _cards[i].Rect.anchoredPosition;
            _cardStartRotations[i] = _cards[i].Rect.localEulerAngles.z;
        }

        _currentScore = 0;
        _scoreView.SetInstant(_currentScore);
    }

    private void OnDestroy()
    {
        _playButton.onClick.RemoveListener(OnPlayClicked);
    }

    private void OnPlayClicked()
    {
        if (_isSequenceRunning)
            return; // не даём запустить последовательность второй раз

        Debug.Log("Click");
        // SFX: клик
        if (_audioSource != null && _clickClip != null)
        {
            _audioSource.PlayOneShot(_clickClip);
        }

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        _isSequenceRunning = true;

        // Блокируем кнопку на время анимации
        _playButton.interactable = false;

        // 1. Анимация кнопки (можно не ждать до конца, если хочешь параллельно)
        if (!_isButtonAnimating)
        {
            StartCoroutine(AnimatePlayButtonScale());
        }

        StartCoroutine(ZoomTableRoot(_zoomedSize, _zoomDuration));

        // 2. Плавно затемняем интерфейс
        yield return StartCoroutine(_overlay.FadeIn(0.2f));

        // 3. Волна по картам
        yield return StartCoroutine(MoveAllCardsToCenter(0.3f));
        //yield return StartCoroutine(BounceCardsOnce());

        StartCoroutine(_comboView.Show("FLUSH", 0.2f, 2f));
        yield return StartCoroutine(ScoreCardsSequence());

        yield return StartCoroutine(MoveAllCardsToStart(0.3f));
        StartCoroutine(ZoomTableRoot(1f, _zoomDuration));
        // 4. Плавно возвращаем яркость
        yield return StartCoroutine(_overlay.FadeOut(0.2f));

        // Разблокируем кнопку
        _playButton.interactable = true;
        _isSequenceRunning = false;
    }


    private IEnumerator PlayScoreTracer(RectTransform card, float duration)
    {
        if (_scoreTracerRect == null || _scoreTracerGroup == null || _scoreTextRect == null)
            yield break;

        // 1. Старт — позиция карты (anchored в системе TableRoot)
        Vector2 startPos = card.anchoredPosition;

        // 2. Цель — позиция ScoreText, переведённая в систему координат TableRoot
        // 2.1. Мировая позиция центра ScoreText
        Vector3 scoreWorldPos = _scoreTextRect.transform.position;

        // 2.2. Локальная позиция в пространстве TableRoot
        Vector3 scoreLocalPos = _tableRoot.InverseTransformPoint(scoreWorldPos);

        // 2.3. Переводим в anchoredPosition (для простоты берём x,y из localPosition)
        Vector2 endPos = new Vector2(scoreLocalPos.x, scoreLocalPos.y);

        _scoreTracerRect.anchoredPosition = startPos;
        _scoreTracerRect.localScale = Vector3.one * 0.5f;
        _scoreTracerGroup.alpha = 1f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            t = Mathf.Clamp01(t);

            _scoreTracerRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            float scale = Mathf.Lerp(0.5f, 1.2f, t);
            _scoreTracerRect.localScale = Vector3.one * scale;

            float alpha = 1f - Mathf.Clamp01((t - 0.5f) * 2f);
            _scoreTracerGroup.alpha = alpha;

            yield return null;
        }

        _scoreTracerGroup.alpha = 0f;
    }

    private IEnumerator ScoreCardsSequence()
    {
        int[] cardScores = { 10, 15, 20, 15, 15 };
        const float cardAnimDuration = 0.25f;
        const float scoreAnimDuration = 0.25f;
        const float delayBetweenCards = 0.2f;
        const float cardJumpHeight = 10f;

        int totalHandScore = 0;

        for (int i = 0; i < _cards.Length; i++)
        {
            CardView card = _cards[i];
            if (card == null) continue;

            int add = cardScores[i];
            totalHandScore += add;

            int startScore = _currentScore;
            int endScore = _currentScore + add;

            if (_audioSource != null && _coinClip != null)
                _audioSource.PlayOneShot(_coinClip);

            StartCoroutine(card.PlayScoreAnimation(add, cardJumpHeight, cardAnimDuration));
            StartCoroutine(PlayScoreTracer(card.Rect, 0.25f));

            yield return StartCoroutine(_scoreView.AnimateScore(startScore, endScore, scoreAnimDuration));
            _currentScore = endScore;

            yield return new WaitForSeconds(delayBetweenCards);
        }

        _lastHandScore = totalHandScore;
        yield return StartCoroutine(_finalScoreView.Show(_lastHandScore, 0.6f, 0.2f));
    }

    private IEnumerator MoveAllCardsToStart(float duration)
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < _cards.Length; i++)
        {
            CardView card = _cards[i];
            if (card == null) continue;

            RectTransform rect = card.Rect;

            Vector2 endPos = _cardStartPositions[i];
            float endZ = _cardStartRotations[i];

            seq.Join(rect.DOAnchorPos(endPos, duration).SetEase(Ease.InOutQuad));
            seq.Join(rect.DOLocalRotate(new Vector3(0f, 0f, endZ), duration));
        }

        yield return seq.WaitForCompletion();
    }

    private IEnumerator MoveAllCardsToCenter(float moveDuration)
    {
        const float delayBetweenCards = 0.05f;
        const float maxRotateZ = 5f;
        const float hopHeight = 7f;
        const float hopDuration = 0.06f;

        int count = Mathf.Min(_cards.Length, _cardCenterSlots.Length);

        for (int i = 0; i < count; i++)
        {
            CardView card = _cards[i];
            RectTransform target = _cardCenterSlots[i];

            if (card != null && target != null)
            {
                if (_audioSource != null && _wooshClip != null)
                    _audioSource.PlayOneShot(_wooshClip);

                StartCoroutine(card.FlyToCenter(target, moveDuration, maxRotateZ, hopHeight, hopDuration));
            }

            yield return new WaitForSeconds(delayBetweenCards);
        }

        float totalWait = moveDuration + 2 * hopDuration + delayBetweenCards * (count - 1);
        yield return new WaitForSeconds(totalWait);
    }

    private IEnumerator ZoomTableRoot(float targetScale, float duration)
    {
        if (_tableRoot == null)
            yield break;

        yield return _tableRoot.DOScale(Vector3.one * targetScale, duration)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();
    }

    private IEnumerator AnimatePlayButtonScale()
    {
        if (_playButtonRect == null)
            yield break;

        _isButtonAnimating = true;

        Vector3 normalScale = Vector3.one;
        Vector3 pressedScale = normalScale * 0.95f;
        float duration = 0.05f;

        Sequence seq = DOTween.Sequence();
        seq.Append(_playButtonRect.DOScale(pressedScale, duration).SetEase(Ease.OutQuad));
        seq.Append(_playButtonRect.DOScale(normalScale, duration).SetEase(Ease.InQuad));

        yield return seq.WaitForCompletion();
        _isButtonAnimating = false;
    }
}
