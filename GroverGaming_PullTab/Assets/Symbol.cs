using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    public SymbolWeight symbolWeight;

    public void SetSymbol(SymbolWeight symbol)
    {
        symbolWeight = symbol;
        GetComponent<Image>().color = symbol._color;
        GetComponent<Image>().sprite = symbol._symbol;
    }
}
