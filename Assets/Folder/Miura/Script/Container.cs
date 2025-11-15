using UnityEngine;
/// <summary> ゲームオブジェクトのみを格納するスクリプト</summary>
public class Container : MonoBehaviour
{
    [Header("Effect")]
    public GameObject VirusEffect;
    public GameObject[] VirusEffectChildren;
    [Header("UI")] 
    public GameObject GameClearUI;
    public GameObject GameOverUI;
    
}
