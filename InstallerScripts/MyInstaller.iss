[Setup]
AppName={ExeName}
AppVersion={AppVersion}
DefaultDirName={pf}\{ExeName}
DefaultGroupName={ExeName}
OutputBaseFilename={ExeName}_Setup_{AppVersion}
Compression=lzma
SolidCompression=yes

[Files]
Source: "{SourceDir}\*.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{SourceDir}\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{SourceDir}\{ExeName}_Data\*"; DestDir: "{app}\{ExeName}_Data"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{ExeName}"; Filename: "{app}\{ExeName}.exe"
