﻿using UnityEngine;
using System.Collections;

namespace Mebiustos.MMD4MecanimFaciem {
    public static class MMD4MecanimAPI {
        public const int MorphCategoryIndexMin = 1;
        public const int MorphCategoryIndexMax = 5;

        /// <summary>
        /// モーフIndexからモーフ名を取得
        /// </summary>
        /// <param name="model"></param>
        /// <param name="morphIndex"></param>
        /// <returns></returns>
        public static string GetMorphName(MMD4MecanimModel model, int morphIndex) {
            string name = null;
            switch (model.editorViewMorphNameType) {
                case MMD4MecanimModel.EditorViewMorphNameType.Japanese:
                    name = model.modelData.morphDataList[morphIndex].nameJp;
                    break;
                case MMD4MecanimModel.EditorViewMorphNameType.English:
                    name = model.modelData.morphDataList[morphIndex].nameEn;
                    break;
                case MMD4MecanimModel.EditorViewMorphNameType.Translated:
                    name = model.modelData.morphDataList[morphIndex].translatedName;
                    break;
            }
            return name;
        }

        /// <summary>
        /// モーフ名からモーフIndexを取得する
        /// </summary>
        /// <param name="model"></param>
        /// <param name="morphName"></param>
        /// <returns></returns>
        public static int GetMorphIndex(MMD4MecanimModel model, string morphName) {
            int index = -1;
            switch (model.editorViewMorphNameType) {
                case MMD4MecanimModel.EditorViewMorphNameType.Japanese:
                    index = model.modelData.GetMorphDataIndexJp(morphName, false);
                    break;
                case MMD4MecanimModel.EditorViewMorphNameType.English:
                    index = model.modelData.GetMorphDataIndexEn(morphName, false);
                    break;
                case MMD4MecanimModel.EditorViewMorphNameType.Translated:
                    index = model.modelData.GetTranslatedMorphDataIndex(morphName, false);
                    break;
            }
            return index;
        }

        /// <summary>
        /// モーフIndexからモーフカテゴリを取得する
        /// </summary>
        /// <param name="model"></param>
        /// <param name="morphIndex"></param>
        /// <returns></returns>
        public static MMD4MecanimData.MorphCategory GetMorphCategory(MMD4MecanimModel model, int morphIndex) {
            return model.modelData.morphDataList[morphIndex].morphCategory;
        }

        public static MMD4MecanimData.MorphCategory GetMorphCategoryByCategoryIndex(int categoryIndex) {
            return (MMD4MecanimData.MorphCategory)categoryIndex;
        }

        /// <summary>
        /// MMD4MecanimModel内のモーフ変更判定を無効化して強制更新する
        /// </summary>
        public static void SuperForceUpdateMorph(MMD4MecanimModel model) {
            if (model == null || model.morphList == null || model.morphList.Length == 0) return;

            var backupAppend = model.morphList[0]._appendWeight;
            var backupWeight = model.morphList[0].weight;
            model.morphList[0]._appendWeight = 1;
            model.morphList[0].weight = model.morphList[0].weight != 1 ? 1 : 0.5f;
            model.ForceUpdateMorph();
            model.morphList[0]._appendWeight = backupAppend;
            model.morphList[0].weight = backupWeight;
            model.ForceUpdateMorph();
        }
    }
}
