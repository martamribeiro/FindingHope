using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIUniverseTravelToMain : MonoBehaviour
{

    public void TravelToMainUniverse()
    {
        SceneManager.LoadScene("MainUniverse");
    }

}
