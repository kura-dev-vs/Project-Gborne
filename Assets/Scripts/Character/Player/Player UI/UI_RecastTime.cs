using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace RK
{
    /// <summary>
    /// skillやburstのリキャストタイムをuiで表示
    /// </summary>
    public class UI_RecastTime : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI recastTime;
        public void SetEnabledRecast(float newTime)
        {
            recastTime.enabled = true;
            recastTime.SetText(newTime.ToString("N1"));
        }
        public void SetDisableRecast()
        {
            recastTime.enabled = false;
        }


    }
}
