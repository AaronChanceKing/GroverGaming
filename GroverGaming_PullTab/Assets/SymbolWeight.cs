using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "My Assets/Symbol")]
public class SymbolWeight : ScriptableObject
{
    [Header("look")]
    public Sprite _symbol;
    public Color _color;

    [Header("Value")]
    public int _weight;
    public int _muliplyer;
}
