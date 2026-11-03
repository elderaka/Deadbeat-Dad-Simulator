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
Unity standalone on Steam and potentially Android; Roblox considered for a separate social-first variant. No definitive engine/platform/shipping commitment has been made for this repository. Team assets and programming are self-produced for planning purposes; publication and hosting costs are external.

## Later multiplayer fantasy
Small lobby of Bapak players: fishing together, chatting, neighborhood visits, local contests and social chaos. The main household loop must work offline first. Persist personal household progress separately from ephemeral lobby interactions.

## Architecture questions for later
Co-op shared household or separate households? Player host vs server-authoritative? Cross-platform? Text chat moderation? Voice? Persistence, anti-cheat, disconnect behavior, accounts and age rating? Do not lock networking until the solo core loop is validated.

## Roadmap gate
Multiplayer is only reconsidered after a fun and technically stable single-player vertical slice. A compelling social idea is not evidence that it should be implemented now.

**Costs quoted in chat are historical budgeting examples, not current prices; reverify before shipping.**
