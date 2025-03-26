using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    /// <summary>
    /// 爆発に必要なエネルギーの状況をuiで表示
    /// sliderで充填状況を示し、マックス時はsliderの色を不透明、それ以外は半透明
    /// </summary>
    public class UI_BurstEnergy : MonoBehaviour
    {
        public Slider currentEnergy;
        [SerializeField] Image backGround;
        [SerializeField] float transparentAlpha = 0.3f;
        [SerializeField] float opacityAlpha = 1f;
        [SerializeField] Image icon;
        [SerializeField] GameObject energyEffect;

        private void Start()
        {
            currentEnergy = GetComponentInChildren<Slider>();
        }
        private void Update()
        {
            if (currentEnergy.value >= 1)
            {
                backGround.color = new Color(backGround.color.r, backGround.color.g, backGround.color.b, opacityAlpha);
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, opacityAlpha);
                energyEffect.SetActive(true);
                return;
            }
            else
            {
                backGround.color = new Color(backGround.color.r, backGround.color.g, backGround.color.b, transparentAlpha);
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, transparentAlpha);
                energyEffect.SetActive(false);
                return;
            }
        }
        public void SetEnergyValue(float newValue)
        {
            currentEnergy.value = newValue;
        }
    }
}
