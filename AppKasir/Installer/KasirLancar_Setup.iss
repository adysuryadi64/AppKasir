#define MyAppName "Kasir Lancar"
#define MyAppVersion "2026.08.28.0"
#define MyAppPublisher "Kasir Lancar"
#define MyAppExeName "KasirLancar.exe"
#define MyAppSourceDir "..\bin\Debug"
#define MyAppDriverDir "..\Printer Driver Software"
#define MyAppFontDir "..\Fonts"
#define MyAppMySQLDir "..\MySQL"
#define MyAppURL "https://kasirlancar.com"

; ============================================================
;  KASIR LANCAR - Modern Installer Script (Inno Setup 6.x)
; ============================================================

[Setup]
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} v{#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

; Default install ke D:\Kasir Lancar (fallback ke C:\ jika D: tidak ada)
; Fungsi GetDefaultInstallDir di bagian Code menangani deteksi drive otomatis
DefaultDirName={code:GetDefaultInstallDir}
DefaultGroupName={#MyAppName}
AllowNoIcons=no
DirExistsWarning=no

; Output
OutputDir=Output
OutputBaseFilename=KasirLancar_Setup_v2026.08.28.0

; Icon
SetupIconFile=Kasir lancar.ico

; Kompresi maksimal
Compression=lzma2/ultra64
SolidCompression=yes
LZMAUseSeparateProcess=yes

; Tampilan Modern
WizardStyle=modern
WizardSizePercent=130
WizardResizable=no

; Gambar wizard modern - generate dulu dengan create_installer_images.ps1
; installer_banner.bmp  = 164x314 px (sidebar kiri)
; installer_logo.bmp    = 55x55 px  (pojok kanan atas)
WizardImageFile=installer_banner.bmp
WizardSmallImageFile=installer_logo.bmp

; Hak akses & arsitektur
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible

; Uninstall
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName} v{#MyAppVersion}
CreateUninstallRegKey=yes

; Misc
ShowLanguageDialog=no
DisableWelcomePage=no
DisableDirPage=no
DisableProgramGroupPage=no
UsePreviousAppDir=yes
UsedUserAreasWarning=no

; ============================================================
[Languages]
Name: "indonesian"; MessagesFile: "compiler:Default.isl"

; ============================================================
[Messages]
WelcomeLabel1=Selamat Datang%ndi {#MyAppName}
WelcomeLabel2=Anda akan menginstal [name/ver] di komputer ini.%n%nTutup semua aplikasi yang sedang berjalan sebelum melanjutkan untuk menghindari konflik file.%n%nKlik Berikutnya untuk memulai.
FinishedHeadingLabel=[name] Siap Digunakan
FinishedLabel=[name/ver] telah berhasil dipasang di komputer Anda.%n%nKlik Selesai untuk menutup wizard ini.
ClickNext=Klik Berikutnya untuk melanjutkan.
SelectDirLabel3=Pilih folder tujuan instalasi [name]:
SelectDirBrowseLabel=Klik Berikutnya untuk melanjutkan, atau Jelajahi untuk memilih folder lain.
DirExistsTitle=Folder Sudah Ada
DirExists=Folder berikut sudah ada:%n%n%1%n%nInstal ke folder ini?
SelectComponentsLabel2=Pilih komponen yang akan dipasang. Hapus centang komponen yang tidak diperlukan.

; ============================================================
[CustomMessages]
ComponentMainApp=Aplikasi Kasir Lancar (Wajib)
ComponentAppServ=AppServ 9.3.0  —  Apache + MySQL Server (khusus Server)
ComponentReportViewer=Microsoft Report Viewer 2015
ComponentMySQLConnector=MySQL Connector .NET 9.1.0
ComponentVCRedist=Visual C++ Redistributable (x64 + x86)
ComponentPOSPrinter=POS Printer Driver
StatusAppServ=Memasang AppServ (Apache + MySQL)...
StatusReportViewer=Memasang Microsoft Report Viewer...
StatusMySQLConnector=Memasang MySQL Connector .NET...
StatusVCRedist=Memasang Visual C++ Redistributable...
StatusPOSPrinter=Memasang POS Printer Driver...
StatusConfigAPI=Mengkonfigurasi API server...
LaunchApp=Jalankan {#MyAppName} sekarang

; ============================================================
[Types]
Name: "server"; Description: "Instalasi Baru — Server (AppServ + Aplikasi)"
Name: "client"; Description: "Instalasi Baru — Client (Aplikasi saja, tanpa AppServ)"
Name: "update"; Description: "Update — Hanya file aplikasi"
Name: "custom"; Description: "Kustom"; Flags: iscustom

; ============================================================
[Components]
Name: "mainapp";       Description: "{cm:ComponentMainApp}";        Types: server client update custom; Flags: fixed
Name: "appserv";       Description: "{cm:ComponentAppServ}";        Types: server
Name: "reportviewer";  Description: "{cm:ComponentReportViewer}";   Types: server client
Name: "mysqlconn";     Description: "{cm:ComponentMySQLConnector}"; Types: server client
Name: "vcredist";      Description: "{cm:ComponentVCRedist}";       Types: server client
Name: "posprinter";    Description: "{cm:ComponentPOSPrinter}";     Types: server client

; ============================================================
[Tasks]
Name: "desktopicon";    Description: "Buat shortcut di Desktop";        GroupDescription: "Shortcut Tambahan:"
Name: "startmenuicon";  Description: "Buat shortcut di Start Menu";     GroupDescription: "Shortcut Tambahan:"; Flags: checkedonce
Name: "startupicon";    Description: "Jalankan otomatis saat Windows startup"; GroupDescription: "Shortcut Tambahan:"; Flags: unchecked

; ============================================================
[Files]
; AUTO-GENERATED oleh Build-Installer.ps1 - 2026-08-28 02:24:01
; Total file di bin\Debug: 870

; ----- File Utama Aplikasi -----
Source: "{#MyAppSourceDir}\_dashboard_tmp.html"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\AutoUpdater.NET.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\AutoUpdater.NET.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Azure.Core.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Azure.Core.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Azure.Identity.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Azure.Identity.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\BouncyCastle.Crypto.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\BouncyCastle.Crypto.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\BouncyCastle.Cryptography.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\BouncyCastle.Cryptography.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\chart.min.js"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\ClosedXML.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\ClosedXML.Parser.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\ClosedXML.Parser.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\ClosedXML.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\config.bin"; DestDir: "{app}"; Flags: onlyifdoesntexist; Components: mainapp
Source: "{#MyAppSourceDir}\database.json"; DestDir: "{app}"; Flags: onlyifdoesntexist; Components: mainapp
Source: "{#MyAppSourceDir}\DocumentFormat.OpenXml.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\DocumentFormat.OpenXml.Framework.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\DocumentFormat.OpenXml.Framework.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\DocumentFormat.OpenXml.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\EnvDTE.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\ESCPOS_NET.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\ExcelNumberFormat.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\ExcelNumberFormat.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\FastReport.Compat.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\FastReport.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\FastReport.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Google.Protobuf.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Google.Protobuf.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\guide.html"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\itextsharp.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\itextsharp.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\K4os.Compression.LZ4.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\K4os.Compression.LZ4.Streams.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\K4os.Compression.LZ4.Streams.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\K4os.Compression.LZ4.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\K4os.Hash.xxHash.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\K4os.Hash.xxHash.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Kasir Lancar.xml"; DestDir: "{app}"; Flags: onlyifdoesntexist; Components: mainapp
Source: "{#MyAppSourceDir}\KasirLancar.exe"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\KasirLancar.exe.config"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\license.ini"; DestDir: "{app}"; Flags: onlyifdoesntexist; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Bcl.AsyncInterfaces.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Bcl.AsyncInterfaces.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Bcl.Cryptography.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Bcl.Cryptography.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Bcl.HashCode.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Bcl.HashCode.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Bcl.Memory.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Bcl.Memory.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Bcl.TimeProvider.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Bcl.TimeProvider.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Data.SqlClient.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Data.SqlClient.SNI.arm64.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Data.SqlClient.SNI.x64.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Data.SqlClient.SNI.x86.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Data.SqlClient.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Caching.Abstractions.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Caching.Abstractions.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Caching.Memory.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Caching.Memory.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.DependencyInjection.Abstractions.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.DependencyInjection.Abstractions.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.DependencyInjection.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.DependencyInjection.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Logging.Abstractions.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Logging.Abstractions.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Logging.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Logging.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Options.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Options.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Primitives.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Extensions.Primitives.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Identity.Client.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Identity.Client.Extensions.Msal.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Identity.Client.Extensions.Msal.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Identity.Client.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.Abstractions.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.Abstractions.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.JsonWebTokens.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.JsonWebTokens.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.Logging.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.Logging.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.Protocols.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.Protocols.OpenIdConnect.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.Protocols.OpenIdConnect.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.Protocols.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.Tokens.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.IdentityModel.Tokens.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.ReportViewer.Common.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.ReportViewer.DataVisualization.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.ReportViewer.Design.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.ReportViewer.ProcessingObjectModel.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.ReportViewer.WinForms.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.SqlServer.Types.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.VisualBasic.PowerPacks.Vs.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Web.WebView2.Core.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Web.WebView2.Core.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Web.WebView2.WinForms.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Microsoft.Web.WebView2.WinForms.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\MySql.Data.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\MySql.Data.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\Newtonsoft.Json.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\pengaturan_cetak.ini"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\perilaku_cetak.ini"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\RBush.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\RBush.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\SimpleTcp.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\SimpleTcp.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\SixLabors.Fonts.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\SixLabors.Fonts.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\stdole.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Buffers.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Buffers.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.ClientModel.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.ClientModel.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Configuration.ConfigurationManager.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Configuration.ConfigurationManager.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Diagnostics.DiagnosticSource.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Diagnostics.DiagnosticSource.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Formats.Asn1.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Formats.Asn1.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.IdentityModel.Tokens.Jwt.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.IdentityModel.Tokens.Jwt.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.IO.FileSystem.AccessControl.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.IO.FileSystem.AccessControl.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.IO.Pipelines.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.IO.Pipelines.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.IO.Ports.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.IO.Ports.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Memory.Data.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Memory.Data.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Memory.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Memory.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Numerics.Vectors.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Numerics.Vectors.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Runtime.CompilerServices.Unsafe.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Runtime.CompilerServices.Unsafe.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Security.AccessControl.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Security.AccessControl.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Security.Cryptography.Pkcs.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Security.Cryptography.Pkcs.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Security.Cryptography.ProtectedData.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Security.Cryptography.ProtectedData.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Security.Principal.Windows.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Security.Principal.Windows.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Text.Encoding.CodePages.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Text.Encoding.CodePages.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Text.Encodings.Web.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Text.Encodings.Web.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Text.Json.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Text.Json.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Threading.Tasks.Extensions.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\System.Threading.Tasks.Extensions.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\TemplateLabaRugi.html"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\WebView2Loader.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\ZstdSharp.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\zxing.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\zxing.presentation.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\zxing.presentation.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppSourceDir}\zxing.xml"; DestDir: "{app}"; Flags: ignoreversion; Components: mainapp

; ----- Printer Driver Software (7 file) -----
; Semua file driver disertakan ke {app}\Printer Driver Software
Source: "{#MyAppDriverDir}\appserv-9-3-0.exe"; DestDir: "{app}\Printer Driver Software"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppDriverDir}\mysql-connector-net-9.1.0.msi"; DestDir: "{app}\Printer Driver Software"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppDriverDir}\POS Printer Driver Setup .exe"; DestDir: "{app}\Printer Driver Software"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppDriverDir}\ReportViewer.exe"; DestDir: "{app}\Printer Driver Software"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppDriverDir}\tsc_driver.exe"; DestDir: "{app}\Printer Driver Software"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppDriverDir}\VC_redist.x64.exe"; DestDir: "{app}\Printer Driver Software"; Flags: ignoreversion; Components: mainapp
Source: "{#MyAppDriverDir}\VC_redist.x86.exe"; DestDir: "{app}\Printer Driver Software"; Flags: ignoreversion; Components: mainapp

; Installer prerequisite juga ke {tmp} untuk dijalankan
Source: "{#MyAppDriverDir}\mysql-connector-net-9.1.0.msi"; DestDir: "{tmp}"; Flags: deleteafterinstall; Components: mysqlconn
Source: "{#MyAppDriverDir}\appserv-9-3-0.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Components: appserv
Source: "{#MyAppDriverDir}\VC_redist.x86.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Components: vcredist
Source: "{#MyAppDriverDir}\VC_redist.x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Components: vcredist
Source: "{#MyAppDriverDir}\ReportViewer.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Components: reportviewer
Source: "{#MyAppDriverDir}\POS Printer Driver Setup .exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Components: posprinter

; ----- ar (1 file) -----
Source: "{#MyAppSourceDir}\ar\*"; DestDir: "{app}\ar"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- cs (2 file) -----
Source: "{#MyAppSourceDir}\cs\*"; DestDir: "{app}\cs"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- da (1 file) -----
Source: "{#MyAppSourceDir}\da\*"; DestDir: "{app}\da"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- de (7 file) -----
Source: "{#MyAppSourceDir}\de\*"; DestDir: "{app}\de"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- es (7 file) -----
Source: "{#MyAppSourceDir}\es\*"; DestDir: "{app}\es"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- fr (7 file) -----
Source: "{#MyAppSourceDir}\fr\*"; DestDir: "{app}\fr"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- it (7 file) -----
Source: "{#MyAppSourceDir}\it\*"; DestDir: "{app}\it"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- ja (6 file) -----
Source: "{#MyAppSourceDir}\ja\*"; DestDir: "{app}\ja"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- ja-JP (1 file) -----
Source: "{#MyAppSourceDir}\ja-JP\*"; DestDir: "{app}\ja-JP"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- ko (7 file) -----
Source: "{#MyAppSourceDir}\ko\*"; DestDir: "{app}\ko"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- lv (1 file) -----
Source: "{#MyAppSourceDir}\lv\*"; DestDir: "{app}\lv"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- nl (1 file) -----
Source: "{#MyAppSourceDir}\nl\*"; DestDir: "{app}\nl"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- pl (2 file) -----
Source: "{#MyAppSourceDir}\pl\*"; DestDir: "{app}\pl"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- pt (6 file) -----
Source: "{#MyAppSourceDir}\pt\*"; DestDir: "{app}\pt"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- pt-BR (2 file) -----
Source: "{#MyAppSourceDir}\pt-BR\*"; DestDir: "{app}\pt-BR"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- Resources (612 file) -----
Source: "{#MyAppSourceDir}\Resources\*"; DestDir: "{app}\Resources"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- ru (7 file) -----
Source: "{#MyAppSourceDir}\ru\*"; DestDir: "{app}\ru"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- sk (1 file) -----
Source: "{#MyAppSourceDir}\sk\*"; DestDir: "{app}\sk"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- sv (1 file) -----
Source: "{#MyAppSourceDir}\sv\*"; DestDir: "{app}\sv"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- th (1 file) -----
Source: "{#MyAppSourceDir}\th\*"; DestDir: "{app}\th"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- tr (2 file) -----
Source: "{#MyAppSourceDir}\tr\*"; DestDir: "{app}\tr"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- x64 (1 file) -----
Source: "{#MyAppSourceDir}\x64\*"; DestDir: "{app}\x64"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- x86 (1 file) -----
Source: "{#MyAppSourceDir}\x86\*"; DestDir: "{app}\x86"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- zh (1 file) -----
Source: "{#MyAppSourceDir}\zh\*"; DestDir: "{app}\zh"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- zh-CHS (4 file) -----
Source: "{#MyAppSourceDir}\zh-CHS\*"; DestDir: "{app}\zh-CHS"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- zh-CHT (4 file) -----
Source: "{#MyAppSourceDir}\zh-CHT\*"; DestDir: "{app}\zh-CHT"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- zh-Hans (2 file) -----
Source: "{#MyAppSourceDir}\zh-Hans\*"; DestDir: "{app}\zh-Hans"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- zh-Hant (2 file) -----
Source: "{#MyAppSourceDir}\zh-Hant\*"; DestDir: "{app}\zh-Hant"; Flags: ignoreversion recursesubdirs; Components: mainapp

; ----- zh-TW (1 file) -----
Source: "{#MyAppSourceDir}\zh-TW\*"; DestDir: "{app}\zh-TW"; Flags: ignoreversion recursesubdirs; Components: mainapp


; ----- Database Migration & Scripts (Auto-include all) -----
Source: "..\Database\*"; DestDir: "{app}\Database"; Flags: ignoreversion; Components: mainapp
; ----- Database Default Master (Data Kategori, Satuan, Merk) -----
Source: "..\database_Default_Master\*"; DestDir: "{app}\database_Default_Master"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: mainapp

[Icons]
Name: "{group}\{#MyAppName}";           Filename: "{app}\{#MyAppExeName}"; Tasks: startmenuicon
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}";        Tasks: startmenuicon
Name: "{autodesktop}\{#MyAppName}";     Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userstartup}\{#MyAppName}";     Filename: "{app}\{#MyAppExeName}"; Tasks: startupicon

; ============================================================
[Run]
; VC++ Redistributable — dipasang pertama karena dependency AppServ & driver lain
Filename: "{tmp}\VC_redist.x64.exe"; Parameters: "/install /quiet /norestart"; StatusMsg: "{cm:StatusVCRedist}"; Components: vcredist
Filename: "{tmp}\VC_redist.x86.exe"; Parameters: "/install /quiet /norestart"; StatusMsg: "{cm:StatusVCRedist}"; Components: vcredist

; AppServ - tampil normal, user set password sendiri lewat wizard AppServ
Filename: "{tmp}\appserv-9-3-0.exe"; StatusMsg: "{cm:StatusAppServ}"; Components: appserv

; ReportViewer
Filename: "{tmp}\ReportViewer.exe"; Parameters: "/q /norestart"; StatusMsg: "{cm:StatusReportViewer}"; Components: reportviewer

; MySQL Connector
Filename: "msiexec.exe"; Parameters: "/i ""{tmp}\mysql-connector-net-9.1.0.msi"" /qn /norestart"; StatusMsg: "{cm:StatusMySQLConnector}"; Components: mysqlconn

; POS Printer Driver
Filename: "{tmp}\POS Printer Driver Setup .exe"; Parameters: "/S"; StatusMsg: "{cm:StatusPOSPrinter}"; Components: posprinter

; Jalankan aplikasi setelah selesai
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchApp}"; Flags: nowait postinstall skipifsilent

