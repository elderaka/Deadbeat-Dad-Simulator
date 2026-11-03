---
title: "ADR-0005 — Starter House Scale and Denah"
type: decision
status: accepted
date: 2026-11-02
---

# ADR-0005 — Starter House Scale and Denah

## Context
The creator requested an FBX model of a typical Indonesian *rumah kontrakan tiga petak* and chose Bapak height of **165 cm** for scale. The proposal described a **single 3.5 × 9.0 m house** with three sequential rooms, rear-right bathroom, front terrace and interior doors positioned according to the previously drawn floor plan (*denah rumah*). The creator accepted the proposal with an explicit constraint: **stick to the denah for the door layout**.

## Accepted decisions
- Use **1 Unity unit = 1 meter** and **Bapak = 1.65 m** for visual reference. This is a game scaling choice, not a factual demographic claim.
- Starter enclosed house footprint **3.5 m width × 9 m depth**, subdivided at 3 m intervals: **front living room → middle bedroom → rear kitchen/service**.
- **2.8 m** wall height, **1.2 m** terrace in front, bathroom in **rear-right** corner.
- Door positions must match previously shown denah:
  - **Main entrance:** front façade, approximately centered.
  - **Living → bedroom:** on the **right side** of the interior partition (not centered).
  - **Bedroom → kitchen:** near the **center** of the second partition.
- Graybox first, with separate removable roof parts, not finished furniture or production textures.

## Blockout coordinates (approved intent represented in the v0.1 asset)
FBX Y-up; X across façade; Z toward the rear (z=0 front). See [layout.json](../../Assets/House/layout.json) for precise values.

| Opening | Wall z | X center | Width | Status |
|---|---:|---:|---:|---|
| Main entrance | 0 | 0.00 m | 0.90 m | Denah match |
| Living → bedroom | 3 | +0.76 m | 0.85 m | Denah right-offset |
| Bedroom → kitchen | 6 | −0.055 m | 0.85 m | Denah near-center |
| Kitchen → bathroom | 7 | +1.05 m | 0.70 m | **Assumed** |

## Explicit non-decision
The original denah labels the bathroom but **does not show its access door**. The bathroom doorway added to v0.1 is a functional modeling assumption, **not an approved exact placement**. Do not silently treat this fourth opening as canon.

## Artifacts
- [FBX](../../Assets/House/rumah-3-petak-v01.fbx)
- [Asset README](../../Assets/House/README.md)
- [Generator](../../tools/generate_house.mjs)

The FBX is generated and format-structured; **Unity import has not been tested**. Colliders, door leaves, windows, furniture and exact player walkability remain implementation tasks.

See [GDD Setting](../gdd/09-setting-and-presentation.md) and [Asset Work Journal](../journal/2026-11-02-house-blockout.md).
