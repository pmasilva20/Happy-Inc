using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundClick : MonoBehaviour
{

    InputManager inputManager;

    private void Awake()
    {
        var levelStateMachine = GameObject.Find("LevelStateMachine");
        inputManager = levelStateMachine.GetComponent<InputManager>();
    }


    /// <summary>
    /// On click report background click
    /// </summary>
    private void OnMouseDown()
    {
        Debug.Log("Click background");
        inputManager.OnBackgroundClick();
    }

}
