using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    /// <summary>
    /// PlayerのUIとHudを管理する。基本的にHUDに情報を載せる場合はここに書く
    /// 予めUIのTransformをInspectorで設定しておき、PlayerUIManager.instanceからこのコンポーネントを取得して使用する
    /// </summary>
    public class PlayerUIHudManager : MonoBehaviour
    {
        [SerializeField] CanvasGroup[] canvasGroups;
        [Header("STAT BARS")]
        [SerializeField] UI_StatBar healthBar;
        [SerializeField] UI_StatBar staminaBar;

        [Header("QUICK SLOTS")]
        [SerializeField] Image rightWeaponQuickSlotIcon;
        [SerializeField] Image leftWeaponQuickSlotIcon;

        [Header("Boss Health Bar")]
        public Transform bossHealthBarParent;
        public GameObject bossHealthBarObject;

        [Header("Enemy Health Bar")]
        public Transform enemyHealthBarParent;

        [Header("Damage Texts")]
        public Transform damageTextParent;

        [Header("Skill & Burst")]
        [SerializeField] Image burstQuickSlotIcon;
        [SerializeField] Image rightSkillQuickSlotIcon;
        [SerializeField] Image leftSkillQuickSlotIcon;
        [SerializeField] UI_RecastTime recastTimeBurst;
        [SerializeField] UI_RecastTime recastTimeRight;
        [SerializeField] UI_RecastTime recastTimeLeft;
        [SerializeField] UI_BurstEnergy burstEnergy;
        [SerializeField] float transparentAlpha = 0.3f;
        [SerializeField] float opacityAlpha = 1f;

        [Header("Character Slots")]
        public Transform characterSlotParent;
        [SerializeField] GameObject characterSlotObject;

        [Header("Just Got Item Slots")]
        public Transform justGotItemSlotParent;
        [SerializeField] GameObject justGotItemInfoObject;

        [Header("Interaction Slots")]
        public Transform interactionSlotParent;
        [SerializeField] GameObject interactionMessageObject;
        public List<GameObject> currentInteractionList;

        [Header("Score")]
        public Transform scoreParent;
        public TextMeshProUGUI score;

        [Header("LockOn")]
        public GameObject lockOnCursor;

        public void ToggleHUD(bool status)
        {
            if (status)
            {
                foreach (var canvas in canvasGroups)
                {
                    canvas.alpha = 1;
                }
            }
            else
            {
                foreach (var canvas in canvasGroups)
                {
                    canvas.alpha = 0;
                }
            }
        }

        public void RefreshHUD()
        {
            healthBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(true);
            staminaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
            //lockOnCursor.SetActive(false);
        }
        public void SetNewHealthValue(int oldValue, int newValue)
        {
            healthBar.SetHealthStat(newValue);
        }
        public void SetMaxHealthValue(int maxHealth)
        {
            healthBar.SetMaxHealthStat(maxHealth);
        }
        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(Mathf.RoundToInt(newValue));
        }
        public void SetMaxStaminaValue(int maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina);
        }
        public void SetRightWeaponQuickSlotIcon(int weaponID)
        {
            // 武器のアイテムIDを要求し、データベースから武器を取得し、それを使って武器のアイテムアイコンを取得する。

            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

            if (weapon == null)
            {
                Debug.Log("ITEM IS NULL");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            if (weapon.itemIcon == null)
            {
                Debug.Log("ITEM HAS NO ICON");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            // 上記の警告をUIで出す場合ここに書く(検討中)

            rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            rightWeaponQuickSlotIcon.enabled = true;
        }
        public void SetLeftWeaponQuickSlotIcon(int weaponID)
        {
            // 武器のアイテムIDを要求し、データベースから武器を取得し、それを使って武器のアイテムアイコンを取得する。

            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

            if (weapon == null)
            {
                Debug.Log("ITEM IS NULL");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            if (weapon.itemIcon == null)
            {
                Debug.Log("ITEM HAS NO ICON");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            // 上記の警告をUIで出す場合ここに書く(検討中)

            leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            leftWeaponQuickSlotIcon.enabled = true;
        }
        public void SetSkillBurstSlotIcon(int characterID)
        {
            PlayableCharacter pc = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(characterID);
            if (pc == null)
            {
                Debug.Log("SKILL IS NULL");
                burstQuickSlotIcon.enabled = false;
                burstQuickSlotIcon.sprite = null;
                rightSkillQuickSlotIcon.enabled = false;
                rightSkillQuickSlotIcon.sprite = null;
                leftSkillQuickSlotIcon.enabled = false;
                leftSkillQuickSlotIcon.sprite = null;
                return;
            }

            if (pc.rightSkill.skillIcon == null || pc.leftSkill.skillIcon == null)
            {
                Debug.Log("SKILL HAS NO ICON");
                burstQuickSlotIcon.enabled = false;
                burstQuickSlotIcon.sprite = null;
                rightSkillQuickSlotIcon.enabled = false;
                rightSkillQuickSlotIcon.sprite = null;
                leftSkillQuickSlotIcon.enabled = false;
                leftSkillQuickSlotIcon.sprite = null;
                return;
            }

            burstQuickSlotIcon.enabled = true;
            burstQuickSlotIcon.sprite = pc.burst.burstIcon;
            rightSkillQuickSlotIcon.enabled = true;
            rightSkillQuickSlotIcon.sprite = pc.rightSkill.skillIcon;
            leftSkillQuickSlotIcon.enabled = true;
            leftSkillQuickSlotIcon.sprite = pc.leftSkill.skillIcon;
        }
        public void SetEnabledBurstRecast(float newTime)
        {
            var c = burstQuickSlotIcon.color;
            burstQuickSlotIcon.color = new Color(c.r, c.g, c.b, transparentAlpha);
            recastTimeBurst.SetEnabledRecast(newTime);
        }
        public void SetDisableBurstRecast()
        {
            var c = burstQuickSlotIcon.color;
            //burstQuickSlotIcon.color = new Color(c.r, c.g, c.b, opacityAlpha);
            recastTimeBurst.SetDisableRecast();
        }
        public void SetEnabledRightRecast(float newTime)
        {
            var c = rightSkillQuickSlotIcon.color;
            rightSkillQuickSlotIcon.color = new Color(c.r, c.g, c.b, transparentAlpha);
            recastTimeRight.SetEnabledRecast(newTime);
        }
        public void SetDisableRightRecast()
        {
            var c = rightSkillQuickSlotIcon.color;
            rightSkillQuickSlotIcon.color = new Color(c.r, c.g, c.b, opacityAlpha);
            recastTimeRight.SetDisableRecast();
        }
        public void SetEnabledLeftRecast(float newTime)
        {
            var c = leftSkillQuickSlotIcon.color;
            leftSkillQuickSlotIcon.color = new Color(c.r, c.g, c.b, transparentAlpha);
            recastTimeLeft.SetEnabledRecast(newTime);
        }
        public void SetDisableLeftRecast()
        {
            var c = leftSkillQuickSlotIcon.color;
            leftSkillQuickSlotIcon.color = new Color(c.r, c.g, c.b, opacityAlpha);
            recastTimeLeft.SetDisableRecast();
        }
        public void SetEnergyValue(float currentEnergy)
        {
            burstEnergy.currentEnergy.value = currentEnergy;
        }
        public void SetCharacterSlotUI(int ptIndex, int characterID)
        {
            var marker = Instantiate(characterSlotObject, characterSlotParent);
            marker.GetComponent<CharacterSlotManager>().SetInformation(ptIndex, characterID);
        }
        public void SetJustGotItemSlotUI(Item item, int amount)
        {
            var marker = Instantiate(justGotItemInfoObject, justGotItemSlotParent);
            marker.GetComponent<JustGotItemManager>().SetItemInformation(item, amount);
        }
        public void SetInteractionMessageSlotUI(Interactable interactable)
        {
            var marker = Instantiate(interactionMessageObject, interactionSlotParent);
            marker.GetComponent<InteractionMessage>().SetInteractionInformation(interactable);
            currentInteractionList.Add(marker);
        }
        public void RemoveInteractionMessageSlotUI(int removeNum)
        {
            Destroy(currentInteractionList[removeNum]);
            currentInteractionList.RemoveAt(removeNum);
        }
        public void ResetInteractionMessageSlotUI()
        {
            if (!(currentInteractionList.Count == 0))
            {
                for (int i = 0; i < currentInteractionList.Count; i++)
                {
                    Destroy(currentInteractionList[i]);
                    currentInteractionList.RemoveAt(i);
                }
            }
        }
        public void RefreshSelectingInteraction(int currentSelectingIndex)
        {
            for (int i = 0; i < currentInteractionList.Count; i++)
            {
                if (i == currentSelectingIndex)
                {
                    currentInteractionList[i].GetComponent<InteractionMessage>().selectingFrame.SetActive(true);
                }
                else
                {
                    currentInteractionList[i].GetComponent<InteractionMessage>().selectingFrame.SetActive(false);
                }
            }
        }
        public void SetScore(int newScore)
        {
            score.SetText(newScore.ToString());
        }
    }
}
