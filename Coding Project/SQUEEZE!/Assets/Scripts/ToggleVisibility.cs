using UnityEngine;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject targetObject; //Assigns target object to turn on/off

    void Start()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    public void ToggleObject()
    {
        if (targetObject != null)
        {
            targetObject.transform.SetAsLastSibling();
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}