; ============================================================
[UninstallDelete]
Type: filesandordirs; Name: "{app}\Logs"
Type: filesandordirs; Name: "{app}\Backup"
Type: filesandordirs; Name: "{app}\Database"

; ============================================================


[Code]

{ ================================================================
  VARIABEL GLOBAL
  ================================================================ }
var
  { Mode instalasi: 'server' | 'client' | 'update' }
  InstallMode : string;

  { Password MySQL — diisi via InputBox setelah AppServ selesai }
  MySQLPassword : string;

  { Halaman pilihan mode }
  PageMode        : TWizardPage;
  RdoServer       : TRadioButton;
  RdoClient       : TRadioButton;
  RdoUpdate       : TRadioButton;
  LblModeInfo     : TLabel;

  { Halaman restore database }
  PageRestore     : TWizardPage;
  LblRestoreDesc  : TLabel;
  LblZipPath      : TLabel;
  EdtZipPath      : TEdit;
  BtnBrowseZip    : TButton;
  ChkSkipRestore  : TCheckBox;
  LblStatus       : TLabel;
  SelectedZipFile : string;

{ ================================================================
  HELPER — cek font sudah terdaftar di registry Windows atau belum
  Registry font: HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts
  Nilai = nama font persis seperti yang muncul di Font Manager Windows
  Return True  = font BELUM ada → installer akan mengkopi file
  Return False = font SUDAH ada → installer skip file ini
  ================================================================ }
{ ================================================================
  HELPER — cek koneksi internet via WinInet
  Return True  = ada koneksi internet
  Return False = tidak ada koneksi / offline
  ================================================================ }
