using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [Header("制限時間")]
    [SerializeField] float _limitTime;
    private float _timer = 0f;
    
    [SerializeField] private Container _container;
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
        _container.gameObject.SetActive(true);
        _container._gameOverUI.GetComponent<Animator>().Play("ActiveGameOver");
        
    }
}
