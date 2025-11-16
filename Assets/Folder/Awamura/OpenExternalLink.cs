using UnityEngine;

public class OpenExternalLink : MonoBehaviour
{
    private const string url = "https://geta.iput.club/cp/wp-content/uploads/2025/11/MonkeyBook.html";

    public void OpenLink()
    {
        Application.OpenURL(url);
    }
}
