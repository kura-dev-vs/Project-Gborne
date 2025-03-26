using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    /*
    feceIcon (clone) に必要なコンポーネント
    @param faceIcon: UIに表示する画像
    @param alreadyJoinedSymbol: PTに加入済を示すGameObject
    @param nemberText: PT内の順番 (alreadyJoinedSymbol内のTextで表示) 
    @param numberInPt: PT内の順番 (加入していない場合"-1")
    */
    public class TransformOfFaceIcon : MonoBehaviour
    {
        public Image faceIcon;
        [HideInInspector] public PlayableCharacter pc;
        public GameObject alreadyJoinedSymbol;
        [SerializeField] TextMeshProUGUI numberText;
        [HideInInspector] public int numberInPT;    // このPCがPTの何番目に入っているか (いない場合は-1)
        public void SetNumberInPT(int number)
        {
            numberText.SetText((number + 1).ToString());
        }

        /// <summary>
        /// アイコンを押した際の挙動。
        /// numberInPTが-1の場合はCheckPTJoinedの値によって仮加入させる
        /// 既に加入している場合は仮除外する
        /// これらの挙動は仮加入の段階で、CurrentPTUIのDeployedPTを押してから正式にPTの入れ替えが行われる
        /// </summary>
        public void ClickFaceIcon()
        {
            if (numberInPT == -1)
            {
                // PT内に空きがあるかをNoPlayerのPCIDを渡して確認 (-1の場合空きがない)
                int result = PlayerUIManager.instance.playerUISelectableCharacterManager.CheckPTJoined(WorldPlayableCharacterDatabase.instance.NoCharacter.pcID);
                if (result != -1)
                {
                    PlayerUIManager.instance.playerUISelectableCharacterManager.ChangePTCharacter(result, pc.pcID);
                    SetNumberInPT(result);
                    alreadyJoinedSymbol.SetActive(true);
                    numberInPT = result;
                }
            }
            else
            {
                alreadyJoinedSymbol.SetActive(false);
                PlayerUIManager.instance.playerUISelectableCharacterManager.ChangePTCharacter(numberInPT, WorldPlayableCharacterDatabase.instance.NoCharacter.pcID);
                numberInPT = -1;
            }
            PlayerUIManager.instance.playerUISelectableCharacterManager.ComparisonPT();
        }

        public void ActiveJoinedSymbol()
        {
            alreadyJoinedSymbol.SetActive(true);
        }
    }
}
