using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
public class SetLocalisation : MonoBehaviour
{
    private bool active = false;
    [SerializeField] V2_Toggle[] dispayLanguage;
    public void ChangeLocale(int localeID)
    {
        if (active) {return; }
        active = true;
        for (int i = 0; i < dispayLanguage.Length; i++)
        {
            if (localeID == i) dispayLanguage[i].SetStateExternal(true);
            else dispayLanguage[i].SetStateExternal(false);
        }
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int localeID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        active = false;

    }


}
