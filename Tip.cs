using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tip : MonoBehaviour
{
    public string[] text;
    public Text tiptext;

    private float randomnum;
    // Start is called before the first frame update
    void Start()
    {
        tiptext.text = text[Random.Range(0, text.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
