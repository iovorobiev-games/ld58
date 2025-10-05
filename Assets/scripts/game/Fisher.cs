using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace.game
{
    public class Fisher : MonoBehaviour
    {
        private Animator animator;
        private RodString rodString;
        private AudioSource audioSource;
        public List<AudioClip> joySounds = new();
        public List<AudioClip> sadSounds = new();
        public AudioClip throwStartSound;
        public AudioClip throwEndSound;

        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            rodString = DI.sceneScope.getInstance<RodString>();
        }

        public async UniTask prepareThrow()
        {
            audioSource.PlayOneShot(throwStartSound);
            animator.Play("throw");
        }

        public async UniTask startFishing(int power)
        {
            audioSource.PlayOneShot(throwEndSound);
            animator.Play("actually_throw");
            await UniTask.Delay(250);
            await rodString.throwHook(3.5f * power, power, 1);
        }

        public async UniTask pullHook(bool happy)
        {
            if (happy)
            {
                audioSource.PlayOneShot(joySounds[Random.Range(0, joySounds.Count)]);
            }
            else
            {
                audioSource.PlayOneShot(joySounds[Random.Range(0, sadSounds.Count)]);
            }
            await rodString.pullHook();
            animator.Play("idle");
        }
    }
}