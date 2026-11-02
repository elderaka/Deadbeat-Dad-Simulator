---
title: "02 — Core Gameplay Loops"
status: proposed
type: design
tags: [gdd]
updated: 2026-11-02
---

# 02 — Core Gameplay Loops

> **Status: PROPOSED** — Canonical = approved direction; Proposed = hypothesis to validate; Open = undecided. Numeric examples are not final balancing.

## Daily loop (candidate: 12–15 real minutes per in-game day)
| Segment | Gameplay | Reward / information |
|---|---|---|
| Morning | Review obligations, drop-offs, schedules and budget; plan | Clear priorities and commitments |
| Day | Fishing, side hustles, errands, social interactions and discoveries | Skill, income, relationships, opportunities |
| Evening | Pickups if required; family interaction; reconcile income and events | Consequences and personal stories |
| Night | Choose upgrades, investments, plans and next-day priorities | New capabilities and strategic trade-offs |

Morning commutes are central to the original pitch: drive the wife to her factory job and child to school; pick them up later. Later automation may replace these tasks. **Do not silently skip their initial gameplay role.**

## Weekly loop
Review household finances, family aspirations, school/work milestones, recurring bills, neighbor relationships and which inconveniences to eliminate next.

## Long-term loop
Housing and transport improvement → more reliable routines → household resilience → family growth and independent careers → income-producing investments → leisure with purpose and optional retirement.

## Core action grammar
**Inspect obligation → choose Bapak action or propose coordination → secure agreement where required → spend time/resources → observe outcome → learn/update plan.** Combine with active leisure minigames. The direct-Bapak + indirect-household control split is canonized in [ADR-0003](../decisions/ADR-0003-player-control.md); the exact planner, requests and task lifecycle are [proposed](13-household-control.md).

## Tension
Each day has finite time; competing events may require trade-offs. Not all important family attention can be bought or delegated. Disruption should be occasional and readable, not relentless or arbitrary.

## Tuning hypotheses
- Proposed pacing: a few in-game days per play session, with fast-forward for repetitive travel/chores.
- A day must produce at least one visible consequence of a prior decision.
- Long-term milestones must be achievable without maximizing family size.

**Unresolved:** Exact clock rate, fatigue/energy systems, fast-forward boundaries and what is mandatory in the first seven days. See [Open Questions](12-open-questions.md).
