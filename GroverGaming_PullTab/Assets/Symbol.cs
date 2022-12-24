using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    public SymbolWeight GetSymbolWeight { get; private set; }


    public void SetSymbol(SymbolWeight symbol)
    {
        GetSymbolWeight = symbol;
        GetComponent<Image>().sprite = symbol._symbol;
    }
}
