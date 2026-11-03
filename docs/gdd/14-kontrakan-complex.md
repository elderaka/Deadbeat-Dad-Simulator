---
title: "14 — Kontrakan Deret Five-Unit Site"
status: proposed
type: design
tags: [gdd, environment, graybox]
updated: 2026-11-03
---

# 14 — Kontrakan Deret Five-Unit Site

> **PROPOSED environment extension.** The creator liked the concept image and asked us to try a complex, but the final site footprint, residents and detailed architecture remain open.

## Core location fantasy

Bapak's household occupies **Unit 1** of a modest, five-door row of family rental houses (kontrakan deret / rumah petak lima pintu) in a fictional industrial suburb. The shared courtyard provides social space for gossip, chores, neighbor help, and children, creating the social ecosystem the standalone house lacks.

## Built first-pass graybox

The [FBX](../../Assets/Housing/kontrakan-row-5-v01.fbx), [site data](../../Assets/Housing/layout.json) and [asset README](../../Assets/Housing/README.md) live in `Assets/Housing/`.

| Element | Proposed dimension |
|---|---:|
| Units | 5 |
| Individual house | **3.5 × 9.0 m**, 2.8m walls |
| Whole row | 17.5m wide |
| Lot / blockout | **19.5 × 17m** |
| Front terrace | 1.2m depth per unit |
| Shared forecourt | 5.3m depth (separate from terraces) |
| Rear service strip | 1.5m |
| Side clearances | 1m on each side |
| Entry gate | 3.2m wide opening |
| Character reference | 1.65m Bapak |

### Dimensional correction to the generated denah concept

The previous concept art showed **18 × 14.5m** but labeled 9m enclosed house + 1.2m terrace + 4m independent court + 1.5m rear strip = **15.7m**, not 14.5m. An 18m width for 17.5m row also leaves only 0.25m on either side. The **19.5 × 17m** blockout is an intentional, provisional adjustment for believable circulation—not a revision to the accepted house module.

## Explicit door preservation

All five units preserve the approved [house ADR](../decisions/ADR-0005-starter-house-layout.md):
- Main entry centered, front Z=0
- Living→bedroom doorway **right-offset local X=+0.76m**, Z=3
- Bedroom→kitchen doorway **near-center local X=−0.055m**, Z=6
- Bathroom access local X=+1.05m, Z=7 remains provisional

No new windows or interactive door leaves are part of this graybox.

## Shared environment props (blockout only)

- One continuous two-part removable roof, not five interpenetrating roofs.
- Four parking bay markings, plus path to the central front gate.
- Communal planter and bench placeholder forms.
- Left/right access strips and rear service lane.
- Shared utility/tank and laundry line placeholders.
- Concrete court, drainage line and low compound boundary wall.

## Gameplay opportunities (not yet implemented)

Five units imply small, memorable NPC households with distinct schedules, not five equally complex simulations. Communal requests and reciprocal arrangements can connect to [household control](13-household-control.md) without a quest-heavy open world.

## MVP constraints

In the first playable slice, only **Unit 1 needs a traversable detailed interior**. Other units may have locked or simplified interiors and basic schedule NPCs. Never generate five full detailed households before validating the two-day gameplay loop.

## Important limitations

Procedural FBX was generated in code (named meshes, graybox colors, openings); Unity Editor import and collisions remain untested. Exact lot size, neighbor roster, roof style, exterior windows, visual detail, site entrance mechanics and outside-world connections are proposals.

See [asset README](../../Assets/Housing/README.md) and [design journal](../journal/2026-11-03-kontrakan-complex.md).
