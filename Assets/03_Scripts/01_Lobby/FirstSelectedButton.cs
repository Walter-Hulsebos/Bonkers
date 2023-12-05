using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectButtonFirst : MonoBehaviour
{
    [SerializeField] 
    private GameObject firstSelectedButton;
    [SerializeField]
    private GameObject backButton;
    [SerializeField]
    private GameObject optionsButton;

    [SerializeField]
    private GameObject StartingScreen;

    //void Start()
    //{
    //    // Check if a button is specified as the first selected button
    //    if (firstSelectedButton != null && !StartingScreen)
    //    {
    //        // Set the specified button as the first selected
    //        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    //    }
    //    else
    //    {
    //        // If no button is specified, try to find a button in the scene and set it as the first selected
    //        GameObject defaultButton = FindDefaultButton();
    //        if (defaultButton != null)
    //        {
    //            EventSystem.current.SetSelectedGameObject(defaultButton);
    //        }
    //    }
    //}

    //GameObject FindDefaultButton()
    //{
    //    // Try to find a button in the scene
    //    Button[] buttons = FindObjectsOfType<Button>();
    //    if (buttons.Length > 0)
    //    {
    //        return buttons[0].gameObject;
    //    }

    //    return null;
    //}

    public void SetSelectedFirst()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
    
    public void SetSelectedBack()
    {
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void SetSelectedOptions() 
    {
        EventSystem.current.SetSelectedGameObject(optionsButton);
    }
}
