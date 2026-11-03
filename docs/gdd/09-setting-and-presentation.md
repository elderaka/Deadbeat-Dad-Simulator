---
title: "09 — Setting, Camera and Presentation"
status: proposed
type: design
tags: [gdd]
updated: 2026-11-02
---

# 09 — Setting, Camera and Presentation

> **Status: PROPOSED** — Canonical = approved direction; Proposed = hypothesis to validate; Open = undecided. Numeric examples are not final balancing.

## Place fantasy
A recognizable Indonesian industrial suburb inspired by the Cikarang–Karawang–Bekasi atmosphere: factory shift traffic, narrow neighborhood roads, school gates, warungs, modest houses and local fishing ponds.

**Do not claim a fully researched real-world map.** Fictionalize names and geography; specific landmarks and factual detail require later research.

## Candidate minimum map
Starter house / kontrakan, school, factory entrance, warung, fishing pond, road connections and a neighbor. Compact navigable neighborhood beats a huge empty open world.

## Approved starter house graybox
The creator accepted **1 unit = 1 meter**, **Bapak = 1.65 m**, and a **3.5 × 9 m three-bay starter kontrakan**. The doorway layout must match the drawn denah: centered exterior entry, right-offset living/bedroom doorway, near-centered bedroom/kitchen doorway. See [ADR-0005](../decisions/ADR-0005-starter-house-layout.md) and [asset layout](../../Assets/House/layout.json). The extra bathroom access opening is provisional because its location was not indicated in the reference diagram.

## Proposed five-unit kontrakan compound
The creator liked a five-door row-house courtyard environment as Bapak's starting neighborhood. A first-pass [five-unit graybox](14-kontrakan-complex.md) repeats the accepted 3.5×9m house modules while preserving their denah door offsets. The proposed **19.5×17m compound** corrects inconsistent site measurements in the generated concept image. This compound layout is not yet an accepted ADR.

## Camera
Isometric household management is attractive because schedules, family autonomy and upgrades are legible. A close third-person or first-person camera would heighten comedic immersion but raises animation, UI and movement complexity.

**No definitive camera choice has been approved.** Proposed compromise: isometric/3D scene with optional closer framing for minigames, validated in prototype.

## Presentation
Warm, approachable, lived-in, mildly ridiculous, not cyberpunk or generic tropical tourism imagery. Key feedback: clock, cash buffer, pending obligations, who is where, what automation changed and what family milestones occurred.

## Audio
Neighborhood ambience, fishing pond sounds, scooters, factory whistles and conversational barks. No production sound direction or licensed music selected.

## Accessibility
Readable schedule/task UI, captions, optional camera motion comfort, color-independent indicators and reasonable controls across platforms (to verify later).
