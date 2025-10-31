using System.Collections;
using UnityEngine;

namespace TeamProject.GameSystem
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        private Coroutine _restoreTimeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// 시간을 0.1배속으로 느리게 만듭니다.
        /// </summary>
        public void SlowDownTime()
        {
            Time.timeScale = 0.1f;
        }

        /// <summary>
        /// 시간을 원래 속도로 복구합니다.
        /// </summary>
        public void RestoreTime()
        {
            Time.timeScale = 1.0f;
        }

        /// <summary>
        /// 주어진 시간(초) 동안 시간을 느리게 했다가 원래 속도로 복구합니다.
        /// </summary>
        /// <param name="duration">느리게 할 시간(초)</param>
        public void SlowDownForDuration(float duration)
        {
            if (_restoreTimeCoroutine != null)
            {
                StopCoroutine(_restoreTimeCoroutine);
            }
            SlowDownTime();
            _restoreTimeCoroutine = StartCoroutine(RestoreTimeAfter(duration));
        }

        private IEnumerator RestoreTimeAfter(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            RestoreTime();
            _restoreTimeCoroutine = null;
        }

        private void OnDestroy()
        {
            // 씬이 변경되거나 오브젝트가 파괴될 때 타임스케일을 원래대로 복구
            RestoreTime();
        }
    }
}
