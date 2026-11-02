---
title: "12 — Open Questions"
status: open
type: decision-queue
updated: 2026-11-02
---

# Open Questions — Creator Decisions Needed

Avoid promoting proposals without explicit user agreement. Prioritize high-impact questions:

| ID | Question | Why it changes design | Current working hypothesis |
|---|---|---|---|
| Q-002 | Isometric, third-person, or switchable viewpoint? | Mechanics, animation scope, UI | Isometric-first; unapproved |
| Q-003 | Fully fictional town vs identifiable Cikarang-inspired district? | Map, cultural tone, research | Fictional industrial suburb |
| Q-004 | Simulation depth vs arcade comedy? | Complexity, pacing, audience | Approachable systems + consequences |
| Q-005 | How realistic are marriage, aging and education? | Time scale, narrative scope | Small starter family first |
| Q-006 | Can family members permanently leave, fail or become estranged? | Emotional tone, ratings, fairness | Prefer nuanced/recoverable consequences |
| Q-007 | Fishing as main interactive minigame or one of many leisure choices? | Core daily play | Main signature, not sole hook |
| Q-008 | Main development platform and engine? | Input/UI/performance/toolchain | Unity plausible, not approved |
| Q-009 | What is the intended audience and language? | UX, humor, store positioning | Indonesian-first flavor, undecided |
| Q-010 | Calendar speed, session target and game economy? | Balance, pacing | 12–15 min/day candidate |
| Q-011 | Exact bathroom doorway position in 3-petak denah? | Asset fidelity, bathroom access | Temporary opening at X +1.05 / Z 7 |

## Resolved decisions
- **Starter house scale/denah — Resolved 2026-11-02:** 1.65m Bapak, 3.5×9m house and door alignment accepted ([ADR-0005](../decisions/ADR-0005-starter-house-layout.md)). Bathroom access (Q-011) remains a layout assumption.
- **Q-001 — Resolved 2026-11-02:** Direct Bapak control plus indirect household coordination. See [ADR-0003](../decisions/ADR-0003-player-control.md) and [control model](13-household-control.md). Detailed planner interaction and AI decision rules remain proposed.

## Proposed next creator workshop
Settle Q-002 (camera) and Q-004 (arcade vs systemic simulation). Then script 2 playable days testing negotiated household coordination and whether reclaiming time is fun.

## Process
When resolved: record user decision in an ADR, update affected GDD pages, mark this table resolved with a link or remove row and retain history in the dated journal. Do not delete old discussions.
