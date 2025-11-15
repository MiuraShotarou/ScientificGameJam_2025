using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [Header("制限時間")]
    [SerializeField] float _limitTime;
    private float _timer = 0f;

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
    }
}
