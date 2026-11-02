---
title: "03 — Time, Responsibilities and Automation"
status: proposed
type: design
tags: [gdd]
updated: 2026-11-02
---

# 03 — Time, Responsibilities and Automation

> **Status: PROPOSED** — Canonical = approved direction; Proposed = hypothesis to validate; Open = undecided. Numeric examples are not final balancing.

## Time is the strategic resource
Money is a constraint; **discretionary time is the primary reward**. Upgrades should change how the player's day feels. Show where time went, how much was recovered, and which responsibilities remain.

Distinguish:
- **Obligations:** routine errands, daily pickup, bills and repetitive logistics.
- **Meaningful commitments:** relationships, school meetings, promises and emergencies; some require Bapak's presence.
- **Opportunities:** fishing, family outings, side ventures, hobbies, exploration and neighborhood events.

## Responsibility objects (candidate data model)
Each responsibility has: owner, recurrence, due window, time cost, cash cost, reliability requirement, people affected, solution options and escalation rules. Example: **School pickup** can be completed manually, delegated to a trusted transport provider for a fee, or missed with relationship/logistics consequences.

## Solutions must be durable but imperfect
A school bus subscription can remove repetitive manual pickups and charge a recurring fee. A household appliance reduces ongoing work. Automated bill payment still requires sufficient funds. Occasionally, a system can change price or become unavailable; the player is managing, not repeatedly clicking.

## Responsibility coverage
Track whether core obligations were fulfilled, independently of how much Bapak personally did. Optional **Indeks Rebahan** can incorporate:
- Normalized discretionary time
- Household stability
- Responsibility coverage

A multiplicative or gated score discourages maximizing free time through neglect; **the exact formula is not approved**.

## Anti-friction rules
Do not require manually repeating a solved routine forever. Avoid intrusive hunger/thirst/toilet micromanagement unless an exceptional minigame needs it. Automating chores should unlock enjoyable choices rather than empty gameplay.

## Testable hypothesis
After purchasing an automation upgrade, players should understand *which* obligation disappeared, *what* it costs, and *what new choice* that extra time enables.