function InternetGetConnectedState(lpdwFlags: DWORD; dwReserved: DWORD): BOOL;
  external 'InternetGetConnectedState@wininet.dll stdcall';

function IsInternetConnected(): Boolean;
var
  Flags: DWORD;
begin
  Result := False;
  try
    Result := InternetGetConnectedState(Flags, 0);
  except
    Result := False;
  end;
end;

function FontBelumAda(NamaFont: string): Boolean;
var
  Dummy: string;
begin
  Result := not RegQueryStringValue(
    HKLM,
    'SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts',
    NamaFont,
    Dummy);
end;

{ ================================================================
  HELPER — path htdocs AppServ (D:\AppServ\www atau C:\AppServ\www)
  ================================================================ }
function GetAppServWww(Param: string): string;
begin
  if DirExists('D:\AppServ') then Result := 'D:\AppServ\www'
  else Result := 'C:\AppServ\www';
end;

{ ================================================================
  HELPER
  ================================================================ }
function ExecAndWait(Prog, Params, WorkDir: string): Integer;
var RC: Integer;
begin
  Exec(Prog, Params, WorkDir, SW_HIDE, ewWaitUntilTerminated, RC);
  Result := RC;
end;

{ ================================================================
  HALAMAN PILIHAN MODE: SERVER / CLIENT / UPDATE
  ================================================================ }
