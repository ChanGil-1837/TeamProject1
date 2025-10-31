using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace TeamProject.Title
{
    public class TitleSceneManager : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float zoomDuration = 1.5f; // 줌인에 걸리는 시간
        [SerializeField] private Ease zoomEase = Ease.InOutSine; // 줌인 애니메이션 방식

        [Header("Scene Settings")]
        [SerializeField] private string nextSceneName = "02_InGame"; // 이동할 씬 이름

        private bool isStarting = false; // 중복 실행 방지 플래그

        /// <summary>
        /// 게임 시작 시퀀스를 실행합니다.
        /// </summary>
        public void StartGameSequence()
        {
            if (isStarting) return;
            isStarting = true;

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main Camera not found in the scene!");
                isStarting = false;
                return;
            }

            // DOTween을 사용하여 카메라의 Field of View를 0으로 애니메이션
            mainCamera.DOFieldOfView(0, zoomDuration)
                .SetEase(zoomEase)
                .OnComplete(() => {
                    // 애니메이션 완료 후 씬 이동
                    SceneManager.LoadScene(nextSceneName);
                });
        }
    }
}
