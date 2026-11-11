---
title: "Milestone 01 — Household Economy and Consequence Loop"
type: roadmap
status: proposed
updated: 2026-11-11
---

# Milestone 01 — Household Economy and Consequence Loop

**Current state (2026-11-11):** the creator reports a **bare playable vertical slice**. The main branch contains physical school/factory transport, an ojol fare, fishing/fish sales, a phone, bills and pinjol, simulated judol, NPC routines/dialogues, the TV lowball offer, and checkpoint persistence. Scripted Unity Play Mode checks are documented in [Neighborhood and Phone Prototype](neighborhood-phone.md); this note is not a fresh hands-on validation.

**Not yet complete:** a recurring family budget, Ibu's income and ownership/agreements, rent and essentials, consequences beyond simple overdue reminders and the one lowball event, or long-horizon economy balancing. Therefore **bare slice achieved does not equal economy slice achieved**.

## Goal

Deliver **one complete readable chain**: a player decision changes household cash/time → a bill or obligation responds → a resident observes or is told a relevant fact → a later opportunity, schedule or relationship changes → the player sees why in a recap. Make at least one responsible recovery route viable.

## Work order and scope

| Priority | Work package | Minimum vertical proof |
|---|---|---|
| P0 | **Durable financial transactions** | Save/reload cannot selectively erase a bet loss while retaining a win. Ledger records sources, amounts, day, reason and confirmed changes; test interruption/restart and migration. |
| P0 | **Recurring household budget** | Define income timing (including Ibu's actual shift/wages), rent, food/utilities and reserve rules. Distinguish Bapak's earnings from Ibu's earned income and any consensually pooled budget, even if prototype UI uses one shared wallet. |
| P1 | **Meaningful income choices** | Ojol payout after route/fuel/time; fishing access fees, catches and warung sales; demonstrate that neither dominates all decisions. Nambak remains later. |
| P1 | **Readable consequences** | Unpaid bills/loan affect next-day choices, feasible contact or disclosure, resident knowledge and one negotiation/lowball/supportive event; no omniscient NPCs. |
| P1 | **Time and household recap** | Show daily income, spend, debt change, obligation coverage, free time gained, and who covered each family task; compare to previous day. |
| P1 | **Scenario/difficulty integrity** | Starting cash and inherited debt remain playable; vary recurring economic circumstances only with a verified legitimate recovery route. No hard mode where judol is the only apparent path. |
| P2 | **Cross-system progression proof** | After budget stability, one paid-pond vs public-river choice, one vehicle repair/access improvement, one autonomous family milestone, one useful home furnishing. |

## Acceptance scenarios (demonstrate, don't just expose controls)

1. **Responsible recovery:** normal preset → earn via an ojol ride or caught fish → meet a realistic bill/loan obligation or arrange a viable partial/negotiated solution → next-day finances, obligations and NPC knowledge reflect actions.
2. **Poor decision with consequence:** gamble or borrow → reach arrears → see understandable collection/reminder pressure → disclose or reveal facts to a specific neighbor → choose accept/refuse a lowball asset offer; no automatic divorce or forced asset loss.
3. **Time reclaimed:** arrange an alternative school pickup → keep task covered → show actual saved time, fee and subsequent leisure or work option.
4. **Persistence:** quit/relaunch at an unsafe moment after a transaction and after a completed day; no duplicated cash, forgotten due debt, reset NPC knowledge or replayed settlement.
5. **Fairness:** on every difficulty, at least one non-gambling path can cover essentials through work, negotiation or support within a realistic period; test across multiple days, not just the first.
6. **Player comprehension:** a tester can explain where cash went, why an NPC reacted, and what one upgrade or agreement changed.

## Technical architecture questions, not pre-approved solutions

- Current code has a single `PlayerProgression.Cash` wallet and end-of-day checkpoints. Choose atomic event persistence, frequent safe checkpoints or a journal/replay architecture; protect against duplicated or reversible transactions.
- Define financial-event IDs, day/time, actor, account/source, transfer amount, bill/loan references and durable causal flags. One authoritative ledger should feed phone, recap, debt and social reactions rather than multiple uncoordinated counters.
- Distinguish wages paid into Ibu's account vs money she chooses to contribute to household essentials; avoid treating her as a passive revenue generator.
- Housing collapse and separation remain gradual *future* consequence branches, not mandatory M01 deliverables. The creator's three-day post-housing-loss rescue concept is a candidate that needs agency, support and pacing rules.

## Explicit deferrals

Full 30-day campaign, entire fishing map, restricted fishing enforcement, nambak, rental fleet/tycoon loops, multi-house real estate, child aging/ending simulation, complex addiction model, Android-device certification and full graphics polish.

**Next:** execute M01 first, then [prototype progression choices](../gdd/16-connected-progression.md) using the [30-day pacing sketch](first-30-days.md). No estimated delivery date or sprint commitment.
