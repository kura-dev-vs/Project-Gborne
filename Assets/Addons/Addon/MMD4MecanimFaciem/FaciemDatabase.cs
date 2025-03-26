using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mebiustos.MMD4MecanimFaciem {
    public class FaciemDatabase : MonoBehaviour {

        [System.Serializable]
        public class FaceData {
            public string FaceName;
            public string FaceGuid;
            public MMD4MMorph[] MMD4Morphs = new MMD4MMorph[0];
#if UNITY_EDITOR
            public EditorParam _editorParam = new EditorParam();
#endif

            public FaceData(String faceName) {
                this.FaceName = faceName;
                this.FaceGuid = System.Guid.NewGuid().ToString();
            }

            /// <summary>
            /// ex: faceList.Sort(FaceData.CompareFaceName)
            /// </summary>
            /// <param name="data1"></param>
            /// <param name="data2"></param>
            /// <returns></returns>
            public static int CompareFaceName(FaceData data1, FaceData data2) {
                return string.Compare(data1.FaceName, data2.FaceName);
            }

            [System.Serializable]
            public class MMD4MMorph {
                public MMD4MMorph() {
                }

                //public string name;
                public float speed;
                public float weight;
                public int morphIndex;

                //[System.NonSerialized]
                public string GetName(MMD4MecanimModel model) {
                    return MMD4MecanimAPI.GetMorphName(model, this.morphIndex);
                }

                public MMD4MecanimData.MorphCategory Category(MMD4MecanimModel model) {
                    return MMD4MecanimAPI.GetMorphCategory(model, this.morphIndex);
                }
            }

            [System.Serializable]
            public class EditorParam {
                const float INIT_SPEED = 0.1f;
                public float BatchInputSpeedAll = INIT_SPEED;
                public float[] BatchInputSpeedCategory = new float[4] { INIT_SPEED, INIT_SPEED, INIT_SPEED, INIT_SPEED };
            }
        }

        /// <summary>
        /// 表情データリスト
        /// </summary>
        public FaceData[] FaceDataList = new FaceData[0];

        public string DefaultGuid;

#if UNITY_EDITOR
        public static bool hideWireframe = false;
        public int _selectedIdx;
        public string _selectedGuid;
        [System.NonSerialized]
        public string _newFaceName;
        [System.NonSerialized]
        public readonly float _newMorphSpeed = 0.1f;
#endif
        
        /// <summary>
        /// 表情名からFaceDataListインデックス番号を取得します。
        /// </summary>
        /// <param name="faceName"></param>
        /// <returns>-1 : notfound</returns>
        public int GetFaceIndex(string faceName) {
            for (int i = 0; i < this.FaceDataList.Length; i++) {
                if (this.FaceDataList[i].FaceName == faceName)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 表情名から表情データクラスを取得します。
        /// </summary>
        /// <param name="faceName"></param>
        /// <returns></returns>
        public FaciemDatabase.FaceData GetFaceData(string faceName) {
            foreach (var face in this.FaceDataList) {
                if (face.FaceName == faceName)
                    return face;
            }
            return null;
        }

        /// <summary>
        /// デフォルト表情のFaceDataListインデックス番号を取得します。<br/>
        /// ex) var DefaultFaceData = FaceDataList(GetDefaultFaceIndex());
        /// </summary>
        /// <returns>-1 : notfound</returns>
        public int GetDefaultFaceIndex() {
            return GetFaceIndexByGuid(this.DefaultGuid);
        }

        /// <summary>
        /// 指定したGUIDを持つFaceDataのインデックス番号を返却する
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="guid"></param>
        /// <returns>notfound is -1</returns>
        public int GetFaceIndexByGuid(string guid) {
            return GetFaceIndexByGuid(this, guid);
        }

        /// <summary>
        /// 指定したFaceDataを削除する
        /// </summary>
        /// <param name="index"></param>
        public void DeleteFace(int index) {
            var foos = new List<FaceData>(FaceDataList);
            foos.RemoveAt(index);
            FaceDataList = foos.ToArray();
            var guididx = GetFaceIndexByGuid(this.DefaultGuid);
            if (guididx == -1 && this.FaceDataList.Length > 0) {
                this.DefaultGuid = this.FaceDataList[0].FaceGuid;
            }
#if UNITY_EDITOR
            this._selectedIdx = 0;
#endif
        }

        /// <summary>
        /// 指定したFaceDataの名前を変更する
        /// </summary>
        /// <param name="index"></param>
        public void RenameFace(int index, string name) {
            this.FaceDataList[index].FaceName = name;
            var guid = this.FaceDataList[index].FaceGuid;
            var foos = new List<FaceData>(FaceDataList);
            foos.Sort(FaciemDatabase.FaceData.CompareFaceName);
            FaceDataList = foos.ToArray();
#if UNITY_EDITOR
            if (guid == this._selectedGuid) {
                this._selectedIdx = GetFaceIndexByGuid(guid);
                this._selectedGuid = guid;
            }
#endif
        }

        /// <summary>
        /// 指定した名前でFaceDataを追加する(MorphSpeedは0.1fで追加されます)
        /// </summary>
        /// <param name="facename"></param>
        /// <returns></returns>
        public FaceData AddNewFace(String facename) {
            return AddNewFace(facename, 0.1f);
        }

        /// <summary>
        /// 指定した名前でFaceDataを追加する
        /// </summary>
        /// <param name="facename"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public FaceData AddNewFace(String facename, float speed) {
            foreach (var data in this.FaceDataList) {
                if (data.FaceName == facename) {
                    return null;
                }
            }

            var faceData = new FaciemDatabase.FaceData(facename);
            faceData.MMD4Morphs = GetFaceMorphsByMMD4Model(this.GetComponent<MMD4MecanimModel>(), speed);
            var foos = new List<FaceData>(FaceDataList);
            foos.Add(faceData);
            foos.Sort(FaciemDatabase.FaceData.CompareFaceName);
            FaceDataList = foos.ToArray();

            if (this.FaceDataList.Length == 1) {
                // 最初のFaceDataはDefaultとする。
                this.DefaultGuid = faceData.FaceGuid;
            }

#if UNITY_EDITOR
            this._selectedIdx = GetFaceIndexByGuid(faceData.FaceGuid);
            this._selectedGuid = this.FaceDataList[this._selectedIdx].FaceGuid;
#endif

            return faceData;
        }

        /// <summary>
        /// 現在のMMD4MecanimModelのMorph状態からMMD4MMorph配列を生成し取得する。
        /// </summary>
        /// <param name="mmd4model"></param>
        /// <param name="morphSpeed"></param>
        /// <returns></returns>
        public static FaciemDatabase.FaceData.MMD4MMorph[] GetFaceMorphsByMMD4Model(MMD4MecanimModel mmd4model, float morphSpeed) {
            var fmorphs = new FaciemDatabase.FaceData.MMD4MMorph[GetWeightMorphCount(mmd4model.morphList)];

            int count = 0;
            var model = mmd4model;
            //MMD4MecanimModel.EditorViewMorphNameType viewNameType = model.editorViewMorphNameType;

            for (int catIndex = MMD4MecanimAPI.MorphCategoryIndexMin; catIndex < MMD4MecanimAPI.MorphCategoryIndexMax; ++catIndex) {
                MMD4MecanimData.MorphCategory morphCategory = (MMD4MecanimData.MorphCategory)catIndex;

                for (int morphIndex = 0; morphIndex < model.modelData.morphDataList.Length; ++morphIndex) {
                    if (model.modelData.morphDataList[morphIndex].morphCategory == morphCategory) {
                        if (model.morphList != null && (uint)morphIndex < model.morphList.Length) {
                            MMD4MecanimModel.Morph morph = model.morphList[morphIndex];
                            if (morph.weight > 0) {
                                var facemorph = new FaciemDatabase.FaceData.MMD4MMorph();
                                facemorph.morphIndex = morphIndex;
                                //facemorph.name = name;
                                facemorph.weight = morph.weight;
                                facemorph.speed = morphSpeed;

                                fmorphs[count] = facemorph;
                                count++;
                            }
                        }
                    }
                }
            }
            return fmorphs;
        }

        /// <summary>
        /// MMD4MecanimModelからWeightが0以上のMorph数を取得する。
        /// </summary>
        /// <returns></returns>
        static int GetWeightMorphCount(MMD4MecanimModel.Morph[] morphs) {
            int count = 0;
            foreach (var morph in morphs) {
                if (morph.weight > 0)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Morph名からMorphIndexを取得する
        /// </summary>
        /// <param name="model"></param>
        /// <param name="morphName"></param>
        /// <returns></returns>
        public static int GetMorphIndex(MMD4MecanimModel model, string morphName) {
            return MMD4MecanimAPI.GetMorphIndex(model, morphName);
        }

        /// <summary>
        /// MorphIndexからMorph名を取得する
        /// </summary>
        /// <param name="model"></param>
        /// <param name="morphIndex"></param>
        /// <returns></returns>
        public static string GetMorphName(MMD4MecanimModel model, int morphIndex) {
            return MMD4MecanimAPI.GetMorphName(model, morphIndex);
        }

        /// <summary>
        /// Databaseに保存されているFace情報のなまえ一覧を取得する。
        /// </summary>
        /// <returns></returns>
        public string[] GetFaceNames() {
            return GetFaceNames(this);
        }

        /// <summary>
        /// Databaseに保存されているFace情報のなまえ一覧を取得する。
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public static string[] GetFaceNames(FaciemDatabase database) {
            string[] keys = new string[database.FaceDataList.Length];
            for (int i = 0; i < database.FaceDataList.Length; i++) {
                var face = database.FaceDataList[i];
                keys[i] = face.FaceName;
            }
            return keys;
        }

        /// <summary>
        /// GUIDから表情Index値を取得する
        /// </summary>
        /// <param name="database"></param>
        /// <param name="faceGuid"></param>
        /// <returns>notfound is -1</returns>
        public static int GetFaceIndexByGuid(FaciemDatabase database, string faceGuid) {
            for (int i=0; i<database.FaceDataList.Length; i++) {
                if (database.FaceDataList[i].FaceGuid == faceGuid)
                    return i;
            }
            return -1;
        }
    }
}
