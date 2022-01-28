[Setup]
;-- Main Setup Information
 AppName                         = Rougue Quest
 AppVerName                      = Rougue Quest 1.0.0.0
 AppCopyright                    = Copyright © 1995-2021 NetworkDLS.
 DefaultDirName                  = {commonpf}\NetworkDLS\Rougue Quest
 DefaultGroupName                = NetworkDLS\Rougue Quest
 UninstallDisplayIcon            = {app}\SSE.Exe
 PrivilegesRequired              = PowerUser
 Uninstallable                   = Yes
 Compression                     = bZIP/9
 ChangesAssociations             = Yes
 OutputBaseFilename              = Rougue Quest
 ArchitecturesInstallIn64BitMode = x64
 ArchitecturesAllowed            = x64

;-- Windows 2000 & XP (Support Dialog)
 AppPublisher    = NetworkDLS
 AppPublisherURL = http://www.NetworkDLS.com/
 AppUpdatesURL   = http://www.NetworkDLS.com/
 AppVersion      = 1.0.0.0
 
[Components]
 Name: "Game";         Description: "Game";             Types: Full Compact Custom; Flags: Fixed;
 Name: "Assets";       Description: "Assets";           Types: Full Compact Custom; Flags: Fixed;
 Name: "ScenarioEdit"; Description: "Scenario Editor";  Types: Full Custom;

[Files]
 Source: "Game\bin\Debug\net5.0-windows\*.exe";          DestDir: "{app}\Game";          Components: Game;          Flags: IgnoreVersion; 
 Source: "Game\bin\Debug\net5.0-windows\*.dll";          DestDir: "{app}\Game";          Components: Game;          Flags: IgnoreVersion;
 Source: "Game\bin\Debug\net5.0-windows\*.json";         DestDir: "{app}\Game";          Components: Game;          Flags: IgnoreVersion;
 Source: "ScenarioEdit\bin\Debug\net5.0-windows\*.exe";  DestDir: "{app}\ScenarioEdit";  Components: ScenarioEdit;  Flags: IgnoreVersion;
 Source: "ScenarioEdit\bin\Debug\net5.0-windows\*.json"; DestDir: "{app}\ScenarioEdit";  Components: ScenarioEdit;  Flags: IgnoreVersion;
 Source: "ScenarioEdit\bin\Debug\net5.0-windows\*.dll";  DestDir: "{app}\ScenarioEdit";  Components: ScenarioEdit;  Flags: IgnoreVersion;
 Source: "Assets\*.*";                                   DestDir: "{app}\Assets";        Components: Assets;        Flags: IgnoreVersion recursesubdirs;

[Registry]
 Root: HKLM; Subkey: "Software\NetworkDLS\Rogue Quest"; ValueType: STRING;ValueName: "AssetPath"; ValueData:"{app}\Assets"; Flags: CreateValueIfDoesntExist;

[Icons]
 Name: "{group}\Rougue Quest";    Filename: "{app}\Game\Game.exe";               Components: Game;
 Name: "{group}\Scenario Editor"; Filename: "{app}\ScenarioEdit\ScenarioEdit.exe"; Components: ScenarioEdit;

[Run]
 Filename: "{app}\Game\Game.exe"; Description: "Run Rougue Quest now?"; Flags: postinstall nowait skipifsilent shellexec; Components: Game;
