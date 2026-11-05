# Deadbeat Dad Simulator (Bapak Simulator)

A comedic Indonesian household-and-leisure simulator about **earning the freedom to do absolutely nothing** while helping your family thrive.

This repository begins as a **design-first project**. The modular, Obsidian-compatible game design vault is being prepared in [`docs/`](docs/README.md). Canonical design decisions, proposals, experiments, and open questions are kept separate.

> **Design principle:** The better Bapak becomes at managing life, the less he is required to do personally.

## Unity prototype

This repository root is also the Unity project root. Use Unity **6000.4.4f1** (URP, Input System). Assets, docs and generators all stay inside the project.

Open `Assets/Scenes/kontrakan-blockout.unity`, then press Play. Desktop: WASD to walk, mouse to look, Escape to release the cursor, click to resume. Android: drag the left half to walk and the right half to look, in landscape. Device validation and Android build tooling status are recorded in the [initialization handoff](docs/journal/2026-11-05-unity-initialization.md).

The scene is a provisional five-unit house blockout with a 1.65m character. Transport, fishing, NPC simulation and English UI are later work. See [approved foundation](docs/decisions/ADR-0006-unity-prototype-foundation.md).

Editor menu `Bapak Simulator > Validate Blockout` runs the collision/scale/movement smoke check. `Create Walkable Blockout` regenerates the scene; do not use it to preserve manual scene edits.
