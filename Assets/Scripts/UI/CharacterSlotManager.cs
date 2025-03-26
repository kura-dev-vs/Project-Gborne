using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    /// <summary>
    /// メインUI上に表示されるPT内のキャラクター情報。以下を表示
    /// 現在アクティブかどうか(未実装)
    /// キャラクターアイコン
    /// 名前
    /// PT内での順番
    /// </summary>
    public class CharacterSlotManager : MonoBehaviour
    {
        public GameObject activeSymbol;
        public Image face_Image;
        public Image face_Frame;
        public TextMeshProUGUI name_Character;
        public TextMeshProUGUI index;
        public void SetInformation(int ptIndex, int characterID)
        {
            PlayableCharacter pc = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(characterID);
            face_Image.sprite = pc.faceIcon;
            name_Character.SetText(pc.characterName);
            index.SetText((ptIndex + 1).ToString());
        }
    }
}
