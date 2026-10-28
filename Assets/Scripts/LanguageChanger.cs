using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageChanger : MonoBehaviour
{
    [Header("Başlangıçta Ayarlanacak Objeler")]
    public GameObject object1; 
    public GameObject object2; 

    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        RefreshActiveObjects();
    }

    public void ChangeLanguage(int languageIndex)
    {
        StartCoroutine(SetLanguageProcess(languageIndex));
    }

    IEnumerator SetLanguageProcess(int index)
    {
        yield return LocalizationSettings.InitializationOperation;
        
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        yield return null; 
        
        RefreshActiveObjects();
    }

    private void RefreshActiveObjects()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code;

        if (object1 == null || object2 == null) return;
        
        bool isTurkish = code.Contains("tr");

        object1.SetActive(isTurkish);
        object2.SetActive(!isTurkish);
    }
}