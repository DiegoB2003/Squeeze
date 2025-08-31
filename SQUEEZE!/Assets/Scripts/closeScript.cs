using UnityEngine;

public class ScrollViewController : MonoBehaviour
{
    public GameObject scrollView;

    public void CloseScrollView()
    {
        scrollView.SetActive(false);
    }
}
