---
title: "Unity Project Context"
type: context
status: living
updated: 2026-11-05
---

# Unity Project Context

- Unity project root and Git repository: `C:\Deadbeat-Dad-Simulator`.
- Baseline inspected: `8d9c1a5`; working branch `codex/unity-init`.
- Foundation: Unity 6000.4.4f1, URP, Input System. See [ADR-0006](../decisions/ADR-0006-unity-prototype-foundation.md).
- Assets: existing single-house and five-unit compound FBXs, source layouts, Node generators under `tools/`. Preserve the [approved house scale](../decisions/ADR-0005-starter-house-layout.md).
- Runtime: `Assets/Scripts/FirstPersonWalker.cs`, CharacterController-based first-person walking, mouse and touch input. No gameplay framework, networking, transport, fishing, or save system.
- Authoring/checks: `Assets/Editor/BlockoutSetup.cs`. Menu `Bapak Simulator` creates the blockout and validates fifteen doorways, floor/wall collision and character scale. `Create` replaces the current scene; save work before invoking it.
- Intended startup scene: `Assets/Scenes/kontrakan-blockout.unity`.
- Docs remain in `docs/`, outside `Assets/` but inside the project. Follow root `AGENTS.md` and existing ADR authority.
- Windows/Android are targets; verify actual build and device status in the [initialization handoff](../journal/2026-11-05-unity-initialization.md).
