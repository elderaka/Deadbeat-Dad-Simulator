---
title: "2026-11-02 — Rumah Tiga Petak Graybox"
type: journal
status: current
date: 2026-11-02
tags: [assets, house, geometry, blockout]
---

# Rumah 3 Petak — House Blockout

## Creator confirmation
The creator approved the starter house proposal **with the condition that doors follow the illustrated denah**.

## Asset details
Generated a low-poly FBX Binary 7.4 house graybox under `Assets/House/` with 19 separately named meshes and a removable two-part roof. Designed for Y-up, 1 unit = 1 metre. Bapak height benchmark 1.65 m. Enclosed house 3.5 × 9 m, front terrace 1.2 m, walls 2.8 m.

## Critical denah constraint
- Front main entry centered.
- Living/bedroom interior door **right offset**, center X +0.76 at Z 3.
- Bedroom/kitchen interior door **near center**, X −0.055 at Z 6.
- Rear-right bathroom: opening at Z 7 / X +1.05 m is **additional provisional geometry**, because the reference denah omitted a bathroom door.

## Files
- `Assets/House/rumah-3-petak-v01.fbx` (binary)
- `Assets/House/layout.json` (precise opening locations)
- `Assets/House/README.md` (Unity notes)
- `tools/generate_house.mjs` (deterministic regeneration without Blender)
- `docs/decisions/ADR-0005-starter-house-layout.md` (canon)

## Verification and limitations
FBX binary structure and dimensions were checked during generation, and a companion GLB local validation of equivalent geometry loaded as 19 mesh groups. **Actual Unity Editor import has not been performed** in this environment. No game colliders, navmesh, furniture, wall-cutaways or animated doors. Roof meshes can be disabled for an isometric view.

## Next action
Import FBX in Unity, place a 1.65 m reference capsule, verify door placement from above, test mesh import scale, then add colliders only to geometry that actually blocks movement. Clarify bathroom door location if the user wants exact plan fidelity.
