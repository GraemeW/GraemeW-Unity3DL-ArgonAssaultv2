using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITextFadeIn : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI = null;
    bool fadeInComplete = false;
    float fadeInStep = 1f;

    private void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.alpha = 0f;
    }

    private void FixedUpdate()
    {
        if (!fadeInComplete)
        {
            FadeIn();
        }
    }

    private void FadeIn()
    {
        textMeshProUGUI.alpha += fadeInStep * Time.deltaTime;
        if (Mathf.Approximately(textMeshProUGUI.alpha, 255f))
        {
            fadeInComplete = true;
        }
    }
}
