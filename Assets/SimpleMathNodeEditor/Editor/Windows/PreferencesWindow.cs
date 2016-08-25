using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class PreferencesWindow : EditorWindow
{
    public static PreferencesWindow currentPreferenceWindow;

    public static void InitPreferenceWindow()
    {
        currentPreferenceWindow = (PreferencesWindow)EditorWindow.GetWindow<PreferencesWindow>();
        currentPreferenceWindow.title = "Preferences";

    }
}
