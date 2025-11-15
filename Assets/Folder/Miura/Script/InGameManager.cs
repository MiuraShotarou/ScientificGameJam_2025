using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InGameManager : MonoBehaviour
{
    [Header("制限時間")]
    [SerializeField] float _limitTime;
    private float _timer = 0f;
    
    [SerializeField, Header("ウイルス浄化時間間隔"), Range(1, 3)] float _virusClearDuration;
    [SerializeField] private Container _container;
    [SerializeField] private GameObject _gameOverUI;

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > _limitTime)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        _gameOverUI.SetActive(true);

        Animator anim = _gameOverUI.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play("ActiveGameOver");
        }
        else
        {
            Debug.LogWarning("GameOverUI に Animator がついてません！");
        }
        _container.gameObject.SetActive(true);
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
}
