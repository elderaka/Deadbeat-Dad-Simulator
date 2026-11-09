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
| Q-003 | Fully fictional town vs identifiable Cikarang-inspired district? | Map, cultural tone, research | Fictional industrial suburb |
| Q-005 | How realistic are marriage, aging and education? | Time scale, narrative scope | Small starter family first |
| Q-006 | How should separation/divorce, child responsibilities and recovery work? | Emotional tone, agency, ratings | Divorce and home loss now permitted; detailed outcomes open |
| Q-007 | Fishing as main interactive minigame or one of many leisure choices? | Core daily play | Main signature, not sole hook |
| Q-009 | Exact audience and localization scope? | UX, humor, store positioning | Indonesian market; ID first and EN intended, exact scope open |
| Q-010 | Calendar speed, session target and game economy? | Balance, pacing | 12–15 min/day candidate |
| Q-011 | Exact bathroom doorway position in 3-petak denah? | Asset fidelity, bathroom access | Temporary opening at X +1.05 / Z 7 |
| Q-012 | Approve the 5-unit compound and 19.5×17m site as the starter neighborhood? | Level scope and navigation | Proposed 5-door shared court; needs playtest |
| Q-013 | Judol RTP/honeytrap implementation and Pinjol loan terms? | Risk balance, fairness, ratings | Creator suggested ≤0.8 RTP; no locked numeric rates |
| Q-014 | How do NPC awareness, lowball prices, encounter chances and cooldowns work? | Believability, counterplay, AI scope | Distress plus NPC-specific knowledge; values hidden |
| Q-015 | What exactly happens after divorce to home, children and fishing-only sandbox, and is alternate access available? | Progression, consequence tone | Divorce-to-fishing route accepted; transition open |

## Resolved decisions
- **2026-11-05 — Q-002, Q-004, Q-008:** First person; approachable simulation with comedic consequences; Unity with Windows/Android compatibility. Indonesian first and English intended. See [ADR-0006](../decisions/ADR-0006-unity-prototype-foundation.md).
- **Risky shortcuts and social vulnerability — Accepted 2026-11-03:** Optional pinjol/judol, a silent financial-pressure system, opportunistic lowball offers, possible loss of house/divorce, and the ironic full-fishing outcome. See [ADR-0006](../decisions/ADR-0006-risk-economy-social-exploitation.md) and [GDD §15](15-risk-economy-and-neighbor-exploitation.md). Exact numbers and consequences (Q-006, Q-013–Q-015) remain open.
- **Starter house scale/denah — Resolved 2026-11-02:** 1.65m Bapak, 3.5×9m house and door alignment accepted ([ADR-0005](../decisions/ADR-0005-starter-house-layout.md)). Bathroom access (Q-011) remains a layout assumption.
- **Q-001 — Resolved 2026-11-02:** Direct Bapak control plus indirect household coordination. See [ADR-0003](../decisions/ADR-0003-player-control.md) and [control model](13-household-control.md). Detailed planner interaction and AI decision rules remain proposed.

## Proposed next creator workshop
After the walkable blockout, refine transport and fishing, then script 2 playable days testing negotiated household coordination and whether reclaiming time is fun.

## Process
When resolved: record user decision in an ADR, update affected GDD pages, mark this table resolved with a link or remove row and retain history in the dated journal. Do not delete old discussions.
