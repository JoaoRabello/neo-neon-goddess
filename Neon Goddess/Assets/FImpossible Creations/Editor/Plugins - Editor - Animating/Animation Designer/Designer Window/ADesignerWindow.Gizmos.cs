﻿using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    public partial class AnimationDesignerWindow : EditorWindow
    {
        bool drawGizmos = true;
        bool drawModsGizmos = true;
        public static bool drawGUIGosting { get { return true;/* Get.drawModsGizmos;*/ } }

        void DrawScreenGUI() { }

        void DrawHandles(Camera c)
        {
            if (!DisplaySave) return;
            if (S.Armature == null) return;
            if (S.Armature.RootBoneReference == null) return;
            if (!S.SkelRootBone) return;
            if (!S.ReferencePelvis) return;
            if (!isReady) return;

            Handles.color = new Color(0.2f, 0.9f, 0.7f, GetCategorySkelAlpha());

            if (Category != ECategory.IK) Gizmos_DrawRefCircle();

            if (drawGizmos) Gizmos_DrawSkeleton();

            if (drawModsGizmos) Gizmos_SectionsGizmos();

            Repaint();
        }


        void Gizmos_DrawRefCircle()
        {
            Handles.DrawWireDisc(S.SkelRootBone.position, S.T.up, S.ScaleRef);
        }


        void Gizmos_DrawSkeleton()
        {
            Handles.DrawDottedLine(S.SkelRootBone.position, S.ReferencePelvis.position, 2f);

            for (int i = 1; i < S.Armature.BonesSetup.Count; i++)
            {
                var b = S.Armature.BonesSetup[i];

                if (b.TempTransform == null)
                {
                    break;
                }

                if (b.TempTransform.parent)
                    FGUI_Handles.DrawBoneHandle(b.TempTransform.parent.position, b.TempTransform.position, 1f);
            }
        }


        void Gizmos_SectionsGizmos()
        {
            switch (Category)
            {
                case ECategory.Setup: _Gizmos_SetupCategory(); break;
                case ECategory.IK: _Gizmos_IKCategory(); break;
                case ECategory.Modificators: _Gizmos_ModsCategory(); break;
                case ECategory.Elasticness: _Gizmos_ElasticnessCategory(); break;
                case ECategory.Morphing: _Gizmos_MorphingCategory(); break;
            }
        }

        float GetCategorySkelAlpha()
        {
            switch (Category)
            {
                case ECategory.Setup: return 0.15f;
                case ECategory.IK: return 0.1f;
                case ECategory.Modificators: return 0.1f;
                case ECategory.Elasticness: return 0.06f;
                case ECategory.Morphing: return 0.1f;
            }

            return 0.15f;
        }

        public static void GUIDrawFloatPercentage(ref float value, GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();

            float width = EditorStyles.label.CalcSize(label).x;

            EditorGUILayout.LabelField(label, GUILayout.Width(width + 2));

            float sliderVal = GUILayout.HorizontalSlider(value, 0f, 1f/*, GUILayout.Width(140)*/);
            GUILayout.Space(4);

            float pre = Mathf.Round(sliderVal * 100f);
            float nvalue = EditorGUILayout.FloatField(Mathf.Round(sliderVal * 100f), GUILayout.Width(30));
            if (nvalue != pre) sliderVal = nvalue / 100f;

            value = sliderVal;

            EditorGUILayout.LabelField("%", GUILayout.Width(14));

            EditorGUILayout.EndHorizontal();
        }

        public static void GUIDrawFloatSeconds(string label, ref float value, float min, float max, string postFix = "sec", string tooltip = "")
        {
            EditorGUILayout.BeginHorizontal();
            if (string.IsNullOrEmpty(tooltip))
                value = EditorGUILayout.Slider(label, value, min, max);
            else
                value = EditorGUILayout.Slider(new GUIContent(label, tooltip), value, min, max);
            EditorGUILayout.LabelField(postFix, GUILayout.Width(26));
            EditorGUILayout.EndHorizontal();
        }

    }
}