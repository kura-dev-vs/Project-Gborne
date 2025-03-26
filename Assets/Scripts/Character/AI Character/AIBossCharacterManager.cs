using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// boss character用のcharacter manager
    /// このidがスポーンしたら, セーブファイルをチェック. 
    /// セーブファイルにこのidを持つボスモンスターが存在しない場合、追加.
    /// もしあればボスが倒されているかチェック. 
    /// ボスが倒されていたら、このゲームオブジェクトを無効.
    /// ボスが倒されていない場合、このオブジェクトを有効
    /// </summary>
    public class AIBossCharacterManager : AICharacterManager
    {
        public int bossID = 0;

        [Header("Music")]
        [SerializeField] AudioClip bossIntroClip;
        [SerializeField] AudioClip bossBattleLoopClip;
        [Header("Status")]
        public NetworkVariable<bool> bossFightIsActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> hasBeenAwakened = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> hasBeenDefeated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> phase02 = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        [SerializeField] List<FogWallInteractable> fogWalls;
        [SerializeField] string sleepAnimation;
        [SerializeField] string awakenAnimation;

        [Header("Phase Shift")]
        public float minimumHealthPercentageToShift = 50;
        [SerializeField] string phaseShiftAnimation = "Phase_Change_01";
        [SerializeField] CombatStanceState phase02CombatStanceState;

        [Header("States")]
        [SerializeField] BossSleepState sleepState;

        protected override void Awake()
        {
            base.Awake();
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            bossFightIsActive.OnValueChanged += OnBossFightIsActiveChanged;
            OnBossFightIsActiveChanged(false, bossFightIsActive.Value);
            phase02.OnValueChanged += OnPhase02Changed;

            if (IsOwner)
            {
                sleepState = Instantiate(sleepState);
                currentState = sleepState;
            }

            if (IsServer)
            {
                // セーブデータにこのボスの情報が含まれていない場合は追加
                if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);
                }
                // 含まれる場合このボスにすでに存在するデータをロード
                else
                {
                    hasBeenDefeated.Value = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];
                    hasBeenAwakened.Value = WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID];
                }

                // fog wallを探す
                // ボスとfog wallで同じidを共有するか、fog wallのid変数をここに置き、それを使ってロックする。
                StartCoroutine(GetFogWallsFromWorldObjectManager());

                // ボスがawaken (邂逅) している場合、fog wallを有効
                if (hasBeenAwakened.Value)
                {
                    for (int i = 0; i < fogWalls.Count; i++)
                    {
                        fogWalls[i].isActive.Value = true;
                    }
                }
                // ボスが既に倒されていた場合fog wallを無効
                if (hasBeenDefeated.Value)
                {
                    for (int i = 0; i < fogWalls.Count; i++)
                    {
                        fogWalls[i].isActive.Value = false;
                    }
                    aiCharacterNetworkManager.isActive.Value = false;
                }
            }
            if (!hasBeenAwakened.Value)
            {
                //characterAnimatorManager.PlayTargetActionAnimation(sleepAnimation, true);
                animator.Play(sleepAnimation);
            }
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            bossFightIsActive.OnValueChanged -= OnBossFightIsActiveChanged;
            phase02.OnValueChanged -= OnPhase02Changed;
        }
        /// <summary>
        /// bossのidと同じidを持つfog wallを関連付けさせる
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetFogWallsFromWorldObjectManager()
        {
            while (WorldObjectManager.instance.fogWalls.Count == 0)
                yield return new WaitForEndOfFrame();

            fogWalls = new List<FogWallInteractable>();
            foreach (var fogWall in WorldObjectManager.instance.fogWalls)
            {
                if (fogWall.fogWallID == bossID)
                    fogWalls.Add(fogWall);
            }
        }

        /// <summary>
        /// ボスが倒された時に起こすイベント
        /// </summary>
        /// <param name="manuallySelectDamageAnimation"></param>
        /// <returns></returns>
        public override IEnumerator ProcessDeathEvent(bool manuallySelectDamageAnimation = false)
        {
            PlayerUIManager.instance.playerUIPopUpManager.SendBossDefeatedPopUp("GREAT FOR FEELED");
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;
                bossFightIsActive.Value = false;
                foreach (var fogWall in fogWalls)
                {
                    fogWall.isActive.Value = false;
                }
                // reset any flags here that need to be reset
                // nothing yet

                // if we are not grounded, play an aerial death animation

                if (!manuallySelectDamageAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
                }

                hasBeenDefeated.Value = true;
                // セーブデータにボスの情報が入っていない場合追加
                if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }
                // セーブデータにボスの情報が入っている場合
                else
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Remove(bossID);
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }

                WorldSaveGameManager.instance.SaveGame();

            }

            yield return new WaitForSeconds(5);

            // sfxやvfx、実績等共通演出面は随時追加. 固有に追加するより楽
            // オブジェクトをdisableにする場合ここかOnIsDeadChangedで管理する
        }

        /// <summary>
        /// ボスを起こすメソッド
        /// 基本fogwallのtriggerで呼び出す
        /// </summary>
        public void WakeBoss()
        {
            if (IsOwner)
            {
                if (!hasBeenAwakened.Value)
                {
                    characterAnimatorManager.PlayTargetActionAnimation(awakenAnimation, true);
                }
                bossFightIsActive.Value = true;
                hasBeenAwakened.Value = true;
                currentState = idle;
                if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                }
                else
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                }

                for (int i = 0; i < fogWalls.Count; i++)
                {
                    fogWalls[i].isActive.Value = true;
                }

            }
        }
        private void OnBossFightIsActiveChanged(bool oldStatus, bool newStatus)
        {
            if (bossFightIsActive.Value)
            {
                WorldSoundFXManager.instance.PlayBossTrack(bossIntroClip, bossBattleLoopClip);

                GameObject bossHealthBar = Instantiate(PlayerUIManager.instance.playerUIHudManager.bossHealthBarObject, PlayerUIManager.instance.playerUIHudManager.bossHealthBarParent);
                UI_Boss_HP_Bar bossHPBar = bossHealthBar.GetComponentInChildren<UI_Boss_HP_Bar>();

                bossHPBar.EnableBossHPBar(this);
            }
            else
            {
                WorldSoundFXManager.instance.StopBossMusic();
            }
        }
        /// <summary>
        /// 形態変化に使用する. 
        /// </summary>
        /// <param name="oldStatus"></param>
        /// <param name="newStatus"></param>
        private void OnPhase02Changed(bool oldStatus, bool newStatus)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("phase02", phase02.Value);
        }
        public void PhaseShift()
        {
            characterAnimatorManager.PlayTargetActionAnimation(phaseShiftAnimation, true);
            combatStance = Instantiate(phase02CombatStanceState);
            currentState = combatStance;

            phase02.Value = true;
        }
    }
}
