using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopText : MonoBehaviour
{
    [SerializeField] TMP_Text HeaderText;

    public void SetHeaderText(string message)
    {
        HeaderText.text = message;
    }
}
