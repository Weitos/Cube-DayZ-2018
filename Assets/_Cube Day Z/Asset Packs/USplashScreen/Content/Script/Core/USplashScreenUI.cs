﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class USplashScreenUI : MonoBehaviour
{
    [SerializeField]
    private string NextLevel = "MainMenu";
    /// <summary>
    /// All splash screen objects.
    /// </summary>
    [Tooltip("All splash screen objects.")]
    public List<USS> m_SplashScreens = new List<USS>();
    [Space(5)]
    //Appear skip button when load next scene.
    [Tooltip("Appear skip button when load next scene.")]
    public bool SkipWhenLoadLevel = true;
    //Hide Loading effect when next scene is loaded?.
    [Tooltip("Hide Loading effect when next scene is loaded?.")]
    public bool HideLoadingWhenLoad = true;
    //Time to appear skip button, just if 'SkipWhenLoadLevel' if false.
    [Tooltip("Time to appear skip button, just if 'SkipWhenLoadLevel' if false")]
    [Range(1, 15)]
    public int TimeForSkip = 2;
    [Space(5)]
    public GameObject SkipUI = null;
    public Image Black;
    public Slider ProgreesSlider = null;
    [Space(5)]
    public USLoadingEffect Loading = null;

    //Private
    private int current = 0;
    private AsyncOperation async = null; // When assigned, load is in progress. (Unity5 and Unity 4 Pro Only)
    private bool isDone = false;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        //Determine if using unity pro version.
        if (isPro)
        {
            //start Async load progress.
            StartCoroutine(LevelProgress());
            if (!SkipWhenLoadLevel)
            {
                InvokeRepeating("CountSkip", 1, 1);
            }
        }
        else
        {
            //If not unity pro or unity5, then desactive progress bar (because not work).
            ProgreesSlider.gameObject.SetActive(false);
            InvokeRepeating("CountSkip", 1, 1);
        }

        //Desactive all splash, for show on order.
        for (int i = 0; i < m_SplashScreens.Count; i++)
        {
            m_SplashScreens[i].SplashUI.SetActive(false);
        }
        //Start Splash screens
        StartCoroutine(SplashCorrutine());
        if (SkipUI != null && SkipUI.activeSelf)
        {
            SkipUI.SetActive(false);
        }

    }
    /// <summary>
    /// Corrutine for show all splash
    /// </summary>
    /// <returns></returns>
    IEnumerator SplashCorrutine()
    {
		for (int i = 0; i < m_SplashScreens.Count; i++)
		{
			//Active the current splash.
			m_SplashScreens[current].SplashUI.SetActive(true);
			//Wait while time of splash pass.
			yield return new WaitForSeconds(m_SplashScreens[i].m_time);
			//Go to next splash
			if (current < m_SplashScreens.Count - 1)
			{
				//Time to hide the current splash.
				m_SplashScreens[current].SplashUI.GetComponent<USplash>().Hide();
				//Now wait the determine time for continue with the next splash.
				yield return new WaitForSeconds(m_SplashScreens[current].WaitForNext);
				current++;
			}
			else//if not more,then load next scene.
			{
				Skip();
			}
		}
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
#if UNITY_PRO_LICENSE ||  UNITY_5_0
        ProgreesLoad();
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    void ProgreesLoad()
    {
        if (ProgreesSlider != null && async != null)
        {
            //Get progress of load level
            float p = (async.progress + 0.1f); //Fix problem of 90%
            //Smooth slider to percent.
            ProgreesSlider.value = Mathf.Lerp(ProgreesSlider.value, p, Time.deltaTime * 2);
            //When already load the next level
            if (async.isDone || ProgreesSlider.value >= 0.98f)
            {
                //Called one time what is inside in this function.
                if (!isDone)
                {
                    isDone = true;
                    //Can skip when next level is loaded.
                    if (SkipWhenLoadLevel)
                    {
                        if (SkipUI != null)
                        {
                            SkipUI.SetActive(true);
                        }
                    }
                    if (HideLoadingWhenLoad) { Loading.Loading = false; }
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void Skip()
    {
        StartCoroutine(SkipIE());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator SkipIE()
    {
        //Do fade effect
        Color c = Black.color;
        while (c.a < 1.0f)
        {
            c.a += Time.deltaTime;
            Black.color = c;
            yield return null;
        }
        //Fade Done, now load next level
        if (isPro)
        {
            async.allowSceneActivation = true;
        }
        else
        {
            SceneManager.LoadScene(NextLevel);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void CountSkip()
    {
        TimeForSkip--;
        if (TimeForSkip <= 0)
        {
            CancelInvoke("CountSkip");
            if (SkipUI != null)
            {
                SkipUI.SetActive(true);
            }
        }
    }
    /// <summary>
    /// Detect if this Unity is pro edition
    /// </summary>
    private bool isPro
    {
        get
        {
            bool p = false;

#if UNITY_PRO_LICENSE ||  UNITY_5_0
            p = true;
#endif
            return p;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="levelName">Level To Get Progress</param>
    /// <returns></returns>
    private IEnumerator LevelProgress()
    {
        async = SceneManager.LoadSceneAsync(NextLevel);
        async.allowSceneActivation = false;
        yield return async;
    }

    [System.Serializable]
    public class USS
    {
        public GameObject SplashUI;
        [Range(0.1f, 10.0f)]
        public float m_time = 2;
        [Range(0.1f, 6.0f)]
        public float WaitForNext = 1.0f;
    }
}