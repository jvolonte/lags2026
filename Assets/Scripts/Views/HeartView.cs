using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HeartView : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> SpritePool;

    [SerializeField]
    private List<Sprite> BrokenSpritePool;

    private Sprite heartSprite;
    private Sprite brokenHeartSprite;

    private Image imageComponent;

    public bool Broken { get; private set; } = true;

    void Awake()
    {
        heartSprite = SpritePool[Random.Range(0, SpritePool.Count)];
        brokenHeartSprite = BrokenSpritePool[Random.Range(0, BrokenSpritePool.Count)];

        imageComponent = GetComponent<Image>();
        Break();
    }

    public void Break() {
        if (!Broken) 
            imageComponent.sprite = brokenHeartSprite;
        Broken = true;
    }

    public void Recover() {   
        if (Broken) 
            imageComponent.sprite = heartSprite;
        Broken = false;
    }
}
