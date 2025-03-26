﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Mebiustos.MMD4MecanimFaciem {
    [RequireComponent(typeof(FaciemDatabase))]
    public class FaciemController : MonoBehaviour {
        // 表情設定の際にHelperに設定するOverrideWeightフラグ
        public bool OverrideWeight = true;
        // 表情設定前にHelperに設定されるSpeed値（未使用Morphの戻りスピード）
        public float ClearSpeed = 0.1f;

        [System.NonSerialized]
        public int CurrentFaceIndex = -1;
        [System.NonSerialized]
        public int DefaultFaceIndex = -1;

        protected FaciemDatabase _database;
        protected MMD4MecanimModel _model;
        protected MMD4MecanimMorphHelper[] helpers;
        protected int[] useMorphIndexs;

        bool initialized = false;
        bool IsInitializedAndEnabled {
            get {
                return initialized && this.enabled;
            }
        }

        protected virtual void Awake() {
            //Debug.Log("Awake");
            this._model = GetComponent<MMD4MecanimModel>();
            this._database = GetComponent<FaciemDatabase>();

            this.DefaultFaceIndex = this._database.GetDefaultFaceIndex();
            this.CurrentFaceIndex = this.DefaultFaceIndex;
            
            _setMyEnable();
            if (!this.enabled)
                return;

            if (this._database.FaceDataList.Length > 0) {
                if (this._model.initializeOnAwake == true) {
                    if (this._model.modelData == null)
                        this._model.Initialize();
                    Initialize();
                }
            }
        }

        protected virtual void Start() {
            //Debug.Log("Start");
        }

        protected virtual void OnEnable() {
            //Debug.Log("OnEnable");
            _setMyEnable();
            if (!this.enabled)
                return;

            if (this._database.FaceDataList.Length > 0) {
                if (!initialized) {
                    if (this._model.modelData == null)
                        this._model.Initialize();
                    Initialize();
                }
            } else {
                if (this._model.modelData == null)
                    this._model.Initialize();
                FroceSetModelWieghtZeroAll();
            }
        }

        void _setMyEnable() {
            if (!this.isActiveAndEnabled) {
                if (!this.enabled) {
                    Debug.Log("FaciemController is disabled.");
                    this.enabled = false;
                    return;
                } else {
                    Debug.Log("GameObject is not active.");
                    this.enabled = false;
                    return;
                }
            }
            if (!this._model) {
                Debug.LogError("MMD4MecanimModel not found.");
                this.enabled = false;
                return;
            }
            if (!this._model.enabled) {
                Debug.LogError("MMD4MecanimModel is disabled.");
                this.enabled = false;
                return;
            }
        }

        public virtual bool isProcessing {
            get {
                foreach (var idx in this.useMorphIndexs) {
                    if (this.helpers[idx].isProcessing)
                        return true;
                }
                return false;
            }
        }

        public virtual bool isAnimating {
            get {
                foreach (var idx in this.useMorphIndexs) {
                    if (this.helpers[idx].isAnimating)
                        return true;
                }
                return false;
            }
        }

        #region SetFace.
        /// <summary>
        /// 表情更新
        /// </summary>
        /// <param name="faceName">表情名</param>
        public virtual void SetFace(string faceName) {
            SetFace(faceName, true);
        }

        /// <summary>
        /// 表情設定
        /// </summary>
        /// <param name="faceName">表情名</param>
        /// <param name="isClearSpeed">更新後の表情で使用しないHelperのスピードをClearSpeed値でクリアする</param>
        public virtual void SetFace(string faceName, bool isClearSpeed) {
            if (!this.IsInitializedAndEnabled)
                return;

            var idx = this._database.GetFaceIndex(faceName);
            if (idx == -1) {
                Debug.LogWarning("Not found face : " + faceName);
                return;
            }
            SetFace(idx, isClearSpeed);
        }

        /// <summary>
        /// 表情設定
        /// </summary>
        /// <param name="faceData">表情Index</param>
        /// <param name="isClearSpeed">更新後の表情で使用しないHelperのスピードをClearSpeed値でクリアする</param>
        public virtual void SetFace(int faceIndex, bool isClearSpeed) {
            this.CurrentFaceIndex = faceIndex;
            var faceData = this._database.FaceDataList[faceIndex];

            foreach (var idx in this.useMorphIndexs) {
                var helper = this.helpers[idx];
                helper.morphWeight = 0;
                helper.morphSpeed = isClearSpeed ? this.ClearSpeed : helper.morphSpeed;
                helper.overrideWeight = this.OverrideWeight;
            }
            foreach (var faceMorph in faceData.MMD4Morphs) {
                var helper = this.helpers[faceMorph.morphIndex];
                helper.morphWeight = faceMorph.weight;
                helper.morphSpeed = faceMorph.speed;
            }
        }
        #endregion

        #region SetFace with speed.
        /// <summary>
        /// 表情設定 - 表情設定後、全てのHelperのSpeed値を指定値で上書きします
        /// </summary>
        /// <param name="faceName">表情名</param>
        /// <param name="speed">表情設定後、全てのHelperのSpeed値を指定値で上書きします</param>
        public virtual void SetFace(string faceName, float speed) {
            SetFace(faceName, speed, -0.1f);
        }

        /// <summary>
        /// 表情設定 - 表情設定後、全てのHelperのSpeed値を指定値で上書きします(閾値指定)
        /// </summary>
        /// <param name="faceName">表情名</param>
        /// <param name="speed">表情設定後、全てのHelperのSpeed値を指定値で上書きします</param>
        /// <param name="threshold">本値を越えるSpeed値を持ったHelperのみを対象とします</param>
        public virtual void SetFace(string faceName, float speed, float threshold) {
            if (!this.IsInitializedAndEnabled)
                return;
            
            var idx = this._database.GetFaceIndex(faceName);
            if (idx == -1) {
                Debug.LogWarning("Not found face : " + faceName);
                return;
            }
            SetFace(idx, false);
            SetSpeedAllHelper(speed, threshold);
        }

        /// <summary>
        /// 表情設定 - 表情設定後、全てのHelperのSpeed値を指定値で上書きします(閾値指定)
        /// </summary>
        /// <param name="faceName">表情Index</param>
        /// <param name="speed">表情設定後、全てのHelperのSpeed値を指定値で上書きします</param>
        /// <param name="threshold">本値を越えるSpeed値を持ったHelperのみを対象とします</param>
        public virtual void SetFace(int faceIndex, float speed, float threshold) {
            if (!this.IsInitializedAndEnabled)
                return;

            SetFace(faceIndex, false);
            SetSpeedAllHelper(speed, threshold);
        }
        #endregion

        #region SetFaceAdditive
        /// <summary>
        /// 表情設定 - 指定した表情にてWeightが0を超えるMorphのみ更新します。
        /// </summary>
        /// <param name="faceName">表情名</param>
        public virtual void SetFaceAdditive(string faceName) {
            SetFaceAdditive(faceName, -1f);
        }

        /// <summary>
        /// 表情設定 - 指定した表情にてWeightが0を超えるMorphのみ更新します。
        /// </summary>
        /// <param name="faceName">表情名</param>
        /// <param name="speed">上書きスピード -1でFaceDataに設定されたスピードを使用する</param>
        public virtual void SetFaceAdditive(string faceName, float speed) {
            if (!this.IsInitializedAndEnabled)
                return;
            
            var idx = this._database.GetFaceIndex(faceName);
            if (idx == -1) {
                Debug.LogWarning("Not found face : " + faceName);
                return;
            }
            SetFaceAdditive(idx, speed);
        }

        /// <summary>
        /// 表情設定 - 指定した表情にてWeightが0を超えるMorphのみ更新します。
        /// </summary>
        /// <param name="faceData">表情Index</param>
        /// <param name="speed">上書きスピード -1でFaceDataに設定されたスピードを使用する</param>
        public virtual void SetFaceAdditive(int faceIndex, float speed) {
            this.CurrentFaceIndex = faceIndex;
            var faceData = this._database.FaceDataList[faceIndex];

            foreach (var faceMorph in faceData.MMD4Morphs) {
                if (faceMorph.weight > 0) {
                    var helper = this.helpers[faceMorph.morphIndex];
                    helper.morphWeight = faceMorph.weight;
                    helper.morphSpeed = speed == -1 ? faceMorph.speed : speed;
                }
            }
        }
        #endregion

        #region SetSpeedCurrentFaceHelper
        /// <summary>
        /// 現在の表情に使用しているMorphHelperのスピード値を上書きします。
        /// </summary>
        /// <param name="speed">設定するスピード値</param>
        public virtual void SetSpeedCurrentFaceHelper(float speed) {
            if (!this.IsInitializedAndEnabled)
                return;

            foreach (var fmoprh in this._database.FaceDataList[this.CurrentFaceIndex].MMD4Morphs) {
                this.helpers[fmoprh.morphIndex].morphSpeed = speed;
            }
        }

        /// <summary>
        /// 現在の表情に使用しているMorphHelperのスピード値を上書きします。（閾値指定）
        /// </summary>
        /// <param name="speed">設定するスピード値</param>
        /// <param name="threshold">本値を越えるSpeed値を持ったHelperのみを対象とします</param>
        public virtual void SetSpeedCurrentFaceHelper(float speed, float threshold) {
            if (!this.IsInitializedAndEnabled)
                return;

            foreach (var fmoprh in this._database.FaceDataList[this.CurrentFaceIndex].MMD4Morphs) {
                if (this.helpers[fmoprh.morphIndex].morphSpeed > threshold)
                    this.helpers[fmoprh.morphIndex].morphSpeed = speed;
            }
        }
        #endregion

        #region SetSpeedAllHelper
        /// <summary>
        /// 全てのMorphHelperのスピード値を上書きします。
        /// </summary>
        /// <param name="speed">設定するスピード値</param>
        public virtual void SetSpeedAllHelper(float speed) {
            SetSpeedAllHelper(speed, -0.1f);
        }

        /// <summary>
        /// 全てのMorphHelperのスピード値を上書きします。（閾値指定）
        /// </summary>
        /// <param name="speed">設定するスピード値</param>
        /// <param name="threshold">本値を越えるSpeed値を持ったHelperのみを対象とします</param>
        public virtual void SetSpeedAllHelper(float speed, float threshold) {
            if (!this.IsInitializedAndEnabled)
                return;

            foreach (var idx in this.useMorphIndexs) {
                var helper = this.helpers[idx];
                if (helper.morphSpeed > threshold)
                    helper.morphSpeed = speed;
            }
        }
        #endregion

        #region GetFaceName
        /// <summary>
        /// 表情名を取得します
        /// </summary>
        /// <param name="faceIndex">表情データのIndex値</param>
        /// <returns></returns>
        public virtual string GetFaceName(int faceIndex) {
            if (!this.IsInitializedAndEnabled)
                return null;
            
            return this._database.FaceDataList[faceIndex].FaceName;
        }

        /// <summary>
        /// 現在の表情名を取得します
        /// </summary>
        /// <returns></returns>
        public virtual string GetCurrentFaceName() {
            if (!this.IsInitializedAndEnabled)
                return null;
            
            return this._database.FaceDataList[this.CurrentFaceIndex].FaceName;
        }

        /// <summary>
        /// デフォルトの表情名を取得します。
        /// </summary>
        /// <returns></returns>
        public virtual string GetDefaultFaceName() {
            if (!this.IsInitializedAndEnabled)
                return null;

            return this._database.FaceDataList[this.DefaultFaceIndex].FaceName;
        }
        #endregion

        /// <summary>
        /// デフォルト表情に強制的に戻します。（Speed値は0)
        /// </summary>
        public void ResetDefaultFace() {
            SetFace(this.DefaultFaceIndex, 0f, -1f);
        }

        /// <summary>
        /// モーフIndex値から対応するHelperコンポーネントを取得します。
        /// </summary>
        /// <param name="morphIndex"></param>
        /// <returns></returns>
        public MMD4MecanimMorphHelper GetMorphHelper(int morphIndex) {
            if (morphIndex < this.helpers.Length) {
                return this.helpers[morphIndex];
            } else {
                return null;
            }
        }

        /// <summary>
        /// MMD4MecanimModelの表情クリア (Weight全て0クリア)
        /// </summary>
        protected void FroceSetModelWieghtZeroAll() {
            if (this._model == null || this._model.morphList == null)
                return;

            foreach (var morph in this._model.morphList) {
                morph.weight = 0;
            }
            this._model.ForceUpdateMorph();
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        protected void Initialize() {
            // 先にtrueにしないと各関数が実行できない
            this.initialized = true;
            
            this.useMorphIndexs = GetRequiredMorpList();
            // Create Helpers
            CreateHelper();
            // Set Default face
            ResetDefaultFace();
        }

        /// <summary>
        /// 利用される可能性のあるHelperを作成します
        /// </summary>
        void CreateHelper() {
            this.helpers = new MMD4MecanimMorphHelper[this._model.modelData.morphDataList.Length];

            var existHelperList = new List<MMD4MecanimMorphHelper>(GetComponents<MMD4MecanimMorphHelper>());
            for (int i=0; i<existHelperList.Count; i++) {
                if (!existHelperList[i].enabled) {
                    existHelperList.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            foreach (var idx in this.useMorphIndexs) {
                int findIdx = -1;
                for (int i = 0; i < existHelperList.Count; i++) {
                    if (existHelperList[i].morphName == getMorphName(idx)) {
                        findIdx = i;
                        break;
                    }
                }
                if (findIdx == -1) {
                    var helper = this.gameObject.AddComponent<MMD4MecanimMorphHelper>();
                    helper.morphName = getMorphName(idx);
                    helper.morphSpeed = 0;
                    helper.morphWeight = 0;
                    helper.overrideWeight = this.OverrideWeight;
                    helpers[idx] = helper;
                } else {
                    var helper = existHelperList[findIdx];
                    helper.morphSpeed = 0;
                    helper.morphWeight = 0;
                    helper.overrideWeight = this.OverrideWeight;
                    helpers[idx] = helper;
                }
            }
        }

        /// <summary>
        /// 利用される可能性のあるMorphのIndex配列を取得します。<br/>
        /// モーフ番号１と３のモーフが利用される可能性がある場合は [1,3]
        /// </summary>
        protected int[] GetRequiredMorpList() {
            var morphDataList = this._model.modelData.morphDataList;
            var morphIdxFlgs = new bool[morphDataList.Length];
            foreach (var fdata in this._database.FaceDataList) {
                foreach (var morph in fdata.MMD4Morphs) {
                    morphIdxFlgs[morph.morphIndex] = true;
                }
            }

            var idxList = new List<int>();
            for (int i = 0; i < morphIdxFlgs.Length; i++) {
                if (morphIdxFlgs[i])
                    idxList.Add(i);
            }

            return idxList.ToArray();
        }

        /// <summary>
        /// index値からモーフ名を取得します
        /// </summary>
        /// <param name="morphIndex"></param>
        /// <returns></returns>
        protected string getMorphName(int morphIndex) {
            string name = null;
            switch (this._model.editorViewMorphNameType) {
                case MMD4MecanimModel.EditorViewMorphNameType.Japanese:
                    name = this._model.modelData.morphDataList[morphIndex].nameJp;
                    break;
                case MMD4MecanimModel.EditorViewMorphNameType.English:
                    name = this._model.modelData.morphDataList[morphIndex].nameEn;
                    break;
                case MMD4MecanimModel.EditorViewMorphNameType.Translated:
                    name = this._model.modelData.morphDataList[morphIndex].translatedName;
                    break;
            }
            return name;
        }
    }
}
