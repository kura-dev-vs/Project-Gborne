using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    public class PCInfoIcon : MonoBehaviour
    {
        public Image faceIcon;
        [HideInInspector] public PlayableCharacter pc;
        public GameObject alreadyJoinedSymbol;

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
                PlayerUIManager.instance.playerUIPCInfoManager.SelectedCurrentPC(pc);
        }

        public void ActiveJoinedSymbol()
        {
            alreadyJoinedSymbol.SetActive(true);
        }

    }
}
