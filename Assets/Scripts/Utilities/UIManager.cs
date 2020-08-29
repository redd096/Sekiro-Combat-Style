namespace redd096
{
    using UnityEngine;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject pauseMenu = default;

        public void PauseMenu(bool active)
        {
            pauseMenu.SetActive(active);
        }
    }
}