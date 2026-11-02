---
title: "13 — Player Control and Household Coordination"
type: design
status: mixed
tags: [gdd, control, agency]
updated: 2026-11-02
---

# 13 — Player Control and Household Coordination

> **Status: MIXED.** The architecture in §1 is **canonical** ([ADR-0003](../decisions/ADR-0003-player-control.md)). Specific interfaces, state machines, prioritization and formulas in later sections are **proposed** until playtested or separately approved.

## 1. Canonical player/control contract

The player directly controls **Bapak only**. Ibu, children and neighbors are autonomous characters who have their own schedules, ambitions, relationships and capacities. The player influences the household *indirectly*, not by switching character or forcing an order.

Three complementary layers:
- **Body:** Bapak moves, drives, fishes, visits, purchases, repairs, talks and personally fulfills obligations.
- **Conversation/agreements:** Bapak asks, offers, coordinates and negotiates help. Other people can consent, counter-propose, or refuse for understandable reasons.
- **Planner:** Bapak views obligations, household schedules and agreements, and decides what to handle or arrange. A plan does not automatically make another person comply.

The wife is a partner, not a subordinate; she has an independent factory job and ambitions. Children cannot be treated as free labor or abstract resource multipliers. Delegation respects availability, reasonable capacity and commitments.

## 2. Proposed control vocabulary (not canonized)

| Verb | Actor | Effect | Caveat |
|---|---|---|---|
| **Do** | Bapak | Player directly performs a task | Costs his time / resources |
| **Ask** | NPC | One-off request with possible agreement | Availability and willingness matter |
| **Arrange** | NPC or provider | A repeated, negotiated arrangement | May carry recurring money/social costs |
| **Reschedule** | Affected people | Change a deadline or window when allowed | Can be refused; depends on context |
| **Prioritize** | Bapak | Rank plans and opportunities | Does not override NPC autonomy |
| **Skip** | Bapak | Accept foreseeable consequences | Never a free route to a high efficiency score |

**Avoid literal unit-selection, "assign worker" gameplay, and mandatory repeated requests for a previously accepted recurring routine.**

## 3. Proposed interaction surfaces

- **In-world:** move Bapak; talk to characters; interact with objects/vehicles.
- **Phone or wall calendar (planner):** today's timeline, who needs transport, confirmed arrangements, unresolved requests, costs and backup options.
- **Contextual resolution:** select an obligation → *Do it myself*, *Ask someone*, *Arrange a service*, *Reschedule*, or *Skip*, with availability hints and likely costs.

Mock screens and camera controls are open. The planner should be accessible without pausing all character agency unless playtesting warrants it.

## 4. Proposed responsibility lifecycle

`Upcoming → Proposed → Requested → Confirmed → Completed` with alternate branches `Declined`, `At risk`, `Failed`, `Rescheduled`. A Bapak-only direct task can go straight from Upcoming to In progress to Completed.

**Crucial distinctions:**
- **Requested ≠ Confirmed.** Asking Ibu to handle pickup does not guarantee pickup.
- **Confirmed ≠ Invulnerable.** Rare disruptions can happen, but should provide warning and a recoverable backup.
- **Delegated ≠ Free.** Costs may be money, scheduling burden, reciprocity or trust; not necessarily a relationship-point tax.
- **Unassigned ≠ Automatically Bapak's problem.** Family members can independently handle suitable responsibilities; critical dependent needs should have clear accountable coverage.

Candidate task fields: `id, timeWindow, recurrence, affectedPeople, requiredCapacity, candidateSolutions, cost, responsibleActor, agreementState, fallback, history`. **Data schema is not a commitment to software architecture.**

## 5. Constraint-driven NPC responses (proposed)

Responses should be grounded primarily in:
1. **Hard constraints:** work/school shifts, health, distance, existing obligations, transport capacity.
2. **Soft constraints:** preferences, fatigue, relationship quality, reciprocity, goals.
3. **Agreement history:** previously promised tasks, repeated requests, reliability.

Present clear reasons when help is unavailable. A high relationship value should not override an impossible work shift. Avoid arbitrary refusal rolls and opaque "NPC betrayed you" gameplay. Accepted arrangements should be dependable enough to reward planning.

## 6. The two-day test case (proposed)

**Day 1:** Bapak physically takes the child to school and Ibu to work, then has limited fishing time. Planner shows who needs collection and what remains unresolved.

**Day 2:** Bapak wants more pond time. He can (a) continue driving himself; (b) ask Ibu, who has a conflicting shift, triggering a clearly explained counterproposal/refusal; or (c) negotiate a recurring school transport service with a fee. Accepted arrangement appears as Confirmed and gets fulfilled without repeating manual interactions.

Evening report: time reclaimed, money spent, responsibility actually covered and family reactions. The goal isn't "maximize delegation" but **free time without abandoning commitments**.

## 7. Interaction design guardrails

- Do not turn the family into Sims puppets or controllable worker units.
- Do not create a corporate kanban board with 40 microtasks.
- Do not constantly guilt players for choosing fishing.
- Do not make an accepted agreement require a new conversation every day.
- Important refusals should be understandable, not a random punishment.
- Keep spouse and children interesting even when no task is being requested.
- Differentiate tasks Bapak can outsource from meaningful family presence he must choose to provide.

## 8. MVP candidate and validation

**Smallest test:** three people, one school transport responsibility, one independent work schedule, one possible negotiation, one paid recurring solution, and one resulting visible free-time gain.

Ask testers:
1. Can you distinguish **request** from **confirmed agreement**?
2. Do family members feel autonomous rather than controllable?
3. Can you explain why a request was declined?
4. Does a confirmed routine save time without unnecessary manual repeats?
5. Does the planner feel useful without becoming the entire game?

## 9. Still open

- Q-002 camera (isometric / third person / switching) and exact platform controls.
- Planner format (phone vs in-world board vs both).
- How many recurring agreements may be active and their failure rate.
- NPC decision algorithm and how much of it to expose.
- Whether there are household-wide goals proposed jointly by Ibu and Bapak.
- How children gain age-appropriate agency as they grow.

Related: [Core Loops](02-core-loops.md) · [Time & Logistics](03-time-and-logistics.md) · [Family Autonomy](04-family-autonomy.md) · [MVP](10-mvp-and-tests.md).
