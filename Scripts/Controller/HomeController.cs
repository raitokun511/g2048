using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayOnClick()
    {
        SceneManager.LoadScene("GameMain");
    }
    public void PlayExtraOnClick()
    {

    }
    public void PlaySuperOnClick()
    {

    }
}
