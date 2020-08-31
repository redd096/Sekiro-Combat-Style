namespace redd096
{
    using UnityEngine;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject pauseMenu = default;
        [SerializeField] GameObject endMenu = default;

        public void PauseMenu(bool active)
        {
            if(pauseMenu)
                pauseMenu.SetActive(active);
        }

        public void EndMenu(bool active)
        {
            if(endMenu)
                endMenu.SetActive(active);
        }
    }
}