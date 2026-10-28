using UnityEngine;

public class MenuToggle : MonoBehaviour
{
    [Header("Bağlantılar")]
    public GameObject menuPanel; 

    public void ToggleMenu()
    {
        if (menuPanel == null) 
        {
            Debug.LogWarning("MenuToggle: Panel atanmamış! Lütfen Inspector'dan atayın.");
            return;
        }

        menuPanel.SetActive(!menuPanel.activeSelf);
    }
}