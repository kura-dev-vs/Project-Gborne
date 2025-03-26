using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    public class PCDetailsManager : MonoBehaviour
    {
        private PlayerControls playerControls;
        [SerializeField] GameObject ui3DInstantiationParent;
        [SerializeField] GameObject pc3DModelObject;
        [SerializeField] TextMeshProUGUI pcName;
        [SerializeField] TextMeshProUGUI level;
        [SerializeField] TextMeshProUGUI physicalAbs, magicAbs, fireAbs, lightAbs, holyAbs;
        public RectTransform defaultRect;
        Quaternion defaultRotation;
        Vector2 defaultMin, defaultMax;
        float pcZoom;
        Vector2 pcCamera_Input;
        public bool canCameraMove = false;

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerCamera.PCInfoCameraMove.performed += i => pcCamera_Input = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.CameraZoom.performed += i => pcZoom = i.ReadValue<float>();

            }
            playerControls.Enable();
        }
        private void OnDisable()
        {
            playerControls.Disable();
        }
        private void Update()
        {
            HandlePCInfoModelZoom();
            HandlePCInfoModelMove();
        }

        private void HandlePCInfoModelZoom()
        {
            if (pcZoom > 0.8)
            {
                if (PlayerCamera.instance.ui3DCamera.fieldOfView > 10)
                    PlayerCamera.instance.ui3DCamera.fieldOfView -= 5;
                else
                    PlayerCamera.instance.ui3DCamera.fieldOfView = 10;
            }
            else if (pcZoom < -0.8)
            {

                if (PlayerCamera.instance.ui3DCamera.fieldOfView < 50)
                    PlayerCamera.instance.ui3DCamera.fieldOfView += 5;
                else
                    PlayerCamera.instance.ui3DCamera.fieldOfView = 50;
            }
            pcZoom = 0;
        }

        private void HandlePCInfoModelMove()
        {
            if (canCameraMove)
            {
                ui3DInstantiationParent.transform.Rotate(0, -pcCamera_Input[0] / 2, 0);

                Transform parent = ui3DInstantiationParent.GetComponentInChildren<Transform>();

                parent.localPosition += new Vector3(0, pcCamera_Input[1] * 10, 0);

                // 回転
                if (parent.localPosition.y > 4500)
                {
                    parent.localPosition = new Vector3(parent.localPosition.x, 4500, parent.localPosition.z);
                }

                if (parent.localPosition.y < -1700)
                {
                    parent.localPosition = new Vector3(parent.localPosition.x, -1700, parent.localPosition.z);
                }

                pcCamera_Input[0] = 0;
                pcCamera_Input[1] = 0;
            }
        }

        public void SetDetails(PlayableCharacter pc)
        {
            Transform root = ui3DInstantiationParent.transform;
            foreach (Transform child in root)
            {
                //自分の子供をDestroyする
                Destroy(child.gameObject);
            }

            if (pc.pcUI3D != null)
            {
                pc3DModelObject = Instantiate(pc.pcUI3D, Vector3.zero, Quaternion.identity, root);
                pc3DModelObject.GetComponent<UI3DManager>().Transparent();
                pc3DModelObject.transform.localPosition = new Vector3(0, 0, 0);
                pc3DModelObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                WorldPlayableCharacterDatabase.instance.SpawnUIVFX(WorldPlayableCharacterDatabase.instance.uiSpawnEffect, pc3DModelObject);
            }
            pcName.SetText(pc.characterName);
            level.SetText("Lv. " + pc.characterLv);
            physicalAbs.SetText(pc.finalPhysicalAbs.ToString());
            magicAbs.SetText(pc.finalMagicAbs.ToString());
            fireAbs.SetText(pc.finalFireAbs.ToString());
            lightAbs.SetText(pc.finalLightningAbs.ToString());
            holyAbs.SetText(pc.finalHolyAbs.ToString());
        }
        public void RefreshRectTransform()
        {
            if (defaultRect == null)
            {
                defaultRect = ui3DInstantiationParent.GetComponent<RectTransform>();
                defaultRotation = defaultRect.rotation;
                defaultMin = defaultRect.offsetMin;
                defaultMax = defaultRect.offsetMax;
            }
            ui3DInstantiationParent.GetComponent<RectTransform>().rotation = defaultRotation;
            ui3DInstantiationParent.GetComponent<RectTransform>().offsetMin = defaultMin;
            ui3DInstantiationParent.GetComponent<RectTransform>().offsetMax = defaultMax;
        }
        public void CloseUI()
        {
            RefreshRectTransform();
        }
        public void OnButtonDown()
        {
            canCameraMove = true;
        }

        public void OnButtonUp()
        {
            canCameraMove = false;
        }
    }
}
