using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class PreferencesWindow : EditorWindow
{
    public static PreferencesWindow currentPreferenceWindow;
    public NodePreferenceView currentPreferenceView;

    public static void InitPreferenceWindow()
    {
        currentPreferenceWindow = (PreferencesWindow)EditorWindow.GetWindow<PreferencesWindow>();
        currentPreferenceWindow.title = "Preferences";

        CreateView();
    }

    void OnEnable()
    {
        Debug.Log("Preferences Windows was Enabled.");
        WorkPreferences.loadPreferences();
    }

    void OnGUI()
    {
        if (currentPreferenceView == null )
        {
            CreateView();
            return;
        }

        Event e = Event.current;

        currentPreferenceView.UpdateView(position, new Rect(0f, 0f, 1f, 1f), e, null);
        currentPreferenceView.ProcessEvents(e);

        Repaint();
    }

    private static void CreateView()
    {
        if (currentPreferenceWindow != null)
        {
            currentPreferenceWindow.currentPreferenceView = new NodePreferenceView();
        }
        else
        {
            currentPreferenceWindow = (PreferencesWindow)EditorWindow.GetWindow<PreferencesWindow>();
        }
    }
}
