using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject _content;
    [SerializeField] RectTransform _endPoint;
    [SerializeField] RectTransform _resetPoint;
    [SerializeField] RectTransform _stopPoint;
    [SerializeField] List<SymbolWeight> _symbols;

    [Header("Rotation")]
    [SerializeField] float _speed;
    [SerializeField] float _spinTime;
    float start;
    float reset;
    int stop;

    private void Awake()
    {
        start = _endPoint.localPosition.y;
        reset = _resetPoint.localPosition.y;
        stop = (int)_stopPoint.localPosition.y;
    }

    public void Spin() => StartCoroutine(SpinCoroutine());

    IEnumerator SpinCoroutine()
    {
        float time = Time.time + _spinTime;
        List<RectTransform> children = new List<RectTransform>();

        for (int i = 1; i < _content.transform.childCount - 1; i++)
            children.Add(_content.transform.GetChild(i).gameObject.GetComponent<RectTransform>());


        while (Time.time < time || (int)_content.transform.GetChild(1).gameObject.GetComponent<RectTransform>().localPosition.y != stop)
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].localPosition += new Vector3(0, _speed, 0);

                if (children[i].localPosition.y > start)
                {
                    children[i].localPosition = new Vector3(children[i].localPosition.x, reset, 0);
                    children[i].transform.SetSiblingIndex(9);
                }
            }

            yield return null;
        }

        //FOR TESTING
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
    }
}
