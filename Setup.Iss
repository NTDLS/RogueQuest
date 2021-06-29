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
 Name: "Game";        Description: "Game";             Types: Full Compact Custom; Flags: Fixed;
 Name: "LevelEditor"; Description: "Level Editor";     Types: Full Custom;

[Files]
 Source: "Game\bin\Debug\net5.0-windows\*.exe";         DestDir: "{app}\Game";         Flags: IgnoreVersion; Components: Game;
 Source: "Game\bin\Debug\net5.0-windows\*.dll";         DestDir: "{app}\Game";         Flags: IgnoreVersion; Components: Game;
 Source: "Game\bin\Debug\net5.0-windows\*.json";        DestDir: "{app}\Game";         Flags: IgnoreVersion; Components: Game;
 Source: "LevelEditor\bin\Debug\net5.0-windows\*.exe";  DestDir: "{app}\LevelEditor";  Flags: IgnoreVersion; Components: LevelEditor;
 Source: "LevelEditor\bin\Debug\net5.0-windows\*.json"; DestDir: "{app}\LevelEditor";  Flags: IgnoreVersion; Components: LevelEditor;
 Source: "LevelEditor\bin\Debug\net5.0-windows\*.dll";  DestDir: "{app}\LevelEditor";  Flags: IgnoreVersion; Components: LevelEditor;
 Source: "Assets\*.*";                                  DestDir: "{app}\Assets";       Flags: IgnoreVersion recursesubdirs;

[Registry]
 Root: HKLM; Subkey: "Software\NetworkDLS\Rogue Quest"; ValueType: STRING;ValueName: "AssetPath"; ValueData:"{app}\Assets"; Flags: CreateValueIfDoesntExist;

[Icons]
 Name: "{group}\Rougue Quest";  Filename: "{app}\Game\Game.exe";               Components: Game;
 Name: "{group}\Level Editor";  Filename: "{app}\LevelEditor\LevelEditor.exe"; Components: LevelEditor;

[Run]
 Filename: "{app}\Game\Game.exe"; Description: "Run Rougue Quest now?"; Flags: postinstall nowait skipifsilent shellexec; Components: Game;
