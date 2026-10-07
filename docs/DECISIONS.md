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

## D2. New case opens as a result flyout, then the Board navigates to Record (2026-10-07)

Decision: Board's `NewCase` command awaits `!NewCase` for a `CaseRef` result, then navigates to `Record`.
Reason: the sheet stays a pure form (save + `NavigateBackWithResultAsync`), and Back from Record returns to the
Board because the Board did the forward navigation. Navigating to Record from inside the flyout would have
needed a back-then-forward workaround (FieldCheck gotcha).
Tradeoff: the Board model owns one extra command; the sheet cannot be reused to open a record elsewhere.

## D3. Feeds reload from a store `Signal`, not a watch stream (2026-10-07)

Decision: list and record feeds are `Feed.Async(load, store.Changed)`; Retry is a model command that raises the
same signal. The 1 Hz clock is a separate `Feed.AsyncEnumerable` combined in.
Reason: `FeedView.Refresh` over `Feed.AsyncEnumerable` → `Combine` → `AsListFeed` greyed out and never recovered
from an injected read fault (measured). The signal path recovered both Board surfaces at once.
Tradeoff: every store write reloads the whole (cached) list; fine for a day's cases, not for a hospital archive.

## D4. Out-of-range dose confirmation is inline in the dose sheet, not a ContentDialog (2026-10-07)

Decision: Give on an out-of-range dose shows an inline confirmation card in the sheet ("Change dose" filled and
default, "Give anyway" outlined). SPEC.md's Interaction Brief asked for a ContentDialog via
`ShowMessageDialogAsync`.
Reason: measured on desktop: the message dialog opened from the `!Dose` flyout dismissed the flyout itself, so
"Change dose" returned the user to the record with the entry lost. The dialog also rendered Fluent chrome with
"Give anyway" as the default (accent) button.
Tradeoff: no modal focus trap; the confirmation is announced through a LiveSetting region instead.
