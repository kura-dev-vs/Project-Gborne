using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    public class BuffCardUI : MonoBehaviour
    {
        public int buffCardID = 0;
        public Image buffCardIcon;
        public TextMeshProUGUI descriptionText;
        public StaticCharacterEffect effect;

        private void Start()
        {
            GetComponent<Toggle>().group = GetComponentInParent<ToggleGroup>();
        }

        public void OnClick()
        {

        }
        public void OnValueChanged()
        {
            if (GetComponent<Toggle>().isOn)
            { }
        }
        public void SetbuffCardInfo(int id)
        {
            effect = WorldCharacterEffectsManager.instance.GetStaticEffectByID(id);
            buffCardID = effect.staticEffectID;
            descriptionText.SetText(effect.effectDescription);
            //Debug.Log(effect.effectDescription);
        }
        public void AffectBufferCard()
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            effect.ProcessStaticEffect(player);
        }
    }
}
