using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class License
{
    public string Licensee { get; set; }
    public DateTime Expiration { get; set; }

    public License()
    {
        Licensee = "Unlicensed";
        Expiration = LicenseManager.Get.initDate.initTimestamp.AddDays(-1);
    }

    public License(string licensee, DateTime expiration)
    {
        Licensee = licensee;
        Expiration = expiration;
    }
}
