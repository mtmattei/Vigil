# Completeness audit: Vigil (2026-10-07, Uno.Sdk 6.7.30, heads: desktop, android, ios, wasm)

Verdict: **Complete with gaps** (0 Blockers, 8 Majors, 12 Minors). Updated 2026-10-08: CI green on every head (run 37791335857).
Safe to show (desktop, runtime-proven): every workflow below; on Android: board, sample day, Monitor strip,
recording a reading, persistence; on WASM: board, new case → record, persistence.
Keep off screen: export on Android/WASM/iOS (not run), anything on iOS (build-only), the sheets' look (full-window).

Method: `uno-completeness-audit` with `uno-verify` runtime proof through the App MCP (desktop, WASM) and adb on
the Android emulator. Evidence grades: R runtime, C code, I inferred, X not checked. Sources: SPEC.md,
docs/fleet/BUILD-LOG.md, docs/fleet/FINDINGS.md, artifacts/shots/, docs/atlas/ROUTE-DIFF.md.

## Inventory (source: SPEC.md)

Screens: 6 intended, 6 present, 0 unreachable, 0 missing (Board, NewCase sheet, Record with 4 tabs, Dose sheet,
SignOff sheet, Settings). Atlas route diff: built tree = spec tree.
Routes: 6 registered, 7 edges (4 XAML-declared, 3 code/UserControl, all present), 0 dead references.
Interactions: 43 XAML triggers (`Command=` / `Navigation.Request`), 26 model commands, 0 dead by construction,
0 simulated.
Critical workflows:
1. Start a case (Board → New case → Create → Record)
2. Pre-op checklist + ASA + Induce
3. Record a vitals reading (pre-filled pad, ≤ 4 taps)
4. Give a dose (calculation, out-of-range confirmation)
5. Recovery → Sign-off → read-only → Export CSV / copy summary
6. Settings (theme, language, interval, sample day, clear all)
7. Board error + Retry, empty, loading

## Completeness by area

| # | Area | Status | Evidence | Blockers | Majors | Minors | Note |
|---|---|---|---|---|---|---|---|
| 1 | Screens | Complete | R | 0 | 0 | 0 | 6/6 on desktop; Board, Settings, Record on Android; Board, NewCase, Record, Settings on WASM |
| 2 | Navigation | Complete | R | 0 | 0 | 1 | Back everywhere via NavigationBar; WASM fixed by D5 (no address-bar routing); no deep links on web (D5 tradeoff) |
| 3 | Interactions | Complete | R | 0 | 0 | 1 | Tabs need pointer input in automation (F-014) |
| 4 | Logic | Complete | R | 0 | 0 | 0 | Dose math, ranges, MAP, schedule: 50 unit tests; runtime: 0.68 mL atropine, out-of-range flagged |
| 5 | Data | Complete | R | 0 | 1 | 0 | JSON per case; persistence proven desktop restart, Android force-stop, WASM fresh load; iOS X |
| 6 | States | Complete | R | 0 | 0 | 0 | Board Value/Loading/Empty/Error+Retry proven (`--vigil-fault`, `--vigil-slow-ms`); Record/SignOff states C |
| 7 | Forms | Complete | R | 0 | 0 | 0 | NewCase validation shows exactly the missing field; vitals validation; sign-off guard |
| 8 | Authentication | N/A (decision) | C | 0 | 0 | 0 | Typed name + confirm on a shared tablet by design (SPEC capability inventory); confirm under Unresolved Questions |
| 9 | Responsive/adaptive UI | Complete | R | 0 | 0 | 1 | 420/700/1200/1280 desktop + 1080×2400 phone; bands after detail + Back; sheets full-window (F-016) |
| 10 | Platform behavior | Partial | R | 0 | 4 | 1 | Android and WASM partially driven; iOS build-only; Android clipboard toast (F-025) |
| 11 | Failure handling | Complete | R | 0 | 0 | 0 | Read fault + Retry; write fault surfaces InfoBar/notice (code + unit test); export failures reported |
| 12 | Accessibility | Partial | R | 0 | 1 | 2 | Named steppers, text flags, table alternative, strip summary; TabBarItem no peer (F-014); no screen-reader pass |
| 13 | Performance | Complete | R | 0 | 0 | 1 | No frame loop; A10 memory plateau at ~70 MB managed; startup not timed |
| 14 | Security/privacy | Complete | C | 0 | 0 | 1 | No secrets, no network; records unencrypted in app storage (sample) |
| 15 | Visual quality | Partial | R | 0 | 1 | 2 | Theme/contrast/fonts proven both themes; spec entrance motion and confirm cross-fade not built; dialog chrome (F-015) |
| 16 | Testing | Partial | R | 0 | 1 | 0 | 50 unit tests pass locally and in CI; CI green on every head (run 37791335857, 9244b45); no UI test suite |

## Critical workflows by head

