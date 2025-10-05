using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Book : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform target;

    public async UniTask swim()
    {
        transform.DOMoveX(target.position.x, 4f).SetEase(Ease.Linear);
        transform.DOMoveY(target.position.y + 0.1f, 0.4f).SetLoops(-1, LoopType.Yoyo);
        await UniTask.WaitForSeconds(4f);
    }
}
