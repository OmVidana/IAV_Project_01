using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ThievesUI : MonoBehaviour
{
    public static int thievesLeft = 10;
    private VisualElement _root;
    public static Label thievesLabel;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        thievesLabel = _root.Q<Label>("robber_count");
        thievesLabel.text = $"Thieves left: {thievesLeft}";
    }
    
    void Update()
    {
        if (thievesLeft == 0)
            SceneManager.LoadScene(0);
    }
}
