using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private Container _container;
    [Header("制限時間")]
    [SerializeField, Range(1, 100)] float _limitTime;
    [Header("ウイルス減少量")]
    [SerializeField] float _reduceVirusValue;
    
    float _virusActivePercentage = 0.7f; // 制限時間の何パーセントから色を濃くしていくのか
    float _virusClearDuration = 3; //ゲームクリア時のウイルス浄化時間間隔
    
    float _timer = 0f;
    // キャッシュ
    Slider _timeLimitSlider;
    float _virusActiveValue; //ウイルスの演出が発生する閾値

    void Awake()
    {
        _timeLimitSlider = _container.TimeLimitSlider.GetComponent<Slider>();
        _timeLimitSlider.maxValue = _limitTime;
        _virusActiveValue = _limitTime * _virusActivePercentage;
    }

    void Update()
    {
        //時間関係
        _timer += Time.deltaTime;
        if (_timer > _limitTime)
        {
            GameOver();
        }
        _timeLimitSlider.value = _timer;
        
        // ウイルスエフェクトの色変更
        if (_timeLimitSlider.value > _virusActiveValue)
        {
            float _virusValuePercentage = _timeLimitSlider.value - _virusActiveValue; //ウイルスエフェクトが出現した時のAlpha値を計算 0 ~ 0.3 を1 として考えた時
            _virusValuePercentage /= (_limitTime - _virusActiveValue);
            _container.VirusEffect.GetComponent<Image>().color = ChangeAlpha(_container.VirusEffect.GetComponent<Image>().color, _virusValuePercentage);
            foreach (var icon in _container.VirusEffectChildren)
            {
                icon.GetComponent<Image>().color = ChangeAlpha(icon.GetComponent<Image>().color, _virusValuePercentage > 0.7843137254901961f? 0.7843137254901961f : _virusValuePercentage);
            }
        }
    }

    void GameOver()
    {
        _container.GameOverUI.SetActive(true);
        _container.GameOverUI.GetComponent<Animator>().Play("ActiveGameOver");
    }
    
    public void GameClear()
    {
        foreach (var icon in _container.VirusEffectChildren)
        {
            icon.GetComponent<Image>().DOFade(0, _virusClearDuration);
        }
        _container.VirusEffect.GetComponent<Image>().DOFade(0, _virusClearDuration)
        .OnComplete(() => {_container.GameClearUI.GetComponent<Animator>().Play("GameClear");});
    }

    public void ReduceVirus()
    {
        _timer -= _reduceVirusValue;
        Debug.Log("ReduceVirus");
    }
    /// <summary>
    /// Alphaだけを変更させる便利ツール
    /// </summary>
    Color ChangeAlpha(Color color, float alpha) => new (color.r, color.g, color.b, alpha);
}
