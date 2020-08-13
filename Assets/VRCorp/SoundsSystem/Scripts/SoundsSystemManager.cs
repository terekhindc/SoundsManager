using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace VRCorp.SoundsSystem.Scripts
{
    [System.Serializable][InitializeOnLoad]
    public static class SoundsSystemManager
    {
        public static bool isEnabled;

        static SoundsSystemManager ()
        {
            isEnabled = Convert.ToBoolean((PlayerPrefs.GetInt("SoundsManagerEnabled")));
            if (isEnabled)
            {
                string[] dataPath = AssetDatabase.FindAssets("SoundsData");

                if (dataPath.Length > 0)
                {
                    AudioData audioData = (AudioData)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(dataPath[0]), typeof(AudioData));
                    SoundsSystemWindow.soundClick = audioData.onClickSound;
                    SoundsSystemWindow.soundEnter = audioData.onEnterSound;
                    EditorApplication.hierarchyChanged += SetSoundsForButton;                    
                }
            }
        }
        
        public static void Init ()
        {
            var audioSources = Resources.FindObjectsOfTypeAll(typeof(AudioSource));
            
            if (audioSources.Length == 0)
            {
                GameObject obj = new GameObject();
                obj.name = "MainAudioSource";
                obj.AddComponent<AudioSource>();
            }
            
            SetSoundsForButton();
            
            EditorApplication.hierarchyChanged += SetSoundsForButton;
            isEnabled = true;
            PlayerPrefs.SetInt("SoundsManagerEnabled", 1);
        }
        
        static void SetSoundsForButton()
        {
            var buttons = Object.FindObjectsOfType<Button>();

            foreach (var btn in buttons)
            {
                if (btn.GetComponent<Button>() && !btn.GetComponent<ButtonSoundsComponent>())
                {
                    ButtonSoundsComponent soundComponent = btn.gameObject.AddComponent<ButtonSoundsComponent>();
                    soundComponent.onClickSound = SoundsSystemWindow.soundClick;
                    soundComponent.onEnterSound = SoundsSystemWindow.soundEnter;
                }
            }
        }
        
        public static void DeleteAllComponents()
        {
            EditorApplication.hierarchyChanged -= SetSoundsForButton;
            var buttons = Object.FindObjectsOfType<Button>();
            Debug.Log($"{buttons.Length} buttons found");
            
            foreach (var btn in buttons)
            {
                if (btn.GetComponent<Button>())
                {
                    var components = btn.gameObject.GetComponents<ButtonSoundsComponent>();
                    Debug.Log($"{components.Length} component at the {btn}");
                    for (int i = 0; i < components.Length; i++)
                    {
                        Object.DestroyImmediate(components[i]);
                        Debug.Log($"Delete {components[i]} at the {btn}");                        
                    }
                }
            }
            
            Object.DestroyImmediate(GameObject.Find("MainAudioSource"));
            string[] dataPath = AssetDatabase.FindAssets("SoundsData");

            if (dataPath.Length > 0)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(dataPath[0]));
            }

            isEnabled = false;
            PlayerPrefs.SetInt("SoundsManagerEnabled", 0);
        }
        
    }

    [System.Serializable]
    public class SoundsSystemWindow : EditorWindow
    {
        private static string _status;
        public static AudioClip soundEnter;
        public static AudioClip soundClick;

        [MenuItem("Sounds Manager/Properties")]
        static void Init()
        {
            SoundsSystemWindow window = (SoundsSystemWindow) GetWindow(typeof(SoundsSystemWindow));
            window.titleContent.text = "Sounds manager";
            window.Show();
        }

        void OnGUI()
        {
            if (!SoundsSystemManager.isEnabled)
            {
                _status = "Enabled";
            } else _status = "Disabled";
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sound for button enter");
            soundEnter = (AudioClip)EditorGUILayout.ObjectField(soundEnter, typeof(AudioClip), true);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sound for button click");
            soundClick = (AudioClip)EditorGUILayout.ObjectField(soundClick, typeof(AudioClip), true);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Add for all buttons"))
            {
                SoundsSystemManager.Init();
                AudioData audioData = (AudioData)ScriptableObject.CreateInstance(typeof(AudioData));
                audioData.onClickSound = soundClick;
                audioData.onEnterSound = soundEnter;
                AssetDatabase.CreateAsset(audioData, "Assets/VRCorp/SoundsSystem/Data/SoundsData.asset");
            }
            
            if (GUILayout.Button("Delete from all buttons"))
            {
                SoundsSystemManager.DeleteAllComponents();
            }
            
        }
    }
}
