---
title: "06 — Economy, Purchases and Investments"
status: proposed
type: design
tags: [gdd]
updated: 2026-11-11
---

# 06 — Economy, Purchases and Investments

> **Status: PROPOSED** — Canonical = approved direction; Proposed = hypothesis to validate; Open = undecided. Numeric examples are not final balancing.

## Household economy
Recurring expenses: food, utilities, transport, education, housing and optional services. Income candidates: spouse wages, sold fish, side hustles, aquaculture and later businesses. Model income timing, liquidity, commitments and emergency buffer.

## Purchase categories
| Category | Example | Gameplay consequence |
|---|---|---|
| Convenience | School transport, appliances, bill autopay | Less recurring manual work, sometimes recurring fees |
| Capacity | Motorcycle → family car, additional room | Supports larger household; adds upkeep |
| Resilience | Savings, home repairs | Fewer emergency spirals |
| Opportunity | Fishing gear, pond access, fish farm | New activities/income sources |
| Expression | Furniture, family photo wall | Personalizes evolving home |

## Design constraint: every serious purchase has a verb
A bigger home should enable an activity or capacity, not merely make the asset counter larger. A better vehicle changes scheduling capacity, speed or reliability, not just its sprite.

## Risky financial shortcuts (accepted direction)
Players may voluntarily enter fictional high-cost **pinjol** loans or simulated **judol** gambling, both deliberately unattractive long-term strategies. Their liabilities, transaction history and potential asset losses feed into a hidden pressure/neighbor-awareness system and can trigger opportunistic lowball purchase offers. Serious consequences can include housing loss and divorce. Exact 80% RTP proposal, lending terms and encounter probabilities are **not final**; see [GDD Section 15](15-risk-economy-and-neighbor-exploitation.md) and accepted [ADR-0006](../decisions/ADR-0006-risk-economy-social-exploitation.md).

## Economic balancing principles
- Family size increases cost, logistics and narrative variety, never automatically profit.
- Wage income must reflect work time and constraints.
- Services save hours but consume money; investments may require initial work.
- Players should be able to recover from setbacks without grinding the same job for hours.

**Numbers previously discussed (rupiah amounts and time savings) are examples only, not canonical prices.** Build a parameter sheet later after playtesting.

## Risk signals

### Post-slice economy gap and proposed sequence — 2026-11-11

The repository now has a wallet, ojol fare payout, fish inventory/sale, bills, one active pinjol and several fictional risk games. **It does not yet have a validated recurring household economy.** In particular, Ibu's wages/shift-to-cash flow, groceries/rent/recurring essentials, account ownership and consent, partial/negotiated repayment, transaction-safe persistence and long-term balance remain open. Avoid describing prototype transactions as a complete household budget.

The next [milestone](../roadmap/milestone-01-economy-and-consequences.md) builds a durable financial event history and one readable household decision → budget → social response chain before expansion. [Connected Progression §16](16-connected-progression.md) treats vehicle maintenance, useful furnishings, fishing access and later nambak/rental as distinct choices that can free time **or** create commitments.
No meaningful trade-offs; upgrades that always pay off instantly; impossible early debt; fish farming so profitable it invalidates the rest of the game.
