using UnityEngine;

public class GraphVisibility : MonoBehaviour
{
    public GameObject targetObject; //Assigns target object to turn on
    public GameObject targetObject2; //Assigns target object to turn off

    public void ToggleObjects()
    {
        targetObject.transform.SetAsLastSibling();
        targetObject.SetActive(true);
        targetObject2.SetActive(false);
    }
}
