using UnityEngine;

namespace TeamProject.Title
{
    [RequireComponent(typeof(Collider))] // 클릭 감지를 위해 Collider가 필수임을 명시
    public class ClickableObject : MonoBehaviour
    {
        private TitleSceneManager titleManager;

        private void Start()
        {
            // 씬에 있는 TitleSceneManager를 찾아서 캐싱
            titleManager = FindObjectOfType<TitleSceneManager>();
            if (titleManager == null)
            {
                Debug.LogError("TitleSceneManager not found in the scene!");
            }
        }

        private void OnMouseDown()
        {
            if (titleManager != null)
            {
                // 클릭 시 StartGameSequence 메서드 호출
                titleManager.StartGameSequence();
            }
        }
    }
}
