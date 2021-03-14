using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationTextFile
{
    public string language;
    public LocalizedTextEntry[] entries;
}

public class LocalizedTextEntry
{
    public string id, text;
}