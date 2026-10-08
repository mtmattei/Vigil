# Deploy matrix: Vigil (2026-10-07)

Uno.Sdk 6.7.30, .NET SDK 10.0.303. Single project `Vigil/Vigil.csproj`; `-p:VigilTargetFrameworks=<tfm>` builds one
head without restoring the others. Nothing has been deployed to a store, a device fleet or a public host: this
matrix records what was built, where, and with what evidence.

## Head by head

| Head | Release build | Where proven | Artifact | Runtime evidence | Distribution status |
|---|---|---|---|---|---|
| Desktop Windows (win-x64) | `dotnet publish Vigil/Vigil.csproj -f net10.0-desktop -c Release -r win-x64 --self-contained -p:VigilTargetFrameworks=net10.0-desktop` | Local (25 s, exit 0, 11:00) + CI runs 37638904753, 37791335857 | `artifacts/publish/win-x64/` (local), CI artifact `vigil-desktop-win-x64` | Every workflow, App MCP (Debug) | Not packaged (no installer, no code signing) |
| Desktop Linux (linux-x64) | same, `-r linux-x64` | CI runs 37638904753, 37791335857 (success) | CI artifact `vigil-desktop-linux-x64` | None (no Linux host run) | Not packaged |
| Desktop macOS (osx-arm64) | same, `-r osx-arm64` | CI runs 37638904753, 37791335857 (success) | CI artifact `vigil-desktop-osx-arm64` | None (no Mac) | Not packaged, not notarized |
| WebAssembly (PWA) | `dotnet publish Vigil/Vigil.csproj -f net10.0-browserwasm -c Release -p:VigilTargetFrameworks=net10.0-browserwasm` | Local (518 s, exit 0) + CI runs 37638904753, 37791335857 | `artifacts/publish/wasm/wwwroot` (local), CI artifact `vigil-wasm` | Debug in Edge: board, new case → record, persistence across a fresh load (D5) | Not hosted. CI has an opt-in GitHub Pages job (`vars.DEPLOY_PAGES == 'true'`); it has never run |
| Android | `dotnet publish … -f net10.0-android -c Release -p:VigilTargetFrameworks=net10.0-android -p:AndroidPackageFormat=apk` (CI also builds `aab`) | Local (219 s, 55.7 MB signed APK, 11:38) + CI runs 37638904753, 37791335857 | `artifacts/publish/android/com.unoplatform.vigil-Signed.apk` (debug-signed), CI artifact `vigil-android` | Release APK on emulator `chefs36` (API 36, 1080×2400): board, sample day, Monitor strip, reading survives force-stop | Debug-signed only. Play needs an upload key (CI reads `ANDROID_KEYSTORE_BASE64` + alias/passwords secrets; none set) |
| iOS | `dotnet build … -f net10.0-ios -c Release -r iossimulator-arm64 -p:VigilTargetFrameworks=net10.0-ios` with Xcode 26.3 + workload 10.0.300 | CI runs 37638904753 (33 min) and 37791335857 (40 min), success | none (simulator build check) | None: no Mac on this host | Not distributable: needs an Apple Developer account, distribution certificate, provisioning profile |

## CI

Workflow: `.github/workflows/ci.yml` (tests → desktop ×3, WASM, Android, iOS simulator; optional Pages).

| Run | Commit | Result |
|---|---|---|
| 37638904753 | 393696e | success on every head: unit tests, desktop win/linux/osx, WASM, Android, iOS simulator (iOS 14:43:49Z → 15:17:00Z) |
| 37640928242 | 23ab49a | success |
| 37643330218, 37644934790, 37644966575, 37647072013 | 20f459e … 557e2e4 | not executed: "The job was not started because an Actions budget is preventing further use." |
| 37790804563 | 3a41f89 | repo made public (2026-10-08); every head green except iOS: `CS0037` in `Controls/ThemePalette.cs` (the iOS compiler typed the palette switch as `Color`, not `Color?`; desktop accepted it) |
| 37791335857 | 9244b45 | **success on every head**: unit tests, desktop win/linux/osx, WASM, Android, iOS simulator (14:19:19Z → 14:59:14Z); Pages job skipped (opt-in) |

CI is green on every head at 9244b45 (the fix for the iOS-only compile error). The four budget-blocked commits
are covered by that run, which contains them.

## Release readiness checklist

- [x] Unit tests (50) pass locally and in CI run 37791335857.
- [x] Release builds succeed for every head (local: desktop, WASM, Android; CI run 37791335857: all six jobs).
- [x] Application id `com.unoplatform.vigil`, display version 1.0 (`Vigil/Vigil.csproj`).
- [ ] Launcher icon and splash still use the template logo (`Assets/Icons`, `Assets/Splash`).
- [ ] Code signing: Windows (none), macOS (none), Android (debug key), iOS (none).
- [ ] Store listings, privacy policy: none. Data stays on the device; there is no network use.
- [ ] Sample formulary and alarm thresholds are labelled "not for clinical use"; a practice must supply its own.

## Unresolved Questions

- WASM hosting: enable the GitHub Pages job (`DEPLOY_PAGES=true` + Pages source "GitHub Actions"), or host elsewhere?
- Android signing: provide an upload keystore as repository secrets for a signed AAB?
- iOS: is an Apple Developer account available for a device or TestFlight build?
- Replace the template launcher icon and splash before any distribution?
