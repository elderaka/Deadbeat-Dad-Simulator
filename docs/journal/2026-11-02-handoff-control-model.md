---
title: "2026-11-02 — Hybrid Control Handoff"
type: handoff
status: historical
date: 2026-11-02
---

# Handoff — Canonized Household Control

## Context
Original documentation bootstrap on branch `docs/canonize-gdd-v0.1`; PR #1 is draft. This session selected the hybrid input/control architecture.

## Accepted ADR
[ADR-0003](../decisions/ADR-0003-player-control.md) now establishes: **direct Bapak only; other NPCs autonomous; household coordination occurs via requests, agreements and planner, never direct possession/forced execution**.

## Detailed design
[GDD Section 13](../gdd/13-household-control.md) separates the canonical architecture from proposed request verbs, planner UI, task states, response constraints, event examples and MVP test.

## Docs touched
ADR-0003; decision index; GDD index, loops, logistics, family, MVP and open questions; vault index; dated journal and handoff. Verify actual committed diff and PR after write.

## Scope exclusions
No production code, tests, UI mockup, engine choice or camera selection. No multiplayer in MVP. Do not infer consent to exact state names or scoring formulas.

## Recommended next design task
Prototype Bapak's first school run and a choice between do-it-yourself, asking an unavailable Ibu, and arranging school transport. This is a design experiment; ask creator for camera and simulation-depth preferences before coding major systems.

## Verification
Documentation-only edits. Review PR diff for proper paths and approved status; no gameplay tests claimed.

## Subsequent update (2026-11-02)
The [starter house asset work log](2026-11-02-house-blockout.md) is the latest dated journal. The earlier hybrid control handoff remains relevant but is no longer the newest project activity.
