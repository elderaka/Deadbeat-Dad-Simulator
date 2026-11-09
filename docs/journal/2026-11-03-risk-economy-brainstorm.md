---
title: "2026-11-03 — Pinjol, Judol and Neighbor Opportunism"
type: journal
status: current
date: 2026-11-03
tags: [brainstorm, economy, consequences, neighbors]
---

# Decision Journal — Risky Money and Social Exploitation

## Creator's explicit direction
The creator wants genuine freedom to make harmful financial choices rather than mandatory protection. Proposed systems:
- **Pinjol** with high costs / high interest.
- **Judol** that loses money in the long run, suggesting **at most 0.8 RTP**, while allowing an early-win "honeytrap" narrative.
- Severe outcomes may include loss of home and, ultimately, divorce.
- The twist: divorce/household collapse is a route to the game's absurd **full-fishing sandbox mode** ("Bapak Merdeka").
- Additional stealth system: frequent judol and pinjol increase a **silent hidden financial-risk signal** that attracts an **opportunistic neighbor** willing to buy Bapak's belongings at a severe lowball because they believe he needs cash.
- The creator explicitly asked that this be added to the design document before the session is lost.

## Accepted direction
[ADR-0006](../decisions/ADR-0006-risk-economy-social-exploitation.md) captures player freedom, negative long-term consequences, hidden social-financial pressure, exploitative NPC purchase offers, potential home loss/divorce and the ironic fishing route.

## Proposed refinements, not agreed numerical implementations
- Separate Financial Desperation from Perceived Vulnerability / awareness **per NPC** so private financial losses do not create omniscient neighbors.
- RTP `≤0.8` is a suggested tuning value, not a guaranteed payout or finalized balancing constant.
- Initial wins may be scripted or sampled, but must not enable reusable profit farming.
- Neighbors can act opportunistically, helpfully or neutrally depending on disposition and information.
- Counterplay: accept/refuse/bargain/find buyer/other income; visible debt/asset ledger, invisible internal signals.
- Gradual dialogue and independent spouse decisions, not a one-frame divorce threshold.
- Detailed sandbox mechanics, custody, finances, legal/ratings and exact percentages remain undecided.

## Documentation touched
New GDD 15 and ADR-0006, linked from economy, family, progression, events, leisure and indexes. Open questions updated. No gameplay code written, UI tests or math simulations run.

## Next steps
Prove core household/fishing loop first. Later prototype three NPC behaviors across controlled secrecy, distress and asset scenarios. Test fairness and recovery options before implementing serious household consequences.
