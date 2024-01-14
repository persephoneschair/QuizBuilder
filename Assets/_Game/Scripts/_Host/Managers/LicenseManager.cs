using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;

public class LicenseManager : SingletonMonoBehaviour<LicenseManager>
{
    private bool invalidLicense;
    public bool InvalidLicense
    {
        get
        {
            return invalidLicense;
        }
        set
        {
            invalidLicense = value;
            if(value)
                LicenseIsExpired();
            else
                LicenseIsValid();
        }
    }

    private License currentLicense;
    public License CurrentLicense
    {
        get
        {
            return currentLicense;
        }
        set
        {
            currentLicense = value;
            InvalidLicense = CheckLicenseExpiration(value);
        }
    }

    public DateTimeModel initDate;
    public GameObject licensePopUp;
    public TMP_InputField licenseField;
    public TextMeshProUGUI licenseMesh;
    public Button okButton;
    public string dateTimeUri;

    private string licensedBlurb =
        "This product is licensed to <color=green>{0}</color>.\n\n" +
        "<color=orange>THE CURRENT LICENSE WILL EXPIRE ON {1}.</color>\n\n" +
        "To extend your license now, please click below and enter a valid license code.\n\n" +
        "If you do not have a valid license code, please contact <color=yellow>ben@persephoneschair.com</color> to purchase one.";
    private string expiredLicenseBlurb =
        "This product is licensed to <color=green>{0}</color>.\n\n" +
        "<color=red>THE CURRENT LICENSE EXPIRED ON {1}.</color>\n\n" +
        "To extend your license now, please click below and enter a valid license code.\n\n" +
        "If you do not have a valid license code, please contact <color=yellow>ben@persephoneschair.com</color> to purchase one.";
    private string unlicensedBlurb =
        "<color=red>THIS PRODUCT IS CURRENTLY UNLICENSED.</color=red>\n\n" +
        "If you have purchased a license, please click below and enter a valid license code.\n\n" +
        "If you do not have a valid license code, please contact <color=yellow>ben@persephoneschair.com</color> to purchase one.";


    public void InitialiseLicense()
    {
        //Returns an independent datetime; prevents user abusing licensing by changing system clock
        StartCoroutine(GetDateTime(dateTimeUri));
    }

    IEnumerator GetDateTime(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            var data = webRequest.downloadHandler.text;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    //Some placeholder screen if no internet connection
                    //Yield break out of this to prevent further code being executed
                    DebugLog.Print("CONNECTION ERROR - PLEASE RELOAD AND TRY AGAIN", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Red);
                    break;
                case UnityWebRequest.Result.Success:
                    initDate = new DateTimeModel();
                    JsonConvert.PopulateObject(data, initDate);
                    if (DateTime.TryParse(initDate.datetime, out DateTime date))
                        initDate.initTimestamp = date;
                    else
                        //If something goes awry with the downloaded datetime model, put system clock in as backup
                        initDate.initTimestamp = DateTime.Now;
                    break;
            }
        }
        FinishGetLicense();
    }

    public void FinishGetLicense()
    {
        if (File.Exists(StorageManager.GetDataPath() + @"/License.json"))
            DecryptLicense(File.ReadAllText(StorageManager.GetDataPath() + @"/License.json"));
        else
            CurrentLicense = new License();
    }

    public void DecryptLicense(string license)
    {
        if (!Encryption.DataIsSafe(license))
            CurrentLicense = new License();

        else
        {
            string dec = Encryption.DecryptData(license);
            var lic = JsonConvert.DeserializeObject<License>(dec);
            if (lic != null)
            {
                CurrentLicense = new License(lic.Licensee, lic.Expiration);
                File.WriteAllText(StorageManager.GetDataPath() + @"/License.json", Encryption.EncryptData(JsonConvert.SerializeObject(CurrentLicense, Formatting.Indented)));
            }
            else
                CurrentLicense = new License();
        }
    }

    public bool CheckLicenseExpiration(License l)
    {
        return l.Expiration <= initDate.initTimestamp;
    }

    public void LicenseIsValid()
    {
        licenseMesh.text = string.Format(licensedBlurb, CurrentLicense.Licensee, CurrentLicense.Expiration.ToShortDateString());
        MainMenuManager.Get.LicensedProduct();
        OnCancel();
    }

    public void LicenseIsExpired()
    {
        licenseMesh.text = CurrentLicense.Licensee == "Unlicensed" ? unlicensedBlurb : string.Format(expiredLicenseBlurb, CurrentLicense.Licensee, CurrentLicense.Expiration.ToShortDateString());
        MainMenuManager.Get.UnlicensedProduct();
        OnCancel();
    }

    public void OnEnterLicenseCode()
    {
        licensePopUp.SetActive(true);
        licenseField.ActivateInputField();
    }
    public void Update()
    {
        if (licenseField.text.Length < 1)
            okButton.interactable = false;
        else
            okButton.interactable = true;
    }

    public void OnOK()
    {
        if (Encryption.DataIsSafe(licenseField.text))
            DecryptLicense(licenseField.text);
        else
            licenseField.text = "";
    }

    public void OnCancel()
    {
        licenseField.text = "";
        licensePopUp.SetActive(false);
    }

    [Header ("License Generation")]
    public string licenseeToGenerate;
    public UDateTime expirationDateToGenerate;

    [TextArea (3,5)] public string encryptedLicense;

    [Button]
    public void GenerateLicense()
    {
        encryptedLicense = Encryption.EncryptData(JsonConvert.SerializeObject(new License(licenseeToGenerate, expirationDateToGenerate)));
    }
}
