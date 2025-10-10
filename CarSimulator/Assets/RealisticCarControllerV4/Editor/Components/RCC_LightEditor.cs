﻿//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2025 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(RCC_Light)), CanEditMultipleObjects]
public class RCC_LightEditor : Editor {

    RCC_Light prop;

    Color originalGUIColor;

    GUISkin skin;

    public void OnEnable() {

        skin = Resources.Load("RCC_WindowSkin") as GUISkin;

    }

    public override void OnInspectorGUI() {

        prop = (RCC_Light)target;
        serializedObject.Update();
        originalGUIColor = GUI.color;

        GUI.skin = skin != null ? skin : EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);

        CheckLights();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("RCC lights will receive inputs from the parent car controller and adjusts intensity for lights. You can choose which type of light you want to use below. You won't need to specify left or right indicator lights.", EditorStyles.helpBox);

        if (!prop.overrideRenderMode)
            EditorGUILayout.LabelField("''Important'' or ''Not Important'' modes (Pixel or Vertex) overrided by RCC_Settings.", EditorStyles.helpBox);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("lightType"), new GUIContent("Light Type"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("inertia"), new GUIContent("Inertia"), false);

        prop.overrideRenderMode = EditorGUILayout.Toggle(new GUIContent("Override RenderMode", "Ignore selected settings in RCC Settings about lights as vertex / pixel lights."), prop.overrideRenderMode);

        if (!prop.overrideRenderMode)
            GUI.enabled = false;

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("renderMode"), new GUIContent("Render Mode"), false);
        EditorGUI.indentLevel--;

        GUI.enabled = true;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("flare"), new GUIContent("Lens Flare"), false);

        if (!prop.GetComponent<LensFlare>()) {

            if (GUILayout.Button("Create LensFlare")) {

                GameObject[] lights = Selection.gameObjects;

                for (int i = 0; i < lights.Length; i++) {

                    if (!lights[i].GetComponent<LensFlare>()) {

                        LensFlare lf = lights[i].AddComponent<LensFlare>();
                        lf.brightness = 0f;
                        lf.color = Color.white;
                        lf.fadeSpeed = 20f;

                        // Register the creation of the object for undo/redo functionality.
                        Undo.RegisterCreatedObjectUndo(lf, "Add Lensflare");

                        // Mark the scene as dirty so Unity knows it has changed.
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    }

                }

                EditorUtility.SetDirty(prop);

            }

        } else {

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("RCC uses ''Interpolation'' mode for all rigidbodies. Therefore, lights at front of the vehicle will blink while on high speeds. To fix this, select your RCC layer in the LensFlare component as ignored layer. RCC_Light script will simulate lens flares depending on camera distance and angle.''", EditorStyles.helpBox);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("flareBrightness"), new GUIContent("Lens Flare Brightness"), false);

        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useEmissionTexture"), new GUIContent("Use Emission Texture"), false);

        GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);

        if (prop.useEmissionTexture) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("emission"), new GUIContent("Emission"), true);
            EditorGUI.indentLevel--;

        }

        GUI.skin = skin != null ? skin : EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("isBreakable"), new GUIContent("Is Breakable"), false);

        if (prop.isBreakable) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("strength"), new GUIContent("Strength"), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("breakPoint"), new GUIContent("Break Point"), false);
            EditorGUI.indentLevel--;

        }

        if (!prop.GetComponentInChildren<TrailRenderer>()) {

            if (GUILayout.Button("Create Trail")) {

                GameObject[] lights = Selection.gameObjects;

                for (int i = 0; i < lights.Length; i++) {

                    if (!lights[i].GetComponentInChildren<TrailRenderer>()) {

                        GameObject newTrail = Instantiate(RCC_Settings.Instance.lightTrailers, lights[i].transform.position, lights[i].transform.rotation, lights[i].transform);
                        newTrail.name = RCC_Settings.Instance.lightTrailers.name;

                        // Register the creation of the object for undo/redo functionality.
                        Undo.RegisterCreatedObjectUndo(newTrail, "Add Trailrenderer");

                        // Mark the scene as dirty so Unity knows it has changed.
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    }

                }

                EditorUtility.SetDirty(prop);

            }

        } else {

            if (GUILayout.Button("Select Trail"))
                Selection.activeGameObject = prop.GetComponentInChildren<TrailRenderer>().gameObject;

        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

    }

    public void CheckLights() {

        if (!prop.gameObject.activeInHierarchy)
            return;

        if (prop.GetComponentInParent<RCC_CarControllerV4>(true) == null)
            return;

        if (prop.lightType == RCC_Light.LightType.External || prop.lightType == RCC_Light.LightType.Interior || prop.lightType == RCC_Light.LightType.ParkLight)
            return;

        Vector3 relativePos = prop.GetComponentInParent<RCC_CarControllerV4>(true).transform.InverseTransformPoint(prop.transform.position);

        if (relativePos.z > 0f) {

            if (Mathf.Abs(prop.transform.localRotation.y) > .5f) {

                GUI.color = Color.red;
                EditorGUILayout.HelpBox("Light is facing to wrong direction!", MessageType.Error);
                GUI.color = originalGUIColor;

                GUI.color = Color.green;

                if (GUILayout.Button("Fix Rotation")) {

                    prop.transform.localRotation = Quaternion.identity;
                    EditorUtility.SetDirty(prop);

                }

                GUI.color = originalGUIColor;

            }

        } else {

            if (Mathf.Abs(prop.transform.localRotation.y) < .5f) {

                GUI.color = Color.red;
                EditorGUILayout.HelpBox("Light is facing to wrong direction!", MessageType.Error);
                GUI.color = originalGUIColor;

                GUI.color = Color.green;

                if (GUILayout.Button("Fix Rotation")) {

                    prop.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                    EditorUtility.SetDirty(prop);

                }

                GUI.color = originalGUIColor;

            }

        }

        if (!EditorApplication.isPlaying) {

            GameObject[] lights = Selection.gameObjects;

            for (int i = 0; i < lights.Length; i++) {

                if (lights[i].GetComponent<Light>().flare != null)
                    lights[i].GetComponent<Light>().flare = null;

                if (lights[i].GetComponent<LensFlare>())
                    lights[i].GetComponent<LensFlare>().brightness = 0f;

            }

        }

    }

}
