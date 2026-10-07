# HANDOFF — Vigil Phase 2–3 (loop stopped on a user decision)
Updated: 2026-10-07 12:12 (America/Montreal)

## Where we are
Implementation Plan steps 0–8 done; Phase 3 gates run: break-it round 1, gold-standard Fit & Finish item,
uno-audit hygiene, atlas route diff, completeness audit. `docs/COMPLETENESS.md` (Complete with gaps: 0 Blockers,
9 Majors) and `docs/DEPLOY.md` exist with evidence. The `/loop` stopped because CI cannot run: GitHub Actions
budget exhausted ("The job was not started because an Actions budget is preventing further use."), which only
the user can resolve.

## Last verified state
- Git: `main` at `fd45dba`+, pushed to private `mtmattei/Vigil`.
- Build: desktop 0 warnings. Tests 50/50. Lint gating 0 (1 TEMPLATE + 1 WORKAROUND, annotated).
- CI: last executed runs green (37638904753 all heads incl. iOS; 37640928242). Runs since 20f459e not started (budget).
- Runtime: desktop all 7 critical workflows (App MCP); Android emulator `chefs36` Release APK: board, sample day,
  strip, reading survives force-stop; WASM: board, new case → record (after D5), persistence across fresh load.

## Guidance applied
| Skill / rule / decision | Applied at |
|---|---|
| mvux: record feeds, ElementName in ErrorTemplate, `Parent.X` TwoWay | `Presentation/Record/RecordPage.xaml` |
| D3: feeds reload from store Signal; Retry raises it | `Presentation/Board/BoardModel.cs` |
| Toolkit doc: VSM `States` on the Control | `Controls/LiveBandView.xaml` |
| uno-navigation: `!` result route then navigate (D2); D5 no address bar | `BoardModel.NewCase`, `App.xaml.cs` |
| skiasharp-uno: SKCanvasElement, cached paints, palette snapshot, dispose on Unloaded | `Controls/VitalsStrip.cs`, `StripRenderer.cs` |
| Gotcha workaround: static card template (Toolkit washout) | `Themes/Controls.xaml` |
| Localization: x:Uid + `Loc`, generator `tools/localize.py` + `tools/fr.json` | `Strings/{en,fr}/Resources.resw` |

## Next actions (in order)
1. User decision on CI (budget / public repo / accept local). Then re-run CI on `main` and update DEPLOY.md.
2. Majors in COMPLETENESS.md: drive Android workflows 1, 2, 4, 5 + export via adb; WASM workflows 2–5; iOS run on a Mac.
3. Spec motion: 280 ms page entrance + Record confirm cross-fade (with reduced motion).
4. Minors: F-014 TabBarItem peer (upstream), F-015 Material dialogs, F-016 drawer sheets, F-025 clipboard toast.

## Relaunch
```
Get-Process Vigil -ErrorAction SilentlyContinue | Stop-Process -Force
uno_app_start  projectPath=Vigil/Vigil.csproj  tfm=net10.0-desktop  args=--vigil-seed=sample
Hooks (DEBUG): --vigil-seed=sample|empty, --vigil-fault=store-read-once|store-write, --vigil-slow-ms=N, --vigil-memlog
Captures incl. popups: pwsh tools/Capture-Window.ps1 -ProcessId <pid> -Out artifacts/shots/x.png
Resize: pwsh tools/Resize-Window.ps1 -Width 420 -Height 760 ; Save dialog: powershell tools/Complete-SaveDialog.ps1 -Path <file>
Strings: python tools/localize.py --write   (fails on missing French)
WASM: stop any stale dotnet on :5000 before uno_app_start; open http://localhost:5000 in a new tab
Android: emulator -avd chefs36; adb install -r artifacts/publish/android/com.unoplatform.vigil-Signed.apk
```
