using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] [Tooltip("Parent object holding all slots")] 
    GameObject _content;

    [SerializeField] [Tooltip("Where the wheel needs to start looping")]
    RectTransform _endPoint;

    [SerializeField] [Tooltip("Where the panel will loop back to")] 
    RectTransform _resetPoint; 

    [SerializeField] [Tooltip("To stop the wheel in view and keep all 3 on track")]
    RectTransform _stopPoint;

    [SerializeField] [Tooltip("All possible symbols to pick")]
    List<SymbolWeight> _symbols;

    [Header("Rotation")]
    [SerializeField] [Tooltip("How fast the bars will spin")]
    float _speed;
    [SerializeField] [Tooltip("How long the bars will spin")]
    float _spinTime;

    float start;
    float reset;
    int stop;

    private void Awake()
    {
        //Keep track of the y postion since object only move up
        start = _endPoint.localPosition.y;
        reset = _resetPoint.localPosition.y;
        stop = (int)_stopPoint.localPosition.y; //Round off to avoid floating nightmare
        _content.GetComponent<VerticalLayoutGroup>().enabled = true;
    }

    public void Spin() => StartCoroutine(SpinCoroutine());

    IEnumerator SpinCoroutine()
    {
        _content.GetComponent<VerticalLayoutGroup>().enabled = false;
        float time = Time.time + _spinTime;
        List<RectTransform> children = new List<RectTransform>();

        for (int i = 1; i < _content.transform.childCount - 1; i++)
            children.Add(_content.transform.GetChild(i).gameObject.GetComponent<RectTransform>());

        while (Time.time < time)
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].localPosition += new Vector3(0, _speed * Time.fixedDeltaTime, 0);

                if (children[i].localPosition.y > start)
                {
                    children[i].localPosition = new Vector3(children[i].localPosition.x, reset, 0);
                    children[i].transform.SetSiblingIndex(9);
                }
            }

            yield return null;
        }

        //Set symbols based on weighed randomness
        List<int> rand = new List<int>();
        List<SymbolWeight> symbols = new List<SymbolWeight>(_symbols);

        for(int i = 1; i < _content.transform.childCount - 1; i++)
        {
            for (int j = 0; j < symbols.Count; j++)
                rand.Add(symbols[j]._weight);

            int pick = GameManager.Instance.GetRandomWeightedIndex(rand);
            _content.transform.GetChild(i).gameObject.GetComponent<Symbol>().SetSymbol(symbols[pick]);

            symbols.RemoveAt(pick);
            rand.Clear();
        }

        symbols.Clear();
        GameManager.Instance.Reset();

        _content.GetComponent<VerticalLayoutGroup>().enabled = true;
    }
}