procedure UpdateModeInfo();
begin
  if InstallMode = 'server' then
    LblModeInfo.Caption :=
      'Instalasi Server memasang semua komponen:' + #13#10 +
      '  •  AppServ (Apache + MySQL Server)' + #13#10 +
      '  •  API PHP (dikopi ke htdocs AppServ)' + #13#10 +
      '  •  ReportViewer, MySQL Connector, VC++ Redist' + #13#10 +
      '  •  POS Printer Driver' + #13#10 + #13#10 +
      'Wizard AppServ akan terbuka — set password MySQL di sana.' + #13#10 +
      'Setelah semua komponen terpasang, Anda diminta konfirmasi password.'
  else if InstallMode = 'client' then
    LblModeInfo.Caption :=
      'Instalasi Client memasang komponen untuk komputer kasir:' + #13#10 +
      '  •  Aplikasi Kasir Lancar' + #13#10 +
      '  •  ReportViewer, MySQL Connector, VC++ Redist' + #13#10 +
      '  •  POS Printer Driver' + #13#10 + #13#10 +
      'AppServ TIDAK dipasang — client konek ke server di jaringan.'
  else
    LblModeInfo.Caption :=
      'Update hanya mengganti file aplikasi.' + #13#10 +
      'AppServ, MySQL, dan driver TIDAK akan diinstal ulang.' + #13#10 + #13#10 +
      'Arahkan ke folder instalasi Kasir Lancar yang sudah ada.' + #13#10 +
      'Installer akan memverifikasi folder sebelum melanjutkan.';
end;

procedure RdoServerClick(Sender: TObject);
begin
  InstallMode := 'server';
  UpdateModeInfo();
end;

procedure RdoClientClick(Sender: TObject);
begin
  InstallMode := 'client';
  UpdateModeInfo();
end;

procedure RdoUpdateClick(Sender: TObject);
begin
  InstallMode := 'update';
  UpdateModeInfo();
end;

procedure CreateModePage();
var
  S    : TWinControl;
  W    : Integer;
  LblHeader : TLabel;
