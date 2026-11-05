---
title: "2026-11-05 — Unity Initialization"
type: handoff
status: living
date: 2026-11-05
---

# Unity Initialization

## Decisions and scope

Creator approved [ADR-0006](../decisions/ADR-0006-unity-prototype-foundation.md): first person, Windows/Android, Indonesian first with English intended, simple walkable compound prototype. Transport and fishing are provisional later gameplay directions.

## Work

- Cloned the existing GitHub repository and retained its history. Branch: `codex/unity-init`.
- Initializing the Unity project at repository root, keeping assets/docs/tools inside it.
- Reusing original FBX geometry; no house doorway or layout changes.
- Added first-person walking with desktop/touch input and an editor scene builder/physics smoke check.
- Added Unity ignore rules and [persistent project context](../agents/unity-project-context.md).

## Validation in progress

- Official CLI 1.0.0-beta.16 is installed and authenticated.
- Unity 6000.4.4f1 is installed; initial project creation uses its 3D URP template.
- Android build module installation stopped because additional module licenses require acceptance. No license acceptance was submitted.
- Compilation, scene generation, runtime and build results will be appended after checks complete.

## Android TODO — creator direction

The creator explicitly deferred Android SDK/NDK/Java installation because storage is limited. Do not install these modules until requested. Once storage is available: install Android Build Support and dependencies, accept the relevant licenses, build an APK, and test touch controls/performance on a physical device. Windows validation can proceed now.

## Next

Finish validation, then creator walk-through and further gameplay brainstorming. Do not treat the compound/bathroom assumptions or detailed transport/fishing mechanics as accepted production design.

## Automated check results

- Unity scene created at `Assets/Scenes/kontrakan-blockout.unity` and enabled as the build scene.
- Both original FBXs initially failed Unity import. Fixed the generators' missing timestamp/file-ID checksum metadata, footer alignment, empty-node sentinels and coordinate handedness conversion; regenerated both assets without changing source house dimensions/door offsets.
- Both generated files pass `node tools/check-fbx.mjs`.
- Unity physics/movement smoke check passes: fifteen doorways clear, five unit walls present, five floors present, 1.65m character, normalized diagonal walking speed and clamped camera pitch.
- Official Pipeline 0.10.0-exp.1 installed. Live Editor command connectivity is not yet verified; batchmode generation and checks do not prove an interactive connection.
- Creator is in the office and deferred manual walkthrough testing. No manual gameplay/device validation is claimed.
- Windows build result to be appended below.

## Final checkpoint

- Windows x64 build succeeded (`WINDOWS_BUILD_PASSED`); Unity exited batchmode with code 0.
- Build location: `Builds/Windows/Deadbeat-Dad-Simulator.exe` (ignored by Git).
- No manual playtest, touch/device test, visual review, or live Pipeline command connection is claimed. The creator deferred hands-on testing until later.
- All Unity batch processes used for validation have exited. No Editor needs to remain open.
- Source and generated Unity assets are retained on local branch `codex/unity-init`; no remote push was performed.
- Next action: creator opens `Assets/Scenes/kontrakan-blockout.unity` in Unity 6000.4.4f1 and walks the house/courtyard. Then brainstorm transport and fishing. Android tooling remains explicitly deferred for storage.
