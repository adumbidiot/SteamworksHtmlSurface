using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerBrowser : MonoBehaviour
{
    private SteamBrowser browser = null;
    public Image image;
    
    // Start is called before the first frame update
    void Start()
    {
        browser = GetComponent<SteamBrowser>();
        browser.SetUrl("https://twitch.tv");
    }

    // Update is called once per frame
    void Update()
    {
        if(browser.IsDirty()) {
            if(image.sprite) {
                // Sprites are not GC'ed
                Destroy(image.sprite);
            }
            
            Texture2D texture = browser.GetTexture();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1, 1));
            image.sprite = sprite;
            
            browser.ClearIsDirty();
        }
    }
}
