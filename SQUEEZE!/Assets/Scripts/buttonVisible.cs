using UnityEngine;

public class buttonVisible : MonoBehaviour
{
    [SerializeField] private GameObject panelToHide;

    public void HidePanel()
    {
        if (panelToHide != null)
        {
            panelToHide.SetActive(false);
        }
    }
}