| Workflow | desktop | android | ios | wasm |
|---|---|---|---|---|
| 1. Start a case | R (Ziggy: trace `Some(CaseRef)`, Record navigation success=True) | X (not driven) | X (no Mac) | R (Pistache: record opened, Back to Board) |
| 2. Pre-op + Induce | R (5 checks + ASA II round-trip to store, Induce → 00:00 / next 4:59) | X | X | X |
| 3. Record a reading | R (1 tap unchanged, 3 taps HR +4; double Record wrote 1) | R (Record at 11:39, survived force-stop) | X | X |
| 4. Give a dose | R (0.68 mL; 0.2 mg/kg → inline confirm → "Out of range, confirmed") | X | X | X |
| 5. Recovery → Sign → Export | R (signed Bella; 22-line CSV via save dialog; summary on clipboard) | X | X | X |
| 6. Settings | R (Theatre, French after restart, interval 3 min → overdue 12:50, Clear all Cancel kept 5) | R (Load sample day through native dialog) | X | R (Load sample day) |
| 7. Board states + Retry | R (all four; Retry recovered list + band) | R (empty, populated, overdue) | X | R (empty, populated) |

## Findings

### Blockers
None.

### Majors
- [Platform] iOS has no runtime evidence: CI simulator build passes (runs 37638904753, 37791335857); no device or simulator run from this host - X.
- [Platform] Android workflows 1, 2, 4, 5 not driven on the emulator - X - drive them with adb before calling Android complete.
- [Platform] WASM workflows 2-5 not driven; strip pixels on WASM not captured (App MCP screenshot timeout, Edge stale frames) - X.
- [Platform] Export (FileSavePicker, clipboard) unproven on Android, WASM, iOS (risk R4) - X.
- [Data] iOS persistence unproven - X.
- [Accessibility] Tab items expose no automation peer (`utu:TabBarItem`, F-014): a screen reader gets no tab semantics; no screen-reader pass on any head - R.
- [Visual] Spec motion not built: 280 ms page entrance and the Record-button confirm cross-fade (SPEC Animations); only the strip column fade exists - C.
- [Testing] No automated UI test suite; runtime proof is the App MCP session log - C.

### Minors
- [Responsive] `!` sheets open full-window, not the spec's right drawer (F-016) - R.
- [Visual] System dialogs (Clear all, Load sample day) use platform chrome (Fluent accent on desktop, purple on Android), not Material (F-015) - R.
- [Visual] Hot Design toolbar overlaps the NavigationBar title in Debug (debug-only) - R.
- [Platform] Android shows "Vigil pasted from your clipboard" when the Monitor opens (framework TextBox clipboard query, F-025) - R.
- [Navigation] No deep links or browser Back on WASM (D5 tradeoff) - C.
- [Accessibility] Steppers' units are symbols ("bpm", "mmHg") in both languages; spoken names carry full words - C.
- [Interactions] Tabs need pointer input in automation (same root as F-014) - R.
- [Performance] Cold start not measured - X.
- [Security] Records stored unencrypted in app-local storage; acceptable for sample data, not for a clinic - C.
- [Responsive] Board live band shows the newest anesthetized patient and names the others ("Also under anesthesia: …") rather than a band per patient - R.
- [Visual] Strip drug-row grouping for simultaneous doses (F-026) proven by code, not by a capture with grouped doses in view - C.
- [Data] `--vigil-seed` clears the store by design (DEBUG only) - C.

## Not checked (X)
- iOS runtime (all workflows) - no Mac on this host - run on a Mac with Xcode 26.3 + workload 10.0.300.
- Android workflows 1, 2, 4, 5 and export - time - adb-driven pass on `chefs36`.
- WASM workflows 2-5, export, strip pixels - App MCP screenshots time out on WASM - Playwright capture or a foreground browser.
- Screen readers (Narrator, TalkBack, VoiceOver) - not run - one pass per head.
- Gloved-hand timing with a real vet tech (the riskiest assumption) - needs a user - VALIDATION.md next test.

## Noted for other skills
- uno-audit: stack current (Uno.Sdk 6.7.30 latest stable); dead store watch removed; lint gating 0 (1 TEMPLATE, 1 WORKAROUND, both annotated).
- gold-standard-pass: band due bar given a visible track (6 px); entrance motion, dialog chrome and drawer sheets remain.

## Unresolved Questions
- Authentication N/A: typed name + confirmation on a shared tablet, no per-user sign-in. Confirm for a real deployment.
- iOS is in the target list with build-only evidence. Is a simulator/device run in scope?
- Spec substitutions stand: custom Skia strip instead of LiveCharts2, no audible reminder, CSV instead of PMS integration, no sync, no monitor integration, no patient photo.
- Sheets as full-window surfaces and platform-chrome dialogs: accept, or invest in a drawer presenter and Material ContentDialogs?
