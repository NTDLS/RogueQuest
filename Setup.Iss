[Setup]
;-- Main Setup Information
 AppName                         = Rougue Quest
 AppVerName                      = Rougue Quest 1.0.0.1
 AppCopyright                    = Copyright © 1995-2022 NetworkDLS.
 DefaultDirName                  = {commonpf}\NetworkDLS\Rougue Quest
 DefaultGroupName                = NetworkDLS\Rougue Quest
 UninstallDisplayIcon            = {app}\Bin\Game.exe
 SetupIconFile                   = RougueQuest.ico
 PrivilegesRequired              = admin
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
 AppVersion      = 1.0.0.1
 
[Components]
 Name: "Game";         Description: "Game";             Types: Full Compact Custom; Flags: Fixed;
 Name: "Assets";       Description: "Assets";           Types: Full Compact Custom; Flags: Fixed;
 Name: "ScenarioEdit"; Description: "Scenario Editor";  Types: Full Custom;

[Files]
 Source: "Game\bin\Release\net6.0-windows\*.exe";          DestDir: "{app}\Bin";                          Components: Game;          Flags: IgnoreVersion; 
 Source: "Game\bin\Release\net6.0-windows\*.dll";          DestDir: "{app}\Bin";                          Components: Game;          Flags: IgnoreVersion;
 Source: "Game\bin\Release\net6.0-windows\*.json";         DestDir: "{app}\Bin";                          Components: Game;          Flags: IgnoreVersion;
 Source: "ScenarioEdit\bin\Release\net6.0-windows\*.exe";  DestDir: "{app}\Bin";                          Components: ScenarioEdit;  Flags: IgnoreVersion;
 Source: "ScenarioEdit\bin\Release\net6.0-windows\*.json"; DestDir: "{app}\Bin";                          Components: ScenarioEdit;  Flags: IgnoreVersion;
 Source: "ScenarioEdit\bin\Release\net6.0-windows\*.dll";  DestDir: "{app}\Bin";                          Components: ScenarioEdit;  Flags: IgnoreVersion;
 Source: "Assets\CommonAssets\*.*";                        DestDir: "{app}\Assets";                       Components: Assets;        Flags: IgnoreVersion recursesubdirs;
 Source: "Assets\UserAssets\*.*";                          DestDir: "{userappdata}\Rougue Quest";         Components: Assets;        Flags: IgnoreVersion recursesubdirs;

[Registry]
 Root: HKLM; Subkey: "Software\NetworkDLS\Rogue Quest"; ValueType: STRING;ValueName: "CommonAssetPath"; ValueData:"{app}\Assets";               Flags: CreateValueIfDoesntExist;
 Root: HKLM; Subkey: "Software\NetworkDLS\Rogue Quest"; ValueType: STRING;ValueName: "UserAssetPath";   ValueData:"{userappdata}\Rougue Quest"; Flags: CreateValueIfDoesntExist;

[Icons]
 Name: "{group}\Rougue Quest";    Filename: "{app}\Bin\Game.exe";         Components: Game;
 Name: "{group}\Scenario Editor"; Filename: "{app}\Bin\ScenarioEdit.exe"; Components: ScenarioEdit;

[Run]
 Filename: "{app}\Bin\Game.exe"; Description: "Run Rougue Quest now?"; Flags: postinstall nowait skipifsilent shellexec; Components: Game;
