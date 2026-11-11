using System;
using System.Collections;
using UnityEngine;

namespace BapakSimulator
{
    [DisallowMultipleComponent]
    public sealed class SceneSequence : MonoBehaviour
    {
        [Serializable]
        private sealed class Step
        {
            public Transform cameraPose;
            [Min(0)] public float cameraMoveSeconds;
            public Animation animation;
            public string dialogueNode;
            [Min(0)] public float holdSeconds;
        }

        [SerializeField] private PlayerController player;
        [SerializeField] private Camera view;
        [SerializeField] private NeighborhoodDialogue dialogue;
        [SerializeField] private Canvas gameplayHud;
        [SerializeField] private Step[] steps;

        private Action completed;
        private Vector3 cameraPosition;
        private Quaternion cameraRotation;
        private bool hudWasEnabled;
        private bool playing;
        private bool speaking;

        public void Play() => TryPlay(null);

        internal bool TryPlay(Action onCompleted)
        {
            if (!isActiveAndEnabled || playing || player == null || view == null ||
                !player.CanOpenDialogue || steps == null || steps.Length == 0)
                return false;

            completed = onCompleted;
            cameraPosition = view.transform.localPosition;
            cameraRotation = view.transform.localRotation;
            hudWasEnabled = gameplayHud != null && gameplayHud.enabled;
            playing = true;
            player.SetSequenceInputBlocked(true);
            if (gameplayHud != null) gameplayHud.enabled = false;
            StartCoroutine(PlaySteps());
            return true;
        }

        private IEnumerator PlaySteps()
        {
            foreach (var step in steps)
            {
                if (step.cameraPose != null)
                {
                    var cameraTransform = view.transform;
                    var startPosition = cameraTransform.position;
                    var startRotation = cameraTransform.rotation;
                    for (float elapsed = 0; elapsed < step.cameraMoveSeconds; elapsed += Time.deltaTime)
                    {
                        float t = Mathf.SmoothStep(0, 1, elapsed / step.cameraMoveSeconds);
                        cameraTransform.SetPositionAndRotation(
                            Vector3.Lerp(startPosition, step.cameraPose.position, t),
                            Quaternion.Slerp(startRotation, step.cameraPose.rotation, t));
                        yield return null;
                    }
                    cameraTransform.SetPositionAndRotation(step.cameraPose.position, step.cameraPose.rotation);
                }

                while (player.IsPaused) yield return null;
                if (step.animation != null) step.animation.Play();
                if (step.holdSeconds > 0) yield return new WaitForSeconds(step.holdSeconds);

                if (!string.IsNullOrWhiteSpace(step.dialogueNode) && dialogue != null)
                {
                    while (player.IsPaused) yield return null;
                    speaking = true;
                    if (dialogue.TryStartSequenceLine(step.dialogueNode, () => speaking = false))
                        while (speaking) yield return null;
                    else speaking = false;
                }
            }
            Finish();
        }

        private void OnDisable()
        {
            if (!playing) return;
            StopAllCoroutines();
            if (speaking && dialogue != null) dialogue.StopSequenceLine();
            Finish();
        }

        private void Finish()
        {
            playing = false;
            speaking = false;
            if (view != null) view.transform.SetLocalPositionAndRotation(cameraPosition, cameraRotation);
            if (player != null) player.SetSequenceInputBlocked(false);
            if (gameplayHud != null) gameplayHud.enabled = hudWasEnabled;
            var callback = completed;
            completed = null;
            callback?.Invoke();
        }
    }
}
