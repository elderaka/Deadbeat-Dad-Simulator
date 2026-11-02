---
title: "2026-11-02 — Hybrid Household Control Decision"
type: journal
status: historical
date: 2026-11-02
tags: [brainstorm, decision, control]
---

# Brainstorm and Decision: Hybrid Household Control

## Creator input
After choosing *The Art of Doing Absolutely Nothing* and deferring multiplayer, the creator selected: **"Let's explore Bapak direct control plus indirect household commands for Deadbeat Dad Simulator, then canonize the decision."**

## Accepted design outcome
Recorded in [ADR-0003](../decisions/ADR-0003-player-control.md):
- Only Bapak is directly playable.
- Household members act autonomously, not like selectable Sims.
- Player indirectly coordinates the household through planning and agreements.
- NPCs can decline unrealistic or conflicting requests rather than instantly obeying.

## Explore / proposals, not approved specifics
- Three-layer interfaces: direct in-world actions, conversations, household planner.
- Responsibility state transitions (Requested vs Confirmed).
- One-off asks vs recurring arrangements.
- Availability and capacity before relationship modifiers.
- Transparent reasons for refusal; dependable arrangements and reasonable backups.
- Phone/calendar UI; actual interaction layout not decided.

## Strong example scenario
Bapak needs to pick up the child but wants to fish. Ibu has a conflicting factory shift; asks can fail for a legible reason. Neighbor or school transportation is feasible for a cost. The resulting agreement removes repeat school trips while respecting the household.

## Tests / limitations
Design documentation only; no UI, NPC behavior, code, or playtest implemented. A proposed responsibility lifecycle is not a locked code contract.

## Next unanswered decisions
Q-002 camera; Q-004 arcade versus systemic realism; household planner UX; exact MVP, AI depth and task economy. See [Open Questions](../gdd/12-open-questions.md).
