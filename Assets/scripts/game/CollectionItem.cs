using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace DefaultNamespace.game
{
    public class CollectionItem: MonoBehaviour
    {
        public SpriteRenderer sprite;
        public TMP_Text text;
        public GameObject curtain;
        public string fishName;

        public async UniTask<bool> revealIfNeeded(FishData fishData)
        {
            if (fishData.Name != fishName)
            {
                return false;
            }
            sprite.DOColor(Color.white, 0.25f);
            var material =  curtain.GetComponent<Renderer>().material;
            await material.DOFloat(1f, "_Progress", 0.25f).ToUniTask();
            text.text = fishData.HiddenDescription;
            await material.DOFloat(0f, "_Progress", 0.25f).ToUniTask();
            return true;
        }
    }
}