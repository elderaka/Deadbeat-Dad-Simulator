---
title: "Unity Project Context"
type: context
status: living
updated: 2026-11-05
---

# Unity Project Context

- Unity project root and Git repository: `C:\Deadbeat-Dad-Simulator`.
- Baseline inspected: `b3f17ca`; working branch `codex/unity-init` (2026-11-05 verification).
- Foundation: Unity 6000.4.4f1, URP, Input System. See [ADR-0006](../decisions/ADR-0006-unity-prototype-foundation.md).
- Assets: existing single-house and five-unit compound FBXs, source layouts, Node generators under `tools/`. Preserve the [approved house scale](../decisions/ADR-0005-starter-house-layout.md).
- Runtime: `Assets/Scripts/FirstPersonWalker.cs`, CharacterController-based first-person walking, mouse and touch input. No gameplay framework, networking, transport, fishing, or save system.
- Authoring/checks: `Assets/Editor/BlockoutSetup.cs`. Menu `Bapak Simulator` creates the blockout and validates fifteen doorways, floor/wall collision and character scale. `Create` replaces the current scene; save work before invoking it.
- Intended startup scene: `Assets/Scenes/kontrakan-blockout.unity`.
- Docs remain in `docs/`, outside `Assets/` but inside the project. Follow root `AGENTS.md` and existing ADR authority.
- Windows/Android are targets; verify actual build and device status in the [initialization handoff](../journal/2026-11-05-unity-initialization.md).

## Verified continuation checkpoint — 2026-11-05

- Confirmed URP 17.4.0 and Input System 1.19.0 in `Packages/manifest.json`; graphics settings reference a render-pipeline asset and `activeInputHandler: 1` selects Input System. Unity Test Framework 1.6.0 is installed; no first-party test assemblies or `.asmdef`/`.asmref` files were found under `Assets/`. Runtime and editor scripts use Unity's default assemblies.
- `EditorBuildSettings.asset` enables only `Assets/Scenes/kontrakan-blockout.unity`; SampleScene remains template content. No custom scene-loading system was found.
- Code is a small MonoBehaviour controller plus a static editor authoring/validation utility; namespace `BapakSimulator`, private serialized fields, no gameplay service framework or networking implementation. Multiplayer Center is an editor package, not evidence of multiplayer gameplay.
- Fresh `node tools/check-fbx.mjs` passes both FBXs. Fresh Unity batch execution of `BlockoutSetup.Validate` reports fifteen doorway checks, five walls/floors, 1.65m character, movement normalization and camera pitch checks passing, with process return code 0. **Caveat:** manual `SendMessage("Awake")` on the edit-mode movement probe also logs `ShouldRunBehaviour()` assertion at `BlockoutSetup.cs:112`. Do not describe this as an error-free validation run.
- Prior Windows build success is supported by the initialization log and existing executable; no rebuild was run during this continuation. The existing build was launched and its rendered entrance view inspected. Hands-on traversal and Android/device validation remain unverified.
- Official Pipeline 0.10.0-exp.1 is installed; batch log shows it stopping on exit. No live CLI connection was established; `unity` is not on this session's PATH. Package documentation read: `Documentation~/connectivity.md`. No Unity MCP tools are exposed in this chat.
- Unity generated untracked `Assets/Resources.meta` during validation; it was retained, not staged. No intentional scene, model, runtime, package, or project-settings edits were made.
- See [continuation handoff](../journal/2026-11-05-project-verification.md) for PR #2 integration conflicts and the next documentation task.
