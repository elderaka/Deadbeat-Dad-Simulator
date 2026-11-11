---
title: "2026-11-11 — Progression and Post-Slice Milestones"
type: journal
status: living
updated: 2026-11-11
---

# Progression and Post-Slice Milestone Review — 2026-11-11

## Creator direction

The creator reports the bare playable Unity slice has been initiated and asks for a connected progression design plus the next roadmap milestone. Discussed: vehicle progression, fishing-spot progression (paid pond versus free river, optional clearly restricted place), house progression, Ibu career opportunities, Anak's evolving autonomy/interests affecting gameplay and longer-term endings, and eventually nambak/vehicle rental. The creator explicitly agreed to **try and document** this framework and to prioritize the missing recurring economy; this is not acceptance of numeric tuning, unlock dates, complete skill trees, or campaign scope.

## Verified repository evidence (code/document review, not fresh Play Mode)

Main branch at review: `cd3cdbecc21979d2341e178a4b8814e015afd208` (2026-11-11). Code includes `PickupDay`, `HouseholdFinances`, `NgojolMechanic`, `FishingRod`, `RiskArcade`, `NeighborBuyout`, Yarn dialogue, and checkpoint v7. [Neighborhood and Phone Prototype](../roadmap/neighborhood-phone.md) reports prior scripted Unity Play Mode checks. No new Unity changes or on-device tests were performed in this documentation update.

## Status and decisions

- **Bare mechanical slice: achieved as a prototype**, not a balanced or production-ready household economy.
- **Next milestone proposed:** durable financial transactions, recurring wages/expenses, income trade-offs, visible consequences, time-reclaimed recap, then the first cross-system progression proof.
- **Progression philosophy for trial:** meaningful new actions, reliable arrangements and family autonomy over generic stat ladders. Existing [Vision](../gdd/00-vision-and-pillars.md), [Family Control ADR](../decisions/ADR-0003-player-control.md), and [Risk Economy ADR](../decisions/ADR-0006-risk-economy-social-exploitation.md) remain authoritative.
- Reconciling historical docs: the original vision's absolute "No gambling" line predates the creator's later accepted optional fictional judol design. It should now express **no real-money/monetized gambling** and the risk of fictional simulated betting. The original MVP risk-system deferral is now an **as-built exception**: prototypes exist, complex consequence systems remain deferred.
- Known docs debt: two historical records both use ADR-0006; no silent renumbering in this design change. Resolve in a separate maintenance pass if needed.

## Files / follow-up

- New [Connected Progression GDD](../gdd/16-connected-progression.md), [M01 economy milestone](../roadmap/milestone-01-economy-and-consequences.md), and [30-day sketch](../roadmap/first-30-days.md).
- Reconcile relevant GDD navigation, MVP stage, history and roadmap status.
- Before implementing new unlock trees, prove a two-day finance → NPC → tomorrow consequence chain with recovery and save/reload tests.
