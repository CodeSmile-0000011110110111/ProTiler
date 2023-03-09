using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBG.Toolbelt.Optimization;

namespace SBG.Toolbelt.Demo
{
	public class CubeSpawner : FrameSkipper
	{
        [SerializeField] private int cubesToSpawn = 1000;
        [SerializeField] private int cubesPerUpdate = 10;
       

        private int _cubeCount = 0;
        private int _frameCount = 0;
        private int _totalFPS = 0;
        private float _startTime = 0;

        private bool _useFrameSkipper;
        private bool _spawnCubes = false;


        private void Start()
        {
            StartCoroutine(SpawnRoutine(false, 0.5f));
        }

        protected override void Update()
        {
            base.Update();

            if (_spawnCubes)
            {
                int currentFPS = Mathf.FloorToInt(1 / Time.deltaTime);
                _totalFPS += currentFPS;
                _frameCount++;
            }

            if (!_spawnCubes || _useFrameSkipper) return;

            SpawnCubes(-2);
        }

        protected override void DelayedUpdate()
        {
            if (!_spawnCubes || !_useFrameSkipper) return;

            SpawnCubes(2);
        }

        private void SpawnCubes(float xPos)
        {
            for (int i = 0; i < cubesPerUpdate; i++)
            {
                Vector3 pos = new Vector3(xPos, 0, _cubeCount);
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = pos;
                _cubeCount++;
            }

            if (_cubeCount >= cubesToSpawn)
            {
                float duration = Time.time - _startTime;
                Debug.Log($"Finished in {duration} Seconds with an average FPS of {_totalFPS/_frameCount:0}");

                _spawnCubes = false;

                if (!_useFrameSkipper) StartCoroutine(SpawnRoutine(true, 3));
            }
        }

        private IEnumerator SpawnRoutine(bool frameSkipping, float startDelay)
        {
            yield return new WaitForSeconds(startDelay);

            Debug.Log($"Spawning {cubesToSpawn} Cubes. {cubesPerUpdate} Cubes per Update.");
            _useFrameSkipper = frameSkipping;
            if (!_useFrameSkipper) Debug.Log("[Using regular Update]");
            else Debug.Log("[Using FrameSkipper]");

            yield return new WaitForSeconds(3);

            _cubeCount = 0;
            _frameCount = 0;
            _totalFPS = 0;
            _startTime = Time.time;
            _spawnCubes = true;
        }
    }
}