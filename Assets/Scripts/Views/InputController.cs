using UnityEngine;
using UnityEngine.InputSystem;

namespace Views
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance { get; private set; }

        InputAction continueAction;

        public bool ContinueRequested { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            continueAction = new InputAction(
                type: InputActionType.Button,
                binding: "*/<button>"
            );
        }

        void OnEnable()
        {
            continueAction.performed += OnContinue;
            continueAction.Enable();
        }

        void OnDisable()
        {
            continueAction.performed -= OnContinue;
            continueAction.Disable();
        }

        void OnContinue(InputAction.CallbackContext _) => ContinueRequested = true;

        public bool ConsumeContinue()
        {
            if (!ContinueRequested)
                return false;

            ContinueRequested = false;
            return true;
        }
    }
}