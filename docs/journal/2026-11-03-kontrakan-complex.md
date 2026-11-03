---
title: "2026-11-03 — Five-Unit Kontrakan Complex Graybox"
type: journal
status: current
date: 2026-11-03
tags: [assets, environment, blockout, brainstorm]
---

# Five-Unit Kontrakan Complex — Implementation Note

## Context
The creator liked the generated 5-unit courtyard concept and asked whether to implement immediately or discuss further. We proceeded with a **separate first-pass 3D graybox** rather than canonizing every environmental detail.

## Decisions preserved
- Existing 3.5 × 9m, three-room starter house, Bapak 1.65m reference.
- Each unit's main entrance, right-offset first internal passage and near-centered second internal passage.
- Previously documented bathroom access remains an assumed opening, not exact approved denah geometry.
- Multiplayer stays later, not MVP.

## Proposed site (NOT canonical)
- Five adjacent row houses; player in U01.
- 19.5 × 17m lot, 5.3m independent shared forecourt, 1.5m rear service, 1m side clearances, 3.2m gate.
- Wider than the **18 × 14.5m conceptual illustration**, because that image's dimensions overlap terrace/courtyard and leave inadequate side passages.

## Work
- Created `Assets/Housing/kontrakan-row-5-v01.fbx` from a source-controlled deterministic FBX serializer.
- Added `Assets/Housing/layout.json` and asset import notes.
- Added `tools/generate_kontrakan_complex.mjs` for regeneration from Node.
- Meshes named by unit and shared component; continuous roof split into two removable sections.
- Added basic compound objects for staging: gate opening, shared court, parked-bike markers, social bench/planter and utility corner.

## Verification / limitations
Generated binary FBX and inspected mesh/triangle counts during generation. No Unity editor launch, physics, NavMesh, textures, or character interaction tests performed. No playable build.

## Suggested next step
Import into Unity with a 1.65m reference capsule. Walk the passage widths and side corridors with roof hidden. Only then add windows, exterior variations, NPC residents and interaction hooks. A high-quality architectural concept rendering is **not a metrically verified architectural plan**.
