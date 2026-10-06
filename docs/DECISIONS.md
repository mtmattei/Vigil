# Decisions

## D1. Concept: Vigil, a veterinary anesthesia record (2026-10-06)

Decision: build Vigil, the tablet anesthesia record for surgical vet techs in a small-animal hospital.
Reason: `/product-thinking` Quick mode on two candidates (docs/VALIDATION.md). Vigil earned
**Test first**; event rental turnaround earned **Reframe/Park** because its job lives inside the
rental suite that owns availability. Vigil's job recurs every 5 minutes during every procedure,
stands alone at the point of care, and its riskiest assumption (fast, gloved, interruptible entry) is
exactly what a built app tests.
Tradeoff: a crowded real market (Vet Radar, VetMate, SurgiVitals, Vetnapp, VetMo). That is acceptable
for a reference app and gives a quality bar to compare against.
Caveat: the validation answers are Claude's, graded E0/E1. Nothing here is customer evidence.

No overlap with Patina (public art), Cargo (port ops), FieldCheck (inspections), Lapse (app history),
Loadpath (truss engineering), MorningCard or UnoBusiness.
