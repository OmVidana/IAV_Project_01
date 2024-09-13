using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ButtonsBehavior : MonoBehaviour
{
    private VisualElement _root;
    private Button _playButton;
    private Button _exitButton;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _playButton = _root.Q<Button>("play");
        _exitButton = _root.Q<Button>("exit");
    }

    private void OnEnable()
    {
        _playButton.clicked += () => { SceneManager.LoadScene(sceneBuildIndex: 1); };
        _exitButton.clicked += () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Aplication.Quit();
#endif
        };
    }
}