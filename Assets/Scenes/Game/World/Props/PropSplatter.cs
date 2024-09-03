using SFH;
using System.Collections.Generic;
using UnityEngine;

public class PropSplatter : MonoBehaviour
{
    [SerializeField] private GameObject propPrefab;
    [SerializeField] private Transform propsHolder;
    [SerializeField] private List<Sprite> propSpriteList = new List<Sprite>();
    [SerializeField] private float minDistance;
    [SerializeField, Range(0,1)] private float falloff;

    private void Start() {
        ScreenUtils.ScreenUtilsInterface screen = ScreenUtils.inst[Camera.main];
        List<Vector2> samples = FastPoissonDiskSampling.Sampling(screen.WorldBottomLeft(), screen.WorldTopRight(),minDistance);
        for (int i = 0; i < samples.Count; i++) {
            if (Random.value > falloff) continue;
            SpriteRenderer sr = Instantiate(propPrefab, samples[i], Quaternion.identity).GetComponent<SpriteRenderer>();
            sr.transform.SetParent(propsHolder);
            sr.sprite = propSpriteList.RandomItem();
        }
    }
}
