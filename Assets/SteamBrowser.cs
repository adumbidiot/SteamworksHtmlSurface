using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SteamBrowser : MonoBehaviour
{
    protected CallResult<HTML_BrowserReady_t> m_HTMLBrowserReady;
    protected Callback<HTML_StartRequest_t> m_StartRequest;
    protected Callback<HTML_NeedsPaint_t> m_NeedsPaint;
    protected Callback<HTML_JSAlert_t> m_JSAlert;
    protected Callback<HTML_JSConfirm_t> m_JSConfirm;
    protected Callback<HTML_FileOpenDialog_t> m_FileOpenDialog;
    protected Callback<HTML_BrowserRestarted_t> m_BrowserRestarted;
    protected Callback<HTML_CloseBrowser_t> m_BrowserClosed;
    
    protected HHTMLBrowser m_BrowserHandle = HHTMLBrowser.Invalid;
    
    protected uint m_Width = 800;
    protected uint m_Height = 600;
    
    protected string m_Url = null;
    protected string m_PostData = null;
    
    protected Texture2D m_Texture = null;
    protected bool m_IsDirty = false;
    
    private void OnEnable() {        
        if (SteamBrowserManager.Initialized) {
			m_HTMLBrowserReady = CallResult<HTML_BrowserReady_t>.Create(OnHTMLBrowserReady);
            
            m_StartRequest = Callback<HTML_StartRequest_t>.Create(OnStartRequest);
            m_NeedsPaint = Callback<HTML_NeedsPaint_t>.Create(OnNeedsPaint);
            m_JSAlert = Callback<HTML_JSAlert_t>.Create(OnJSAlert);
            m_JSConfirm = Callback<HTML_JSConfirm_t>.Create(OnJSConfirm);
            m_FileOpenDialog = Callback<HTML_FileOpenDialog_t>.Create(OnOpenFileDialog);
            m_BrowserRestarted = Callback<HTML_BrowserRestarted_t>.Create(OnBrowserRestarted);
            m_BrowserClosed = Callback<HTML_CloseBrowser_t>.Create(OnBrowserClosed);
            
            SteamAPICall_t handle = SteamHTMLSurface.CreateBrowser(null, null);
            m_HTMLBrowserReady.Set(handle);
		} else {
            Debug.LogError("Failed to make ComputerBrowser, SteamBrowserManager was not initialized");
        }
    }
    
    private void onDisable() {
        if(m_BrowserHandle != HHTMLBrowser.Invalid) {
            SteamHTMLSurface.RemoveBrowser(m_BrowserHandle);
            m_BrowserHandle = HHTMLBrowser.Invalid;
        }
        
        m_Texture = null;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnHTMLBrowserReady(HTML_BrowserReady_t pCallback, bool bIOFailure) {
		if (pCallback.unBrowserHandle == HHTMLBrowser.Invalid || bIOFailure) {
			Debug.Log("There was an error creating a new Browser");
            return;
        }
        
        UpdateBrowserHandle(pCallback.unBrowserHandle);
	}
    
    private void UpdateBrowserHandle(HHTMLBrowser handle) {
        m_BrowserHandle = handle;            
        SteamHTMLSurface.SetSize(m_BrowserHandle, m_Width, m_Height);
        if(m_Url != null) {
            SteamHTMLSurface.LoadURL(m_BrowserHandle, m_Url, m_PostData);
        }
    }
    
    private void OnStartRequest(HTML_StartRequest_t pCallback) {
        if(pCallback.unBrowserHandle == m_BrowserHandle) {
            SteamHTMLSurface.AllowStartRequest(m_BrowserHandle, true);
        }
    }
    
    private void OnNeedsPaint(HTML_NeedsPaint_t pCallback) {
        if(pCallback.unBrowserHandle == m_BrowserHandle) {
            if(pCallback.unWide != m_Width || pCallback.unTall != m_Height) {
                Debug.LogWarning("Texture dimensions (" + pCallback.unWide + " x " + pCallback.unTall + ") does not match browser dimensions (" + m_Width + " x " + m_Height + ")");
                return;
            }
            
            if(m_Texture == null) {
                m_Texture = new Texture2D((int)m_Width, (int)m_Height, TextureFormat.BGRA32, false, true);
            }
            
            int dataSize = (int)(pCallback.unWide * pCallback.unTall * 4);
            byte[] bytes = new byte[dataSize];
            Marshal.Copy(pCallback.pBGRA, bytes, 0, dataSize);

            m_Texture.LoadRawTextureData(bytes);
            m_Texture.Apply();
            
            m_IsDirty = true;
        }
    }
    
    private void OnJSAlert(HTML_JSAlert_t pCallback) {
        if(pCallback.unBrowserHandle == m_BrowserHandle) {
            SteamHTMLSurface.JSDialogResponse(pCallback.unBrowserHandle, false);
        }
    }
    
    private void OnJSConfirm(HTML_JSConfirm_t pCallback) {
        if(pCallback.unBrowserHandle == m_BrowserHandle) {
            SteamHTMLSurface.JSDialogResponse(pCallback.unBrowserHandle, false);
        }
    }
    
    private void OnOpenFileDialog(HTML_FileOpenDialog_t pCallback) {
        if(pCallback.unBrowserHandle == m_BrowserHandle) {
            SteamHTMLSurface.FileLoadDialogResponse(m_BrowserHandle, IntPtr.Zero);
        }
    }
    
    private void OnBrowserRestarted(HTML_BrowserRestarted_t pCallback) {
        if (pCallback.unOldBrowserHandle == m_BrowserHandle) {
            UpdateBrowserHandle(pCallback.unBrowserHandle);
        }
    }
    
    private void OnBrowserClosed(HTML_CloseBrowser_t pCallback) {
        if (pCallback.unBrowserHandle == m_BrowserHandle) {
            m_BrowserHandle = HHTMLBrowser.Invalid;
        }
    }
    
    public void SetSize(uint width, uint height) {
        m_Width = width;
        m_Height = height;
        
        if(m_BrowserHandle != HHTMLBrowser.Invalid) {
            SteamHTMLSurface.SetSize(m_BrowserHandle, m_Width, m_Height);
        }
        
        m_Texture = null;
    }
    
    public void SetUrl(string url, string postData = null) {
        m_Url = url;
        m_PostData = postData;
        
        if(m_BrowserHandle != HHTMLBrowser.Invalid) {
            SteamHTMLSurface.LoadURL(m_BrowserHandle, m_Url, m_PostData);
        }
    }
    
    public Texture2D GetTexture() {
        return m_Texture;
    }
    
    public bool IsDirty() {
        return m_IsDirty;
    }
    
    public void ClearIsDirty() {
        m_IsDirty = false;
    }
}
