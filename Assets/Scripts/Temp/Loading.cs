using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using KC_Custom;

public class Loading : MonobehaviourSingleton<Loading>
{
    [SerializeField] private GameObject m_group = null;

    [SerializeField] private TextMeshProUGUI m_loadingText = null;

    // =======================================================

    protected override void Start()
    {
        base.Start();
        this.UIStatus(false);
    }

    // =======================================================

    public void UIStatus(bool status)
    {
        this.m_group.SetActive(status);
    }

    public void UpdateText(string text)
    {
        this.m_loadingText.text = text;
    }

    // =======================================================
}
