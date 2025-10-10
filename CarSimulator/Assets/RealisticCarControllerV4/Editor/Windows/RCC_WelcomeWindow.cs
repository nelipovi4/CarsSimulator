﻿//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2025 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class RCC_WelcomeWindow : EditorWindow {

    public class ToolBar {

        public string title;
        public UnityEngine.Events.UnityAction Draw;

        /// <summary>
        /// Create New Toolbar
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="onDraw">Method to draw when toolbar is selected</param>
        public ToolBar(string title, UnityEngine.Events.UnityAction onDraw) {

            this.title = title;
            this.Draw = onDraw;

        }

        public static implicit operator string(ToolBar tool) {
            return tool.title;
        }

    }

    /// <summary>
    /// Index of selected toolbar.
    /// </summary>
    public int toolBarIndex = 0;

    /// <summary>
    /// List of Toolbars
    /// </summary>
    public ToolBar[] toolBars = new ToolBar[]{

        new ToolBar("Welcome", WelcomePageContent),
        new ToolBar("Demos", DemosPageContent),
        new ToolBar("Updates", UpdatePageContent),
        new ToolBar("Addons", Addons),
        new ToolBar("DOCS", Documentation)

    };

    public static Texture2D bannerTexture = null;

    private GUISkin skin;

    private const int windowWidth = 600;
    private const int windowHeight = 750;

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller/Welcome Window", false, 10000)]
    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller/Welcome Window", false, 10000)]
    public static void OpenWindow() {

        GetWindow<RCC_WelcomeWindow>(true);

    }

    public void OnEnable() {

        titleContent = new GUIContent("Realistic Car Controller");
        maxSize = new Vector2(windowWidth, windowHeight);
        minSize = maxSize;

        InitStyle();

    }

    public void InitStyle() {

        if (!skin)
            skin = Resources.Load("RCC_WindowSkin") as GUISkin;

        bannerTexture = (Texture2D)Resources.Load("Editor/RCCBanner", typeof(Texture2D));

    }

    public void OnGUI() {

        GUI.skin = skin != null ? skin : EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);

        DrawHeader();
        DrawMenuButtons();
        DrawToolBar();
        DrawFooter();

        if (!EditorApplication.isPlaying)
            Repaint();

    }

    public void DrawHeader() {

        GUILayout.Label(bannerTexture, GUILayout.Height(120));

    }

    public void DrawMenuButtons() {

        GUILayout.Space(-10);
        toolBarIndex = GUILayout.Toolbar(toolBarIndex, ToolbarNames());

    }

    #region ToolBars

    public static void WelcomePageContent() {

        EditorGUILayout.BeginVertical("window");
        GUILayout.Label("Welcome!");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>Thank you for purchasing and using Realistic Car Controller. Please read the documentation before use. Also check out the online documentation for updated info. Have fun :)</b>");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>If you don't want to overwrite your project settings, you can create these layers and select them in RCC Settings (Tools --> BCG --> RCC --> Edit Settings --> Tags & Layers section.) \n \n RCC_Vehicle \n RCC_WheelCollider \n RCC_DetachablePart \n RCC_Prop \n \n More info can be found in documentation (First To Do).</b>");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Demo Scenes To Build Settings"))
            AddDemoScenesToBuildSettings();

        EditorGUILayout.Separator();

        EditorGUILayout.HelpBox("If you want to add Photon PUN2 scenes, import and install Photon PUN2 & integration first. Then click again to add those scenes to your Build Settings.", MessageType.Info, true);
        EditorGUILayout.HelpBox("If you want to add Enter / Exit scenes, import BCG Shared Assets to your project first. Then click again to add those scenes to your Build Settings.", MessageType.Info, true);
        EditorGUILayout.Separator();

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        GUI.color = Color.red;

        if (GUILayout.Button("Delete all demo content from the project")) {

            if (EditorUtility.DisplayDialog("Realistic Car Controller | Warning", "You are about to delete all demo content such as vehicle models, vehicle prefabs, vehicle textures, all scenes, scene models, scene prefabs, scene textures!", "Delete", "Cancel"))
                DeleteDemoContent();

        }

        GUI.color = Color.white;

        EditorGUILayout.EndVertical();

    }

    public static void UpdatePageContent() {

        EditorGUILayout.BeginVertical("window");
        GUILayout.Label("Updates");

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>Installed Version: </b>" + RCC_Version.version.ToString());
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>1</b>- Always backup your project before updating RCC or any asset in your project!");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>2</b>- If you have own assets such as prefabs, audioclips, models, scripts in RealisticCarControllerV4 folder, keep your own asset outside from RealisticCarControllerV3 folder.");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>3</b>- Delete RealisticCarControllerV4 folder, and import latest version to your project.");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        if (GUILayout.Button("Check Updates"))
            Application.OpenURL(RCC_AssetPaths.assetStorePath);

        GUILayout.Space(6);

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

    }

    public static void DemosPageContent() {

        EditorGUILayout.BeginVertical("window");

        GUILayout.Label("Demo Scenes");

        bool BCGInstalled = false;

#if BCG_ENTEREXIT
        BCGInstalled = true;
#endif

        bool photonInstalled = false;

#if RCC_PHOTON && PHOTON_UNITY_NETWORKING
        photonInstalled = true;
#endif

        EditorGUILayout.Separator();
        EditorGUILayout.HelpBox("All scenes must be in your Build Settings to run AIO demo.", MessageType.Warning, true);
        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical("box");

        if (GUILayout.Button("RCC City AIO"))
            EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_AIO), OpenSceneMode.Single);

        if (GUILayout.Button("RCC City"))
            EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_City), OpenSceneMode.Single);

        if (GUILayout.Button("RCC City Car Selection"))
            EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_CarSelection), OpenSceneMode.Single);

        if (GUILayout.Button("RCC City Car Selection with Load Next Scene"))
            EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_CarSelectionLoadNextScene), OpenSceneMode.Single);

        if (GUILayout.Button("RCC City Car Selection with Loaded Scene"))
            EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_CarSelectionLoadedScene), OpenSceneMode.Single);

        if (GUILayout.Button("RCC Blank Override Inputs"))
            EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_OverrideInputs), OpenSceneMode.Single);

        if (GUILayout.Button("RCC Blank Customization"))
            EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_Customization), OpenSceneMode.Single);

        if (GUILayout.Button("RCC Blank API"))
            EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_APIBlank), OpenSceneMode.Single);

        if (GUILayout.Button("RCC Blank Test Scene"))
            EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_BlankMobile), OpenSceneMode.Single);

        if (GUILayout.Button("RCC Damage Test Scene"))
            EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_Damage), OpenSceneMode.Single);

        if (GUILayout.Button("RCC Multiple Terrain Test Scene"))
            EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_MultipleTerrain), OpenSceneMode.Single);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        if (BCGInstalled) {

            if (GUILayout.Button("RCC City Enter-Exit FPS"))
                EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_CityFPS), OpenSceneMode.Single);

            if (GUILayout.Button("RCC City Enter-Exit TPS"))
                EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_CityTPS), OpenSceneMode.Single);

        } else {

            EditorGUILayout.HelpBox("You have to import latest BCG Shared Assets to your project first.", MessageType.Warning);

            if (GUILayout.Button("Download and import BCG Shared Assets"))
                AssetDatabase.ImportPackage(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.addon_BCGSharedAssets), true);

        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");

        if (photonInstalled) {

            if (GUILayout.Button("RCC Lobby Photon PUN 2"))
                EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_PUN2Lobby), OpenSceneMode.Single);

            if (GUILayout.Button("RCC City Photon PUN 2"))
                EditorSceneManager.OpenScene(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_PUN2City), OpenSceneMode.Single);

        } else {

            EditorGUILayout.HelpBox("You have to import latest Photon PUN2 to your project first.", MessageType.Warning);

            if (GUILayout.Button("Download and import Photon PUN2"))
                Application.OpenURL(RCC_AssetPaths.photonPUN2);

        }

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

    }

    public static void Addons() {

        EditorGUILayout.BeginVertical("window");
        GUILayout.Label("Addons");
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("<b>Photon PUN2</b>");

        bool photonInstalled = false;

#if PHOTON_UNITY_NETWORKING
        photonInstalled = true;
#endif

        bool photonAndRCCInstalled = false;

#if RCC_PHOTON && PHOTON_UNITY_NETWORKING
        photonAndRCCInstalled = true;
#endif

        if (!photonAndRCCInstalled) {

            if (!photonInstalled) {

                EditorGUILayout.HelpBox("You have to import latest Photon PUN2 to your project first.", MessageType.Warning);

                if (GUILayout.Button("Download and import Photon PUN2"))
                    Application.OpenURL(RCC_AssetPaths.photonPUN2);

            } else {

                EditorGUILayout.HelpBox("Found Photon PUN2, You can import integration package and open Photon demo scenes now.", MessageType.Info);

                if (GUILayout.Button("Import Photon PUN2 Integration"))
                    AssetDatabase.ImportPackage(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.addon_PhotonPUN2), true);

            }

        } else if (photonInstalled) {

            EditorGUILayout.HelpBox("Installed Photon PUN2 with RCC, You can open Photon demo scenes now.", MessageType.Info);

        }

