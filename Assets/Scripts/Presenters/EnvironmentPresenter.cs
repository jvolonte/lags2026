using System.Threading.Tasks;
using Animation;
using UnityEngine;

namespace Presenters
{
    public class EnvironmentPresenter : MonoBehaviour
    {
        [SerializeField] ScreenFader fader;

        [SerializeField] GameObject dayEnv;
        [SerializeField] GameObject nightEnv;

        GameObject current;

        void Awake()
        {
            CombatEventManager.OnChangeTimeOfDay += HandleTimeChanged;
            SetEnvironment(TimeOfDay.Day);
        }

        void OnDestroy() =>
            CombatEventManager.OnChangeTimeOfDay -= HandleTimeChanged;

        async void HandleTimeChanged(TimeOfDay time)
        {
            await TransitionTo(time);
        }

        async Task TransitionTo(TimeOfDay time)
        {
            await fader.FadeOut();
            SetEnvironment(time);
            await fader.FadeIn();
        }

        void SetEnvironment(TimeOfDay time)
        {
            if (current != null)
                current.SetActive(false);

            current = time switch
            {
                TimeOfDay.Day => dayEnv,
                TimeOfDay.Night => nightEnv,
                _ => current
            };

            current.SetActive(true);
        }
    }
}