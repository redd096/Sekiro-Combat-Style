using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu = default;

    public void PauseMenu(bool pause)
    {
        pauseMenu.SetActive(pause);
    }
}
