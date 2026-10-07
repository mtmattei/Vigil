# HANDOFF — Vigil Phase 2 (rooted, non-stop loop)
Updated: 2026-10-07 11:38 (America/Montreal)

## Where we are
Rooted session with the App MCP (Healthy, 12 tools). Running `/loop` non-stop through SPEC.md's
Implementation Plan and Phase 3 gates; the loop stops only when `docs/COMPLETENESS.md` + `docs/DEPLOY.md`
exist with evidence and CI is green on every head, or on a user-only choice.

Done: steps 0–7. Board, NewCase, Record (Pre-op, Drugs, Monitor strip + pad, Recovery), Dose sheet, Sign-off,
Export, Settings, Theatre theme, full EN/FR (316 keys). Release path gate met: CI run 37638904753 green on all
heads incl. iOS; local Release publish desktop/WASM/Android.

## Last verified state
- Build: desktop 0 warnings. Tests 50/50. Lint gating 0. Git `main` at a5f9cff, pushed.
- Runtime (App MCP desktop): Board Value/Loading/Empty/Error+Retry; NewCase validation + create; Pre-op
  round trip; Induce; tab persistence across ticks; Back; bands 420/700/1200 after detail + Back.
- CI run 37638904753: tests, desktop ×3, WASM, Android green; iOS pending.
- Git: `main` at `23ab49a`, pushed to origin.

## Guidance applied
| Skill / rule | Applied at |
|---|---|
| mvux: record feeds in FeedView, ElementName in ErrorTemplate, `Parent.X` TwoWay | `Presentation/Record/RecordPage.xaml` |
| Gotcha: Refresh does not recover upstream failure → model Retry + store Signal | `Presentation/Board/BoardModel.cs` |
| Toolkit doc (not skill): VSM `States` on the Control, groups on first child | `Controls/LiveBandView.xaml:6` |
| uno-navigation: `!Route` for result, Navigation.Request on ItemsRepeater | `BoardModel.NewCase`, `BoardPage.xaml` |
| Gotcha: uno_app_start leaves old instance → always `Stop-Process Vigil` before relaunch | loop habit |

## Next actions (in order)
1. Step 8 persistence passes: WASM (reload), Android emulator (relaunch); R2 (SKCanvasElement on WASM/Android),
   R5 (LocalFolder on WASM). Check latest CI runs.
2. Phase 3 gates (break-it → gold-standard-pass → uno-audit → atlas extract → completeness →
   `docs/COMPLETENESS.md`, `docs/DEPLOY.md`).

## Open questions / findings so far
- SPEC Unresolved Questions stand (chart substitution, no audio, CSV not PMS, no sync, no auth, no monitor
  integration, no photo).
- NewCase sheet renders full-window (navigation flyout default), not the spec's right drawer: polish item.
- Board live band shows one anesthetized case; with two under anesthesia only the newest is in the band.
- `TabBarItem` has no automation peer (a11y + automation).
- `uno-toolkit` skill reference has VSM `States` placement backwards (Suggested Doc Update).

## Relaunch
```
Get-Process Vigil -ErrorAction SilentlyContinue | Stop-Process -Force
uno_app_start  projectPath=Vigil/Vigil.csproj  tfm=net10.0-desktop  args=--vigil-seed=sample
Hooks: --vigil-seed=sample|empty, --vigil-fault=store-read-once|store-write, --vigil-slow-ms=N
Captures incl. popups: pwsh tools/Capture-Window.ps1 -ProcessId <pid> -Out artifacts/shots/x.png
Resize: pwsh tools/Resize-Window.ps1 -Width 420 -Height 760
```
