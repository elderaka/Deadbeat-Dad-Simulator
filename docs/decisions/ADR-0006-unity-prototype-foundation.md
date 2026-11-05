---
title: "ADR-0006 — Unity Prototype Foundation"
type: decision
status: accepted
date: 2026-11-05
---

# Unity Prototype Foundation

## Creator decision

The creator approved the setup questionnaire on 2026-11-05, with these changes:

- Repository root is the Unity project root at `C:\Deadbeat-Dad-Simulator`. Keep `Assets/`, `docs/`, and `tools/` within it.
- Use installed Unity 6000.4.4f1 and URP. Use official Unity CLI and Pipeline if compatible; do not modify another project's Coplay setup.
- Design for Windows and Android compatibility.
- First-person camera for immersion. Only Bapak is directly controlled, preserving [ADR-0003](ADR-0003-player-control.md).
- First milestone is a simple walkable blockout with scale, movement, camera, and passage collision checks. The five-unit compound is a provisional test environment, not a finalized neighborhood decision.
- Approachable simulation with comedic consequences; Indonesian first, English also intended. The initial blockout HUD may be Indonesian only.
- Transport and fishing are the current two gameplay directions, explicitly subject to further planning. Neither system is part of this initialization milestone.

## Consequences

Desktop movement uses WASD and mouse look; Android needs touch movement and look. Android compatibility is a target, not evidence of device validation. Do not implement multiplayer, economy, family simulation, localization infrastructure, transport, or fishing during initialization.

The approved house dimensions and door offsets remain unchanged. The bathroom opening and compound dimensions remain provisional. Further brainstorming follows initiation.
