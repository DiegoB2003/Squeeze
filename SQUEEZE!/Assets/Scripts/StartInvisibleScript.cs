using UnityEngine;

public class StartInvisibleScript : MonoBehaviour
{
    public GameObject targetObject; //Assigns target object to turn on

    void Start()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }
}
