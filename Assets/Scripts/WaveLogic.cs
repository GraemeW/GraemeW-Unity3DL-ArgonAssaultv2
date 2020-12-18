﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveLogic : MonoBehaviour
{
    private void OnEnable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
