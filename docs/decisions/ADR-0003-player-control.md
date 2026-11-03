---
title: "ADR-0003 — Direct Bapak Control and Indirect Household Coordination"
type: decision
status: accepted
date: 2026-11-02
---

# ADR-0003 — Direct Bapak Control and Indirect Household Coordination

## Context

The creator weighed (a) controlling the entire family, (b) controlling Bapak only with autonomous NPCs, and (c) a hybrid: directly control Bapak while coordinating the household indirectly. They explicitly selected **"Bapak direct control plus indirect household commands"** for exploration and canonization.

## Decision — Accepted

**Bapak is the only character the player directly controls.** Other family members and neighbors have autonomous schedules, goals, needs and agency. The player coordinates responsibilities indirectly through conversations, requests, plans and mutually workable agreements.

The control architecture has three conceptual layers:

1. **Direct action:** player moves Bapak and interacts with the world (travel, fish, work, shop, visit and personally fulfill responsibilities).
2. **Social coordination:** player asks other people for help, discusses arrangements and negotiates shared responsibilities. NPCs can accept, counter-propose, or decline for grounded reasons.
3. **Household planning:** player inspects obligations and schedules, sets priorities and records established arrangements; the planner is *not* a remote-control screen for family members.

The household is a network of people with their own lives, **not an employee-management simulator**. Children's welfare and age-appropriate autonomy must be respected.

## Why this decision

- Maintains identification with the central character and physical/comedic gameplay.
- Allows the strategic household-engineering fantasy without playing every family member.
- Creates real trade-offs through time, capacity, money, independent schedules and relationships.
- Makes agreements and reliable routines a source of progression ("earn freedom").

## Boundaries

- A player's request is not automatically fulfilled; the distinction between requested and accepted obligations must be clear.
- Do not introduce direct NPC possession/switching or unrestricted task-forcing without explicitly revisiting this ADR.
- Avoid obscure arbitrary refusal and endless NPC micromanagement; choices should be legible and fair.
- One-off assistance, recurring agreements, specific planner UI, NPC AI algorithms and mechanical values remain **proposals**, not locked implementation details.
- Camera angle, engine, platform and control mapping are independent **open choices**.

## Consequences / follow-up

Implement a small paper/playable scenario where the player chooses between self-handling, asking Ibu (who may have a conflicting factory shift), and arranging neighbor/school transport. Validate that the player understands *request vs agreement*, time savings and consequences.

See [Household Control GDD](../gdd/13-household-control.md), [Family Autonomy](../gdd/04-family-autonomy.md), and the [decision journal](../journal/2026-11-02-household-control.md).
