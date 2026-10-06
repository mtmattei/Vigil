# Validation: Vigil (veterinary anesthesia record) vs. event rental turnaround

Date: 2026-10-06   Mode: Quick (stages 1, 2, 3, 8, 9, 10)   Sessions: 1

## How this was run (deviation from the skill)

The user asked for an autonomous end-to-end build ("decide autonomously; ask only when blocked"), so
Claude answered the grilling questions from domain knowledge and public research instead of the user
answering them. Consequence: **no claim below can exceed E1** except public market facts (incumbents
exist and sell), which are graded as E2-proxy (someone pays someone, not our customer paying us).
This is a sample/reference-app build, so the verdict decides which concept to build as a credible
industry app, not whether to start a company.

## Candidate A: Vigil, the veterinary anesthesia record

**Idea:** Surgical vet techs in a small-animal hospital record the pre-anesthetic check, weight-based
drug doses and vitals every 5 minutes on a tablet beside the patient, and the veterinarian signs the
record; it replaces the paper anesthesia sheet.

| Stage | Answer | Evidence | Gap |
|---|---|---|---|
| 1 Problem | Paper sheets clipped to the anesthesia machine: vitals charted by hand every 5 min, dose math done on a calculator from the weight, sheets scanned into the PMS afterwards. Errors: missed intervals, unit/concentration mistakes in mg/kg to mL conversion, illegible trend. Frequency: every anesthetic procedure, several per day per surgery room. | E0 (Claude domain knowledge) | No named tech has told us this. |
| 2 Customer | Licensed vet techs (RVT/LVT) running anesthesia in 2-6 doctor general practices doing daily spays, neuters and dentals; the medical director decides; the practice owner pays. | E0 | Cannot name five; payer differs from user. |
| 3 Evidence | At least five products sell this exact job: Vet Radar, VetMate (Covetrus), SurgiVitals, Vetnapp, VetMo. Practices pay for it. | E2-proxy (public market) | Proves demand exists, not that our version is wanted. |
| 8 Risks | Value 2/5 (incumbents prove the job), usability 4/5 (gloved hands, glanceable at arm's length, interruptions), feasibility 2/5, viability 4/5 (crowded market, PMS integration). | E0 | Usability is the risk this build can actually test. |
| 9 Pre-mortem | (1) Techs found tapping slower than paper during a crisis, (2) no PMS integration so double entry remained, (3) incumbents bundle it free with their PMS. | E0 | |

**Riskiest assumption:** a tech can record a full 5-minute vitals set on a tablet in under 15 seconds
with gloves on, faster than paper.
**Next test (if this were a product):** at least 4 of 5 vet techs at one practice record a complete
vitals set in under 15 s on a prototype during a mock procedure, and 3 of 5 prefer it to paper.
Stop if fewer than 2 of 5. Run by 2026-11-06.

## Candidate B: event rental turnaround

**Idea:** Warehouse crews at an event-rental company check in returned items after weekend events,
log damage and missing items with photos, route items to cleaning or repair, and release them for
the next order.

| Stage | Answer | Evidence | Gap |
|---|---|---|---|
| 1 Problem | Monday surge of returns; damage noticed late; items double-booked because they are still in repair. Weekly frequency, concentrated. | E0 | |
| 2 Customer | Warehouse leads at regional party/event rental firms. | E0 | Cannot name five. |
| 3 Evidence | Turnaround is a module inside full rental suites (Rentopian, Goodshuffle-class tools, Reservety, Growrental); check-in with photos already ships there. | E2-proxy | A standalone turnaround app would sit beside the system of record, which owns availability. |
| 8 Risks | Value 4/5 (the job is already inside the suite they use), usability 2/5, feasibility 2/5, viability 5/5 (no standalone wedge). | E0 | |
| 9 Pre-mortem | (1) Availability lives in the rental suite, so the app is double entry, (2) the job is weekly, not hourly, (3) the suite vendors add the same screen. | E0 | |

## Verdicts

| | A: Vigil | B: Event rental turnaround |
|---|---|---|
| Verdict | **Test first** | **Reframe / Park** |
| Rule applied | Riskiest assumption (usability under gloves and interruptions) at E0; a cheap test exists | Evidence points to the job living inside the rental suite, a different shape than pitched |
| Strength | The job recurs every 5 minutes during every procedure and stands alone at the point of care; the market already pays for it | Weekly job, owned by an existing system of record |

**Chosen: A, Vigil.** Its verdict is stronger (Test first against Reframe/Park), and the riskiest
assumption is usability, which a built app tests directly. User override: none. The user asked for a
build, so the build is the prototype the next test would use; it is recorded as a sample app, not as
validated product demand.

## Open questions
- No real vet tech has seen this. Before any product claim: the 5-tech timing test above.
- Drug formulary and dose ranges in the app are reference data for a sample, not clinical guidance.

Sources: [Vet Radar](https://www.vetradar.com/blog/improving-anesthesia-monitoring-for-better-patient-care), [VetMate](https://solutions.covetrus.com/vetmate), [SurgiVitals](https://mwm.ai/apps/surgivitals/6759831896), [Vetnapp](https://www.vetsurgeon.org/b/veterinary-news/archive/tags/Vetnapp), [VetMo](https://london.vetshow.com/press-release/vetmo-digital-anesthesia-management-platform-to-enhance-clinical-efficiency-and-patient-safety), [Rentopian](https://rentopian.com/seamless-check-in-and-check-out-simplifying-rentals-with-inventory-software/), [Reservety](https://reservety.com/tools/party-event-rental/party-rental-inventory-checklist.html), [Growrental](https://www.capterra.com/p/10038984/Growrental/)
