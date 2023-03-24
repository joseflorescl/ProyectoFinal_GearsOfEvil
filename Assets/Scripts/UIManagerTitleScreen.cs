using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerTitleScreen : MonoBehaviour
{
    public GameObject titleScreenPanel;
    public int secondsToActivateTitleScreenPanel = 10;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(ActivateTitleScreen), secondsToActivateTitleScreenPanel);
    }

    private void ActivateTitleScreen()
    {
        titleScreenPanel.SetActive(true);
    }
}
