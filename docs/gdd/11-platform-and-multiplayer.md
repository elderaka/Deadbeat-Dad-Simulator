---
title: "11 — Platform and Multiplayer Roadmap"
status: proposed
type: design
tags: [gdd]
updated: 2026-11-02
---

# 11 — Platform and Multiplayer Roadmap

> **Status: PROPOSED** — Canonical = approved direction; Proposed = hypothesis to validate; Open = undecided. Numeric examples are not final balancing.

## Current decision
**Multiplayer is explicitly future roadmap, not MVP.** See [ADR-0002](../decisions/ADR-0002-multiplayer-roadmap.md).

## Candidate platforms
**Accepted 2026-11-05:** Unity with Windows and Android compatibility; Indonesian first, English also intended ([ADR-0006](../decisions/ADR-0006-unity-prototype-foundation.md)). Store publication, audience details and release dates remain open. Roblox was a historical candidate, not the current implementation target. Publication and hosting costs are external.

## Later multiplayer fantasy
Small lobby of Bapak players: fishing together, chatting, neighborhood visits, local contests and social chaos. The main household loop must work offline first. Persist personal household progress separately from ephemeral lobby interactions.

## Architecture questions for later
Co-op shared household or separate households? Player host vs server-authoritative? Cross-platform? Text chat moderation? Voice? Persistence, anti-cheat, disconnect behavior, accounts and age rating? Do not lock networking until the solo core loop is validated.

## Roadmap gate
Multiplayer is only reconsidered after a fun and technically stable single-player vertical slice. A compelling social idea is not evidence that it should be implemented now.

**Costs quoted in chat are historical budgeting examples, not current prices; reverify before shipping.**
