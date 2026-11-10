using System;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

namespace BapakSimulator
{
    [DisallowMultipleComponent]
    public sealed class NeighborhoodDialogue : MonoBehaviour
    {
        [SerializeField] private DialogueRunner runner;
        [SerializeField] private PlayerController player;

        private UnityEngine.UI.GraphicRaycaster dialogueRaycaster;
        private Func<string, bool> performAction;
        private bool active;

        private void Awake()
        {
            if (runner == null || runner.YarnProject == null || player == null)
            {
                Debug.LogError($"Neighborhood Dialogue on '{name}' needs a Yarn project and player.", this);
                enabled = false;
                return;
            }

            dialogueRaycaster = runner.GetComponentInChildren<UnityEngine.UI.GraphicRaycaster>(true);
            if (dialogueRaycaster != null) dialogueRaycaster.enabled = false;
            runner.AddCommandHandler("npc_action", (Action<string>)PerformAction);
        }

        private void OnDisable()
        {
            if (active && runner != null) runner.Stop().Forget();
        }

        private void OnDestroy()
        {
            if (runner != null) runner.RemoveCommandHandler("npc_action");
        }

        internal bool TryStart(string node, Action<VariableStorageBehaviour> prepare, Func<string, bool> action, Action onClosed = null)
        {
            if (!isActiveAndEnabled || active || !player.CanOpenDialogue || runner.IsDialogueRunning ||
                string.IsNullOrWhiteSpace(node) || !runner.YarnProject.NodeNames.Contains(node))
                return false;

            Run(node, prepare, action, onClosed).Forget();
            return true;
        }

        private async YarnTask Run(string node, Action<VariableStorageBehaviour> prepare, Func<string, bool> action, Action onClosed = null)
        {
            active = true;
            if (dialogueRaycaster != null) dialogueRaycaster.enabled = true;
            performAction = action;
            var previousLock = Cursor.lockState;
            bool previousVisibility = Cursor.visible;
            player.SetDialogueInputBlocked(true);
            player.SetWorldInteractionBlocked(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            try
            {
                runner.VariableStorage.SetValue("$action_ok", false);
                prepare(runner.VariableStorage);
                await runner.StartDialogue(node);
                await runner.DialogueTask;
            }
            finally
            {
                performAction = null;
                active = false;
                if (dialogueRaycaster != null) dialogueRaycaster.enabled = false;
                if (player != null)
                {
                    player.SetDialogueInputBlocked(false);
                    player.SetWorldInteractionBlocked(false);
                }
                Cursor.lockState = previousLock;
                Cursor.visible = previousVisibility;
                onClosed?.Invoke();
            }
        }

        private void PerformAction(string action) =>
            runner.VariableStorage.SetValue("$action_ok", performAction?.Invoke(action) == true);
    }
}