begin
  PageMode := CreateCustomPage(wpWelcome,
    'Pilih Jenis Instalasi',
    'Tentukan jenis instalasi yang sesuai dengan komputer ini');
  S := PageMode.Surface;
  W := S.Width;

  LblHeader := TLabel.Create(S);
  LblHeader.Parent   := S;
  LblHeader.Left     := 0;
  LblHeader.Top      := 0;
  LblHeader.Width    := W;
  LblHeader.Caption  := 'Pilih salah satu opsi di bawah ini:';
  LblHeader.Font.Name := 'Segoe UI';
  LblHeader.Font.Size := 9;
  LblHeader.AutoSize := True;

  { Radio: Server }
  RdoServer := TRadioButton.Create(S);
  RdoServer.Parent   := S;
  RdoServer.Left     := 4;
  RdoServer.Top      := 22;
  RdoServer.Width    := W - 4;
  RdoServer.Height   := 20;
  RdoServer.Caption  := 'Instalasi Baru — Server  (komputer utama dengan MySQL)';
  RdoServer.Checked  := True;
  RdoServer.Font.Name := 'Segoe UI';
  RdoServer.Font.Size := 9;
  RdoServer.OnClick  := @RdoServerClick;

  { Radio: Client }
  RdoClient := TRadioButton.Create(S);
  RdoClient.Parent   := S;
  RdoClient.Left     := 4;
  RdoClient.Top      := 46;
  RdoClient.Width    := W - 4;
  RdoClient.Height   := 20;
  RdoClient.Caption  := 'Instalasi Baru — Client  (komputer kasir, konek ke server)';
  RdoClient.Font.Name := 'Segoe UI';
  RdoClient.Font.Size := 9;
  RdoClient.OnClick  := @RdoClientClick;

  { Radio: Update }
  RdoUpdate := TRadioButton.Create(S);
  RdoUpdate.Parent  := S;
  RdoUpdate.Left    := 4;
  RdoUpdate.Top     := 70;
  RdoUpdate.Width   := W - 4;
  RdoUpdate.Height  := 20;
  RdoUpdate.Caption := 'Update / Perbarui  —  Kasir Lancar sudah terpasang sebelumnya';
  RdoUpdate.Font.Name := 'Segoe UI';
  RdoUpdate.Font.Size := 9;
  RdoUpdate.OnClick := @RdoUpdateClick;

  { Info box }
  LblModeInfo := TLabel.Create(S);
  LblModeInfo.Parent    := S;
  LblModeInfo.Left      := 4;
  LblModeInfo.Top       := 100;
  LblModeInfo.Width     := W - 8;
  LblModeInfo.Height    := 120;
  LblModeInfo.AutoSize  := False;
  LblModeInfo.WordWrap  := True;
  LblModeInfo.Font.Name := 'Segoe UI';
  LblModeInfo.Font.Size := 9;
  UpdateModeInfo();
end;

{ ================================================================
  HALAMAN RESTORE DATABASE
  ================================================================ }
procedure BrowseZipClick(Sender: TObject);
var F: string;
begin
  F := '';
  if GetOpenFileName('Pilih file backup database (.zip)', F, '',
    'File ZIP Backup (*.zip)|*.zip|Semua File (*.*)|*.*', 'zip') then
  begin
    SelectedZipFile   := F;
    EdtZipPath.Text   := F;
    LblStatus.Caption := '';
  end;
end;

procedure ChkSkipRestoreClick(Sender: TObject);
begin
  EdtZipPath.Enabled   := not ChkSkipRestore.Checked;
  BtnBrowseZip.Enabled := not ChkSkipRestore.Checked;
  if ChkSkipRestore.Checked then
    LblStatus.Caption := 'Restore database akan dilewati.'
  else
    LblStatus.Caption := '';
end;

procedure CreateRestorePage();
var
  S : TWinControl;
  W : Integer;
begin
  PageRestore := CreateCustomPage(wpSelectDir,
    'Restore Database',
    'Pilih file backup (.zip) untuk merestore database Kasir Lancar');
  S := PageRestore.Surface;
  W := S.Width;

  LblRestoreDesc := TLabel.Create(S);
  LblRestoreDesc.Parent   := S;
  LblRestoreDesc.Left     := 0;
  LblRestoreDesc.Top      := 0;
  LblRestoreDesc.Width    := W;
  LblRestoreDesc.Height   := 34;
  LblRestoreDesc.AutoSize := False;
  LblRestoreDesc.WordWrap := True;
  LblRestoreDesc.Font.Name := 'Segoe UI';
  LblRestoreDesc.Font.Size := 9;
  LblRestoreDesc.Caption  :=
    'Pilih file backup ZIP untuk merestore database.' + #13#10 +
    'File ZIP harus berisi file .sql hasil backup Kasir Lancar.';

  LblZipPath := TLabel.Create(S);
  LblZipPath.Parent  := S;
  LblZipPath.Left    := 0;
  LblZipPath.Top     := 46;
  LblZipPath.Caption := 'File Backup Database (.zip):';
  LblZipPath.Font.Name := 'Segoe UI';
  LblZipPath.Font.Size := 9;

  { Edit + Tombol Browse dalam satu baris, tidak tabrakan }
  EdtZipPath := TEdit.Create(S);
  EdtZipPath.Parent   := S;
  EdtZipPath.Left     := 0;
  EdtZipPath.Top      := 62;
  EdtZipPath.Width    := W - 96;
  EdtZipPath.Height   := 23;
  EdtZipPath.ReadOnly := True;

  BtnBrowseZip := TButton.Create(S);
  BtnBrowseZip.Parent   := S;
  BtnBrowseZip.Left     := W - 92;
  BtnBrowseZip.Top      := 61;
  BtnBrowseZip.Width    := 92;
  BtnBrowseZip.Height   := 25;
  BtnBrowseZip.Caption  := 'Pilih File...';
  BtnBrowseZip.OnClick  := @BrowseZipClick;

  ChkSkipRestore := TCheckBox.Create(S);
  ChkSkipRestore.Parent   := S;
  ChkSkipRestore.Left     := 0;
  ChkSkipRestore.Top      := 98;
  ChkSkipRestore.Width    := W;
  ChkSkipRestore.Height   := 20;
  ChkSkipRestore.Caption  := 'Lewati restore database (saya akan restore manual nanti)';
  ChkSkipRestore.Checked  := False;
  ChkSkipRestore.Font.Name := 'Segoe UI';
  ChkSkipRestore.Font.Size := 9;
  ChkSkipRestore.OnClick  := @ChkSkipRestoreClick;

  LblStatus := TLabel.Create(S);
  LblStatus.Parent   := S;
  LblStatus.Left     := 0;
  LblStatus.Top      := 128;
  LblStatus.Width    := W;
  LblStatus.Height   := 50;
  LblStatus.AutoSize := False;
  LblStatus.WordWrap := True;
  LblStatus.Caption  := '';
end;

{ ================================================================
  RESTORE DATABASE
  ================================================================ }
procedure DoRestoreDatabase();
var
  ZipFile, TempExtract, SqlFile : string;
  PSScript, PSScriptPath, MySQLExe, Params : string;
  RC      : Integer;
  FindRec : TFindRec;
  Found   : Boolean;
