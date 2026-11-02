---
title: "04 — Family Autonomy and Relationships"
status: proposed
type: design
tags: [gdd]
updated: 2026-11-02
---

# 04 — Family Autonomy and Relationships

> **Status: PROPOSED** — Canonical = approved direction; Proposed = hypothesis to validate; Open = undecided. Numeric examples are not final balancing.

## Family is the progression, not a meter
Proposed model: directly control Bapak; other household members are autonomous NPCs with jobs, school, ambitions, preferences and relationships. **Single-character control has not been explicitly locked by the creator.** See [ADR-0003](../decisions/ADR-0003-player-control.md).

### NPC state
- Stable traits: ambition, sociability, interests, independence
- Dynamic needs: fatigue, wellbeing, security, connection
- Schedules: work, school, travel, hobbies and sleep
- Goals: promotions, education, hobbies, friendships
- Selective memory flags: repeated encouragement, important commitments, neglected promises

Start with rule/utility/state-based AI, not an LLM dependency.

## Agency and player influence
Bapak can support, negotiate, spend resources, join activities, and plan logistics. NPCs make reasonable decisions on their own. A spouse's salary is not free passive income: they have work constraints and independent ambitions. A child's grades aren't the sole measure of their wellbeing.

## Family growth
Additional children may increase complexity, costs and opportunities for stories, but are entirely optional. Keep family planning tasteful/non-explicit. Aging, graduation, careers and grandchildren are **long-term proposals**, not MVP features.

## Example consequence chain
Child asks Bapak to attend a school performance overlapping a fishing tournament. Attending may strengthen their bond; choosing the competition may disappoint them; arranging a substitute may work depending on work schedules. **No single choice should deterministically decide a child's future.** Long-term relationships depend on repeated patterns.

## MVP candidate
One spouse, one child; simple schedules, few traits, two or three meaningful family events and transparent causes for relationship changes. Example names Adit/Pak Ujang are placeholders, not lore canon.

## Risks
- Emotional guilt becoming the only challenge
- NPC AI causing inexplicable outcomes
- Household members reduced to vending machines or income engines
- Excessive simulation before proven core loop
