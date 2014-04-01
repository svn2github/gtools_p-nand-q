!include "MUI2.nsh"
SetCompressor /SOLID lzma

Name "GTools 4.1"

OutFile "gtools-4.1.exe"

RequestExecutionLevel admin ;Require admin rights on NT6+ (When UAC is turned on)

;InstallDir $DESKTOP\IFDF
InstallDir "$PROGRAMFILES\p-nand-q.com\GTools"
InstallDirRegKey HKLM "SOFTWARE\Gerson Kurz\GTools" "Install_Dir"

  RequestExecutionLevel user
  
  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_DIRECTORY
  ;!insertmacro MUI_PAGE_COMPONENTS  
  !insertmacro MUI_PAGE_INSTFILES
     
    !define MUI_FINISHPAGE_NOAUTOCLOSE
    !define MUI_FINISHPAGE_RUN
    !define MUI_FINISHPAGE_RUN_NOTCHECKED
    !define MUI_FINISHPAGE_RUN_TEXT "start pserv4"
    !define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"

  !insertmacro MUI_PAGE_FINISH

  !insertmacro MUI_LANGUAGE "English"

Section "GTools (required)"

    SectionIn RO

    ; Set output path to the installation directory.
    SetOutPath "$INSTDIR"

    ; Put file there
    File /R "..\BIN\release\*"
    File /R "..\..\regdiff\BIN\release\*"

    ; Write the installation path into the registry
    WriteRegStr HKLM "Software\Gerson Kurz\GTools" "Install_Dir" "$INSTDIR"

    CreateDirectory "$SMPROGRAMS\GTools"
    CreateShortCut "$SMPROGRAMS\GTools\Gkalk.lnk" "$INSTDIR\Gkalk.exe" "" "$INSTDIR\Gkalk.exe" 0
    CreateShortCut "$SMPROGRAMS\GTools\pserv3.lnk" "$INSTDIR\pserv4.exe" "" "$INSTDIR\pserv4.exe" 0
    CreateShortCut "$SMPROGRAMS\GTools\dllusage.lnk" "$INSTDIR\dllusage.exe" "" "$INSTDIR\dllusage.exe" 0
      
SectionEnd

Function LaunchLink
  ExecShell "" "$INSTDIR\pserv4.exe"
FunctionEnd