begin
  ZipFile := EdtZipPath.Text;

  if ZipFile = '' then begin MsgBox('Pilih file backup ZIP terlebih dahulu.', mbError, MB_OK); Exit; end;
  if not FileExists(ZipFile) then begin MsgBox('File ZIP tidak ditemukan.', mbError, MB_OK); Exit; end;

  LblStatus.Caption := 'Mengekstrak file backup...';

  TempExtract  := ExpandConstant('{tmp}') + '\db_restore';
  PSScript     := 'Add-Type -AssemblyName System.IO.Compression.FileSystem; ' +
                  '[System.IO.Compression.ZipFile]::ExtractToDirectory(''' +
                  ZipFile + ''', ''' + TempExtract + ''');';
  PSScriptPath := ExpandConstant('{tmp}') + '\extract_backup.ps1';
  SaveStringToFile(PSScriptPath, PSScript, False);

  RC := ExecAndWait('powershell.exe',
    '-ExecutionPolicy Bypass -NonInteractive -File "' + PSScriptPath + '"',
    ExpandConstant('{tmp}'));
  if RC <> 0 then begin LblStatus.Caption := 'GAGAL mengekstrak ZIP.'; Exit; end;

  SqlFile := '';
  Found   := False;
  if FindFirst(TempExtract + '\*.sql', FindRec) then
  begin
    repeat
      if FindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY = 0 then
      begin
        SqlFile := TempExtract + '\' + FindRec.Name;
        Found   := True;
        Break;
      end;
    until not FindNext(FindRec);
    FindClose(FindRec);
  end;
  if not Found then begin LblStatus.Caption := 'File .sql tidak ditemukan di dalam ZIP.'; Exit; end;

  LblStatus.Caption := 'Merestore database... Harap tunggu.';

  MySQLExe := ExpandConstant('{app}') + '\mysql.exe';
  if not FileExists(MySQLExe) then MySQLExe := GetAppServWww('') + '\..\MySQL\bin\mysql.exe';
  if not FileExists(MySQLExe) then MySQLExe := 'C:\AppServ\MySQL\bin\mysql.exe';

  { Jika password belum diisi, tanya dulu via MsgBox + field di halaman restore }
  if MySQLPassword = '' then
  begin
    LblStatus.Caption := 'Masukkan password MySQL di kolom password lalu klik Restore lagi.';
    Exit;
  end;

  Params := '"' + MySQLExe + '" -u root -p' + MySQLPassword + ' db_kasirlancar < "' + SqlFile + '"';
  PSScriptPath := ExpandConstant('{tmp}') + '\do_restore.bat';
  SaveStringToFile(PSScriptPath, '@echo off' + #13#10 + Params + #13#10, False);
  RC := ExecAndWait(ExpandConstant('{cmd}'), '/C "' + PSScriptPath + '"', TempExtract);

  if RC <> 0 then
  begin
    { Fallback: coba tanpa password }
    Params := '"' + MySQLExe + '" -u root db_kasirlancar < "' + SqlFile + '"';
    SaveStringToFile(PSScriptPath, '@echo off' + #13#10 + Params + #13#10, False);
    RC := ExecAndWait(ExpandConstant('{cmd}'), '/C "' + PSScriptPath + '"', TempExtract);
  end;

  if RC = 0 then
    LblStatus.Caption := 'BERHASIL! Database direstore dari: ' + ExtractFileName(ZipFile)
  else
    LblStatus.Caption := 'GAGAL restore. Pastikan MySQL berjalan dan password sesuai yang diset saat instalasi AppServ.';
end;

{ ================================================================
  VALIDASI FOLDER UPDATE
  Cek apakah folder yang dipilih benar-benar instalasi Kasir Lancar
  ================================================================ }
function ValidateUpdateFolder(Dir: string): Boolean;
var
  Missing : string;
  Found   : Integer;
begin
  Result  := False;
  Missing := '';
  Found   := 0;

  { Cek file-file kunci yang harus ada }
  if FileExists(Dir + '\KasirLancar.exe')   then Inc(Found);
  if FileExists(Dir + '\database.json')     then Inc(Found);
  if FileExists(Dir + '\MySql.Data.dll')    then Inc(Found);
  if FileExists(Dir + '\KasirLancar.exe.config') then Inc(Found);

  if Found = 0 then
    Missing := 'Folder ini tidak mengandung instalasi Kasir Lancar sama sekali.'
  else if Found < 3 then
    Missing := 'Folder ini mungkin bukan instalasi Kasir Lancar yang lengkap.' + #13#10 +
               '(Hanya ' + IntToStr(Found) + ' dari 4 file kunci ditemukan)';

  if Missing <> '' then
  begin
    MsgBox(
      'Verifikasi Folder Gagal' + #13#10 + #13#10 +
      Missing + #13#10 + #13#10 +
      'Folder dipilih: ' + Dir + #13#10 + #13#10 +
      'Pastikan Anda memilih folder tempat Kasir Lancar sebelumnya diinstal.' + #13#10 +
      'Contoh: D:\Kasir Lancar',
      mbError, MB_OK);
    Exit;
  end;

  Result := True;
end;

{ ================================================================
  INITIALIZE WIZARD
  ================================================================ }
procedure InitializeWizard();
var
  BtnW, BtnH, BtnTop, TotalW : Integer;
begin
  InstallMode   := 'server';
  MySQLPassword := '';

  with WizardForm do
  begin
    Color := $00FFFFFF;

    PageNameLabel.Font.Name  := 'Segoe UI';
    PageNameLabel.Font.Size  := 12;
    PageNameLabel.Font.Style := [fsBold];
    PageNameLabel.Font.Color := $00321400;

    PageDescriptionLabel.Font.Name  := 'Segoe UI';
    PageDescriptionLabel.Font.Size  := 9;
    PageDescriptionLabel.Font.Color := $00665544;

    MainPanel.Color := $00F5F2EE;

    BtnW   := 90;
    BtnH   := 28;
    BtnTop := ClientHeight - BtnH - 12;
    TotalW := ClientWidth;

    CancelButton.Width   := BtnW;
    CancelButton.Height  := BtnH;
    CancelButton.Left    := 12;
    CancelButton.Top     := BtnTop;
    CancelButton.Caption := 'Batal';
    CancelButton.Font.Name := 'Segoe UI';
    CancelButton.Font.Size := 9;

    BackButton.Width   := BtnW;
    BackButton.Height  := BtnH;
    BackButton.Left    := TotalW - (BtnW * 2) - 18;
    BackButton.Top     := BtnTop;
    BackButton.Caption := '< Kembali';
    BackButton.Font.Name := 'Segoe UI';
    BackButton.Font.Size := 9;

    NextButton.Width   := BtnW;
    NextButton.Height  := BtnH;
    NextButton.Left    := TotalW - BtnW - 12;
    NextButton.Top     := BtnTop;
    NextButton.Caption := 'Lanjut >';
    NextButton.Font.Name  := 'Segoe UI';
    NextButton.Font.Size  := 9;
    NextButton.Font.Style := [fsBold];
  end;

  CreateModePage();
  CreateRestorePage();

  { ── Perbaiki posisi label status di halaman Installing ──────────
    StatusLabel menimpa teks judul — geser ke bawah agar tidak overlap }
  WizardForm.StatusLabel.Top   := WizardForm.StatusLabel.Top + 18;
  WizardForm.FilenameLabel.Top := WizardForm.FilenameLabel.Top + 18;
end;

{ ================================================================
  KONTROL VISIBILITAS HALAMAN BERDASARKAN MODE
  ================================================================ }
function ShouldSkipPage(PageID: Integer): Boolean;
begin
  Result := False;

  { Halaman restore database — hanya untuk mode server }
  if PageID = PageRestore.ID then
  begin
    Result := (InstallMode <> 'server');
    Exit;
  end;

  { Mode update: skip komponen & tasks }
  if InstallMode = 'update' then
  begin
    if PageID = wpSelectComponents then Result := True;
    if PageID = wpSelectTasks      then Result := True;
  end;
end;

{ ================================================================
  SEMBUNYIKAN/TAMPILKAN HALAMAN BERDASARKAN MODE
  ================================================================ }
procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpSelectDir then
  begin
    { Tidak ada aksi khusus }
  end;
end;

{ ================================================================
  DETEKSI DRIVE D:
  ================================================================ }
function GetDefaultInstallDir(Param: string): string;
begin
  if DirExists('D:\') then Result := 'D:\Kasir Lancar'
  else Result := 'C:\Kasir Lancar';
end;

{ ================================================================
  TULIS config.php API dengan password MySQL dari user
  ================================================================ }
procedure TulisConfigAPI(Password: string);
var
  WwwPath, ConfigPath, Content : string;
begin
  WwwPath    := GetAppServWww('');
  ConfigPath := WwwPath + '\api\config.php';

  { Tunggu sampai file ada (AppServ baru saja diinstal) }
  if not FileExists(ConfigPath) then Exit;

  Content :=
    '<?php' + #13#10 +
    '// Blokir akses langsung ke file ini via browser' + #13#10 +
    'if (basename($_SERVER[''SCRIPT_FILENAME'']) === basename(__FILE__)) {' + #13#10 +
    '    http_response_code(403);' + #13#10 +
    '    exit(''Forbidden'');' + #13#10 +
    '}' + #13#10 +
    '' + #13#10 +
    'return [' + #13#10 +
    '    ''host''     => ''localhost'',' + #13#10 +
    '    ''db_name''  => ''db_kasirlancar'',' + #13#10 +
    '    ''username'' => ''root'',' + #13#10 +
    '    ''password'' => ''' + Password + ''',' + #13#10 +
    '    ''port''     => 3306,' + #13#10 +
    '    ''charset''  => ''utf8mb4'',' + #13#10 +
    '];' + #13#10;

  SaveStringToFile(ConfigPath, Content, False);
end;

{ ================================================================
  TULIS database.json dengan password MySQL dari user
  ================================================================ }
procedure TulisDatabaseJson(InstDir, Password: string);
var
  JsonPath, Content : string;
begin
  JsonPath := InstDir + '\database.json';

  { Selalu tulis ulang untuk mode server — isi password yang benar }
  { Untuk client, file sudah dikopi dengan onlyifdoesntexist, tidak perlu tulis ulang }
  Content :=
    '{' + #13#10 +
    '  "Server": "localhost",' + #13#10 +
    '  "Port": "3306",' + #13#10 +
    '  "User": "root",' + #13#10 +
    '  "Password": "' + Password + '",' + #13#10 +
    '  "Database": "db_kasirlancar"' + #13#10 +
    '}' + #13#10;

  SaveStringToFile(JsonPath, Content, False);
end;

{ ================================================================
  FORM INPUT PASSWORD MYSQL
  Menampilkan form dengan dua field password + konfirmasi
  Return: password yang diinput, atau '' jika user skip
  ================================================================ }
function TanyaPasswordMySQL(): string;
var
  Frm        : TForm;
  LblInfo    : TLabel;
  LblP1      : TLabel;
  EdtP1      : TPasswordEdit;
  LblP2      : TLabel;
  EdtP2      : TPasswordEdit;
  LblWarn    : TLabel;
  BtnOK      : TButton;
  BtnSkip    : TButton;
begin
  Result := '';

  Frm := TForm.Create(nil);
  try
    Frm.Caption  := 'Konfigurasi Password MySQL';
    Frm.Width    := 420;
    Frm.Height   := 260;
    Frm.Position := poScreenCenter;
    Frm.BorderStyle := bsDialog;

    LblInfo := TLabel.Create(Frm);
    LblInfo.Parent   := Frm;
    LblInfo.Left     := 16;
    LblInfo.Top      := 14;
    LblInfo.Width    := 388;
    LblInfo.Height   := 36;
    LblInfo.AutoSize := False;
    LblInfo.WordWrap := True;
    LblInfo.Caption  := 'Masukkan password MySQL root yang baru saja Anda set di wizard AppServ:';

    LblP1 := TLabel.Create(Frm);
    LblP1.Parent  := Frm;
    LblP1.Left    := 16;
    LblP1.Top     := 60;
    LblP1.Caption := 'Password MySQL root:';
    LblP1.AutoSize := True;

    EdtP1 := TPasswordEdit.Create(Frm);
    EdtP1.Parent := Frm;
    EdtP1.Left   := 16;
    EdtP1.Top    := 78;
    EdtP1.Width  := 388;
    EdtP1.Height := 23;

    LblP2 := TLabel.Create(Frm);
    LblP2.Parent  := Frm;
    LblP2.Left    := 16;
    LblP2.Top     := 112;
    LblP2.Caption := 'Konfirmasi password:';
    LblP2.AutoSize := True;

    EdtP2 := TPasswordEdit.Create(Frm);
    EdtP2.Parent := Frm;
    EdtP2.Left   := 16;
    EdtP2.Top    := 130;
    EdtP2.Width  := 388;
    EdtP2.Height := 23;

    LblWarn := TLabel.Create(Frm);
    LblWarn.Parent    := Frm;
    LblWarn.Left      := 16;
    LblWarn.Top       := 162;
    LblWarn.Width     := 388;
    LblWarn.AutoSize  := False;
    LblWarn.WordWrap  := True;
    LblWarn.Caption   := '';
    LblWarn.Font.Color := clRed;

    BtnOK := TButton.Create(Frm);
    BtnOK.Parent      := Frm;
    BtnOK.Caption     := 'Simpan';
    BtnOK.Left        := 220;
    BtnOK.Top         := 192;
    BtnOK.Width       := 88;
    BtnOK.Height      := 28;
    BtnOK.ModalResult := mrOk;

    BtnSkip := TButton.Create(Frm);
    BtnSkip.Parent      := Frm;
    BtnSkip.Caption     := 'Lewati';
    BtnSkip.Left        := 316;
    BtnSkip.Top         := 192;
    BtnSkip.Width       := 88;
    BtnSkip.Height      := 28;
    BtnSkip.ModalResult := mrCancel;

    { Loop validasi }
    while True do
    begin
      if Frm.ShowModal() = mrCancel then
      begin
        Result := '';
        Break;
      end;
      if EdtP1.Text = '' then
      begin
        LblWarn.Caption := 'Password tidak boleh kosong.';
        Continue;
      end;
      if EdtP1.Text <> EdtP2.Text then
      begin
        LblWarn.Caption := 'Password dan konfirmasi tidak cocok.';
        EdtP2.Text := '';
        Continue;
      end;
      Result := EdtP1.Text;
      Break;
    end;
  finally
    Frm.Free();
  end;
end;
function NextButtonClick(CurPageID: Integer): Boolean;
var
  InstDir: string;
begin
  Result := True;

  { Halaman pilih mode: simpan pilihan }
  if CurPageID = PageMode.ID then
  begin
    if RdoServer.Checked then InstallMode := 'server'
    else if RdoClient.Checked then InstallMode := 'client'
    else InstallMode := 'update';
    Exit;
  end;

  { Halaman pilih folder }
  if CurPageID = wpSelectDir then
  begin
    InstDir := WizardDirValue;

    if Length(InstDir) <= 3 then
    begin
      MsgBox('Folder instalasi tidak boleh di root drive.' + #13#10 +
             'Contoh: D:\Kasir Lancar', mbError, MB_OK);
      Result := False;
      Exit;
    end;

    { Mode Update: validasi isi folder }
    if InstallMode = 'update' then
    begin
      if not ValidateUpdateFolder(InstDir) then
      begin
        Result := False;
        Exit;
      end;
      if MsgBox(
        'Folder terverifikasi sebagai instalasi Kasir Lancar.' + #13#10 + #13#10 +
        'Update akan mengganti file aplikasi di:' + #13#10 +
        InstDir + #13#10 + #13#10 +
        'Data, konfigurasi, dan database TIDAK akan dihapus.' + #13#10 +
        'Lanjutkan update?',
        mbConfirmation, MB_YESNO) = IDNO then
      begin
        Result := False;
        Exit;
      end;
    end;
  end;

  { Halaman restore database }
  if CurPageID = PageRestore.ID then
  begin
    if not ChkSkipRestore.Checked then
    begin
      if EdtZipPath.Text = '' then
      begin
        if MsgBox('Belum ada file backup dipilih.' + #13#10 +
                  'Lanjutkan tanpa restore database?',
                  mbConfirmation, MB_YESNO) = IDNO then
        begin
          Result := False;
          Exit;
        end;
      end
      else
        DoRestoreDatabase();
    end;
  end;
end;

{ ================================================================
  SETELAH INSTALL SELESAI — tanya password MySQL lalu tulis config
  ================================================================ }
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    if InstallMode = 'server' then
    begin
      { Tanya password MySQL yang sudah diset user di wizard AppServ }
      MsgBox(
        'AppServ dan semua komponen sudah terpasang.' + #13#10 + #13#10 +
        'Masukkan password MySQL root yang baru saja Anda set' + #13#10 +
        'di wizard AppServ. Password ini akan disimpan ke' + #13#10 +
        'konfigurasi aplikasi (database.json) dan API (config.php).',
        mbInformation, MB_OK);

      MySQLPassword := TanyaPasswordMySQL();

      if MySQLPassword <> '' then
      begin
        TulisConfigAPI(MySQLPassword);
        TulisDatabaseJson(WizardDirValue, MySQLPassword);
        MsgBox('Konfigurasi berhasil disimpan.', mbInformation, MB_OK);
      end
      else
        MsgBox('Password dilewati. Edit database.json dan config.php manual jika diperlukan.',
               mbInformation, MB_OK);
    end
    else if InstallMode = 'client' then
    begin
      { database.json untuk client dikopi dari installer dengan onlyifdoesntexist }
      { User mengisi IP server lewat SettingDatabase di dalam aplikasi }
      MsgBox(
        'Instalasi Client selesai!' + #13#10 + #13#10 +
        'Langkah selanjutnya:' + #13#10 +
        '  1. Jalankan Kasir Lancar' + #13#10 +
        '  2. Buka menu Pengaturan → Setting Database' + #13#10 +
        '  3. Isi IP Address komputer server, lalu klik Tes Koneksi',
        mbInformation, MB_OK);
    end;

    { ── Tidak ada cek WebView2 ── }
  end;
end;

{ ================================================================
  KONFIRMASI UNINSTALL
  ================================================================ }
function InitializeUninstall(): Boolean;
begin
  Result := MsgBox(
    'Apakah Anda yakin ingin menghapus Kasir Lancar?' + #13#10 +
    'Data dan konfigurasi tidak akan dihapus.',
    mbConfirmation, MB_YESNO) = IDYES;
end;