#if RCC_PHOTON && PHOTON_UNITY_NETWORKING
        if (photonInstalled) {

            EditorGUILayout.LabelField("Photon PUN2 Version: " + System.Reflection.Assembly.GetAssembly(typeof(ExitGames.Client.Photon.PhotonPeer)).GetName().Version.ToString(), EditorStyles.centeredGreyMiniLabel);
            GUILayout.Space(6);

        }
#endif

        EditorGUILayout.EndVertical();

        bool BCGInstalled = false;

#if BCG_ENTEREXIT
        BCGInstalled = true;
#endif

        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("<b>BCG Shared Assets (Enter / Exit)</b>");

        if (!BCGInstalled) {

            EditorGUILayout.HelpBox("You have to import latest BCG Shared Assets to your project first.", MessageType.Warning);

            if (GUILayout.Button("Download and import BCG Shared Assets"))
                AssetDatabase.ImportPackage(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.addon_BCGSharedAssets), true);

        } else {

            EditorGUILayout.HelpBox("Found BCG Shared Assets, You can open Enter / Exit demo scenes now.", MessageType.Info);

#if BCG_ENTEREXIT
            EditorGUILayout.LabelField("BCG Shared Assets Version: " + BCG_Version.version, EditorStyles.centeredGreyMiniLabel);
#endif
            GUILayout.Space(6);

        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("<b>ProFlares</b>");

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Download and import ProFlares"))
            Application.OpenURL(RCC_AssetPaths.proFlares);

        if (GUILayout.Button("Import ProFlares Integration"))
            AssetDatabase.ImportPackage(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.addon_ProFlare), true);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("<b>URP</b>");

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Convert All Materials To URP")) {

            EditorUtility.DisplayDialog("Realistic Car Controller | Converting All Demo Materials To URP", "All demo materials will be selected in your project now. After that, you'll need to convert them to URP shaders while they have been selected. You can convert them from the Edit --> Render Pipeline --> Universal Render Pipeline --> Convert Selected Materials.", "Close");

            UnityEngine.Object[] objects = new UnityEngine.Object[RCC_DemoMaterials.Instance.demoMaterials.Length];

            for (int i = 0; i < objects.Length; i++)
                objects[i] = RCC_DemoMaterials.Instance.demoMaterials[i];

            Selection.objects = objects;

        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

    }

    public static void Documentation() {

        EditorGUILayout.BeginVertical("window");

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.HelpBox("Latest online documentation for scripts, settings, setup, how to do, and API.", MessageType.Info);

        if (GUILayout.Button("Youtube Tutorial Videos"))
            Application.OpenURL(RCC_AssetPaths.YTVideos);

        if (GUILayout.Button("Other Assets"))
            Application.OpenURL(RCC_AssetPaths.otherAssets);

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

    }

    #endregion

    public string[] ToolbarNames() {

        string[] names = new string[toolBars.Length];

        for (int i = 0; i < toolBars.Length; i++)
            names[i] = toolBars[i];

        return names;

    }

    public void DrawToolBar() {

        GUILayout.BeginArea(new Rect(4, 140, 592, 540));

        toolBars[toolBarIndex].Draw();

        GUILayout.EndArea();

        GUILayout.FlexibleSpace();

    }

    public void DrawFooter() {

        EditorGUILayout.BeginHorizontal("box");

        EditorGUILayout.LabelField("BoneCracker Games", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Realistic Car Controller " + RCC_Version.version, EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Ekrem Bugra Ozdoganlar", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndHorizontal();

    }

    public static void ImportPackage(string package) {

        try {
            AssetDatabase.ImportPackage(package, true);
        } catch (Exception) {
            Debug.LogError("Failed to import package: " + package);
            throw;
        }

    }

    public static void DeleteDemoContent() {

        Debug.LogWarning("Deleting demo content...");

        List<string> contentToDelete = new List<string>();

        // Collect paths to delete
        for (int i = 0; i < RCC_AssetPaths.Instance.demoContentToDelete.Length; i++) {

            if (!string.IsNullOrEmpty(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demoContentToDelete[i]))) {

                string path = RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demoContentToDelete[i]);

                if (Directory.Exists(path) || File.Exists(path))
                    contentToDelete.Add(path);
                else
                    Debug.LogWarning($"Path not found: {path}");

            }

        }

        // Delete the contents
        foreach (string item in contentToDelete) {

            try {

                FileUtil.DeleteFileOrDirectory(item);
                Debug.Log($"Deleted: {item}");

            } catch (System.Exception ex) {

                Debug.LogError($"Failed to delete: {item}. Exception: {ex.Message}");

            }

        }

        // Refresh AssetDatabase after deletion
        AssetDatabase.Refresh();

        string defaultResourcesPath = RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.importDefaultResources);

        if (File.Exists(defaultResourcesPath)) {

            AssetDatabase.ImportPackage(defaultResourcesPath, true);
            Debug.Log($"Imported default resources package: {defaultResourcesPath}");

        } else {

            Debug.LogError($"Default resources package not found at: {defaultResourcesPath}");

        }

        Debug.LogWarning("Deleted demo content!");
        EditorUtility.DisplayDialog("Realistic Car Controller | Deleted Demo Content", "All demo content have been removed from the project!", "Ok");

    }


    public static void AddDemoScenesToBuildSettings() {

        List<string> demoScenePaths = new List<string>();

        demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_AIO));
        demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_City));
        demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_CarSelection));
        demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_CarSelectionLoadNextScene));
        demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_CarSelectionLoadedScene));
        demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_OverrideInputs));
        demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_Customization));
        demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_APIBlank));
        demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_BlankMobile));
        demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_Damage));
        demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_MultipleTerrain));

        bool BCGInstalled = false;

#if BCG_ENTEREXIT
        BCGInstalled = true;
#endif

        if (BCGInstalled) {

            demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_CityFPS));
            demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_CityTPS));

        }

        bool photonAndRCCInstalled = false;

#if RCC_PHOTON && PHOTON_UNITY_NETWORKING
        photonAndRCCInstalled = true;
#endif

        if (photonAndRCCInstalled) {

            demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_PUN2Lobby));
            demoScenePaths.Add(RCC_AssetPaths.Instance.GetPath(RCC_AssetPaths.Instance.demo_PUN2City));

        }

        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();

        foreach (string path in demoScenePaths) {

            if (!string.IsNullOrEmpty(path))
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(path, true));

        }

        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

        EditorUtility.DisplayDialog("Realistic Car Controller | Demo Scenes", "All demo scenes have been added to the Build Settings. For Photon and Enter / Exit scenes, you have to import and intregrate them first (Addons). After importing them, click again to add new demo scenes.", "Ok");

    }

}
