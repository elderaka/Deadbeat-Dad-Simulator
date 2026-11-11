---
title: "10 — MVP, Vertical Slice and Playtests"
status: proposed
type: design
tags: [gdd]
updated: 2026-11-11
---

# 10 — MVP, Vertical Slice and Playtests

> **Status: PROPOSED** — Canonical = approved direction; Proposed = hypothesis to validate; Open = undecided. Numeric examples are not final balancing.

## Purpose
Validate whether **eliminating chores while maintaining a lively household is fun**, independent of fishing loot novelty.

## Proposed 7-day vertical slice
- One playable Bapak, one spouse, one child.
- Compact map: house, school, factory, warung, pond, short road links.
- Day/night schedule; cash, obligations, time and basic household stability.
- Simplified morning drop-off and evening pickup.
- Fishing minigame, fish selling and a few variants.
- A responsibility board with 3–5 durable convenience solutions.
- Reflect the accepted control split: only Bapak moves directly; other characters are approached through requests/agreements and a planning surface. A minimal test needs one request that cannot be fulfilled because of a visible schedule conflict and one viable alternative.
- 6–10 contextual events and a weekly household report.
- One weekly goal: visibly improve a morning routine.

## The critical 2-day test
Day 1: complete family transport, pay a bill, get groceries and find fishing time. Day 2: subscribe to transportation or organize a neighbor arrangement and see new leisure time but different costs.

Ask: (1) Did the player understand the trade-off? (2) Did they feel smart about freeing time? (3) Did they care about family outcomes? (4) Were they willing to play another day without a rare fish reward?

## Constraints
No advanced generations, procedural town, online lobbies, big city, full driving physics, complicated AI, or paid online backend in the first test. Art polish is not needed for this design validation.

## As-built note — 2026-11-11

The creator reports a bare playable Kampung slice. The current code includes real vehicle/passenger transport, family calls, live fishing and fish sales, prototype ojol fares, loan quotes, simulated judol and a single NPC distress-sale event. Earlier scoped-out risk mechanics were **prototyped opportunistically**. Their mere presence does not establish a complete consequence model, balanced economy, or validated seven-day household loop. Refer to the documented [prototype checkpoints](../roadmap/neighborhood-phone.md) for previous scripted Play Mode evidence; no new hands-on test is claimed here.

**Next milestone:** [Recurring household economy, transaction durability and one financial → NPC → tomorrow consequence chain](../roadmap/milestone-01-economy-and-consequences.md), before adding large upgrade trees. A candidate [first-30-day progression experiment](../roadmap/first-30-days.md) follows after shorter tests succeed. Full eviction/divorce outcomes, nambak, vehicle rental tycoon systems and generational endings remain deferred.

## Pass/fail indicators (initial hypotheses)
- Players articulate the objective as "improve life to reclaim time."
- They notice at least one causal consequence of past decisions.
- They experiment with one alternative household solution.
- Failure cases are attributable to understandable decisions.

## Next production artifacts
After approval, write testable mechanics specs, resource data model, game-day state machine and first playable scenario. Do not silently convert this candidate scope into a committed sprint.
