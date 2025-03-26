using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    /// <summary>
    /// hpやスタミナのui管理
    /// </summary>
    public class UI_StatBar : MonoBehaviour
    {
        private Slider slider;
        private RectTransform rectTransform;
        [Header("Bar Options")]
        [SerializeField] protected bool scaleBarLengthWithStats = true;
        [SerializeField] protected float widthScaleMultiplier = 1;
        [SerializeField] private TextMeshProUGUI healthText;

        protected virtual void Awake()
        {
            slider = GetComponentInChildren<Slider>();
            rectTransform = GetComponent<RectTransform>();
        }
        public virtual void SetStat(int newValue)
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(int maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;
            if (scaleBarLengthWithStats)
            {
                rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
                // バーの位置をリセット
                PlayerUIManager.instance.playerUIHudManager.RefreshHUD();
            }

        }
        public virtual void SetHealthStat(int newValue)
        {
            slider.value = newValue;
            healthText.SetText(newValue + "/" + slider.maxValue);
        }

        public virtual void SetMaxHealthStat(int maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;
            if (scaleBarLengthWithStats)
            {
                rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
                // バーの位置をリセット
                PlayerUIManager.instance.playerUIHudManager.RefreshHUD();
            }
            healthText.SetText(maxValue + "/" + slider.maxValue);
        }

    }
}
