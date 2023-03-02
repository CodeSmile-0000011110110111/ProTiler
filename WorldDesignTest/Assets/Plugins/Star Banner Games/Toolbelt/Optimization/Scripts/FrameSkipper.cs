using UnityEngine;

namespace SBG.Toolbelt.Optimization
{
    /// <summary>
    /// Inherit from FrameSkipper to create a MonoBehaviour,
    /// which features a custom "DelayedUpdate", that only executes after a set number of frames have passed.
    /// Use this to execute expensive function that dont need to be called every single frame,
    /// such as AI Pathfinding, etc.
    /// </summary>
	public abstract class FrameSkipper : MonoBehaviour
	{
		[Range(1,60)]
		[SerializeField] private int framesToSkip = 1; //Frames skipped until the next Update is called
        [SerializeField] private bool randomStartOffset = true;

		public int FramesToSkip => framesToSkip;

        /// <summary>
        /// The time since the last executed Frame
        /// </summary>
        public float SkippedDeltaTime { get; private set; }


		private int _skippedFrames = 0;


        protected virtual void Awake()
        {
            if (randomStartOffset) _skippedFrames = Random.Range(0, framesToSkip);
        }

        protected virtual void Update()
        {
            if (_skippedFrames < framesToSkip)
            {
                _skippedFrames++;
                SkippedDeltaTime += Time.deltaTime;
            }
            else
            {
                DelayedUpdate();

                _skippedFrames = 0;
                SkippedDeltaTime = 0;
            }
        }

        /// <summary>
        /// DelayedUpdate is called every time the defined number of frames have been rendered.
        /// </summary>
        protected abstract void DelayedUpdate();
    }
}