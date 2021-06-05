using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamBrowserManager : MonoBehaviour
{
    protected static SteamBrowserManager s_instance;
	protected static SteamBrowserManager Instance {
		get {
			if (s_instance == null) {
				return new GameObject("SteamBrowserManager").AddComponent<SteamBrowserManager>();
			}
			else {
				return s_instance;
			}
		}
	}
    
    protected bool m_bInitialized = false;
	public static bool Initialized {
		get {
			return Instance.m_bInitialized;
		}
	}
    
    protected virtual void Awake() {
		if (s_instance != null) {
			Destroy(gameObject);
			return;
		}
		s_instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        if (SteamManager.Initialized) {
            m_bInitialized = SteamHTMLSurface.Init();
            if(!m_bInitialized) {
                Debug.LogError("Failed to initalize SteamHTMLSurface!");
            }
        } else {
            Debug.LogError("Failed to initalize SteamBrowserManager, SteamManager was not initalized");
        }
    }
    
    private void OnEnable() {
            
    }
    
    protected virtual void onDestroy() {
        if (s_instance != this) {
			return;
		}

		s_instance = null;

		if (!m_bInitialized) {
			return;
		}
        
        SteamHTMLSurface.Shutdown();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
