using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameAnalyticsSDK.Setup;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace DefaultNamespace.game
{
    public class FishSpawner : MonoBehaviour
    {
        public List<(Vector3, Vector3)> spawnPositions = new()
        {
            (new Vector3(-5f, -0.5f, 0f), new Vector3(2.5f, -0.5f, 0f)),
            (new Vector3(-5f, -3f, 0f), new Vector3(2.5f, -3f, 0f)),
            (new Vector3(-5f, -5.5f, 0f), new Vector3(2.5f, -5.5f, 0f)),
        };
        private GameObject fishPrefab;

        private void Awake()
        {
            DI.sceneScope.register(this);
        }

        public async UniTask<List<FishData>> InitialSpawn(int countForEachDepth)
        {
            if (fishPrefab == null)
            {
                fishPrefab = await Resources.LoadAsync("Prefabs/FishCont").ToUniTask() as GameObject;
            }
            var result = new List<FishData>();
            var depth = 0;
            foreach (var spawnPosition in spawnPositions)
            {
                
                for (var i = 0; i < countForEachDepth; i++)
                {
                    var x = Random.Range(spawnPosition.Item1.x, spawnPosition.Item2.x);
                    var fishObject = Instantiate(fishPrefab, new Vector3(x, spawnPosition.Item1.y, spawnPosition.Item1.z),
                        Quaternion.identity);
                    var data = FishDB.getRandomFishOfDepth(depth);
                    result.Add(data);
                    fishObject.GetComponent<Fish>().data = data;
                }
                depth++;
            }

            return result;
        }

        public FishData SpawnOnDepth(int depth)
        {
            var spawnPosition = spawnPositions[depth];
            var fishObject = Instantiate(fishPrefab, Random.Range(0f, 1f) < 0.5f ? spawnPosition.Item1 : spawnPosition.Item2,
                Quaternion.identity);
            var fish = FishDB.getRandomFishOfDepth(depth);
            fishObject.GetComponent<Fish>().data = fish;
            return fish;
        }

        public void Spawn(FishData fishData)
        {
            var spawnPosition = spawnPositions[fishData.Depth]; 
            var fishObject = Instantiate(fishPrefab, Random.Range(0f, 1f) < 0.5f ? spawnPosition.Item1 : spawnPosition.Item2,
                Quaternion.identity);
            fishObject.GetComponent<Fish>().data = fishData;

        }
    }
}