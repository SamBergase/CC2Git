:: ---------------------------------------------------------------------------
:: Installation script for the ClearCase2Git tool.
::
:: The script will install necessary files in the installation directory.
:: 
:: Rev  Date        Name        Description
:: 1    2025-02-118 S Bergåse	Created the script.
:: ---------------------------------------------------------------------------
@echo off

rem set KEY=HKEY_LOCAL_MACHINE\SOFTWARE\Telelogic\DOORS\9.7\
rem set KEYVAL=InstallationDirectory

set "CC2Git_Version=0.1.4"

if "%USERNAME%" == "esbberg" (
    set "CC2GIT_FOLDER=C:\Users\%USERNAME%\source\releases\ClearCase2Git\v%CC2Git_Version%\"
    mkdir %CC2GIT_FOLDER%
	echo "Copying operation files to "%CC2GIT_FOLDER%
	xcopy /i /e %USERPROFILE%\source\repos\cc2git\ClearCase2Git\bin\Release\ %CC2GIT_FOLDER%
	echo "Copying resource files to "%CC2GIT_FOLDER%
	xcopy /i /e %USERPROFILE%\source\repos\cc2git\ClearCase2Git\Resources\ %CC2GIT_FOLDER%
    echo "Copying batch script for installation (this one)"
	xcopy %USERPROFILE%\source\repos\cc2git\install.bat %CC2GIT_FOLDER%
    echo "Packaging copied files"
	rem for %f in %CC2GIT_FOLDER% do zip -r "cc2git_014.zip"
    
	goto:eof
)
if "%USERNAME%" != "esbberg" (
	set "CC2GIT_FOLDER=C:\Program Files\ClearCase2Git\v%CC2Git_Version%\"
	mkdir %CC2GIT_FOLDER%
	set "CC2GIT_SOURCE=C:\Users\%USERNAME%\source\releases\ClearCase2Git\v%CC2Git_Version%\"
	xcopy /i /e %CC2GIT_SOURCE% %CC2GIT_FOLDER%
	mklink "%USERPROFILE%\Desktop" "%CC2GIT_FOLDER%\ClearCase2Git.exe"
	IconFile="%CC2GIT_FOLDER%\cc2gitLogoBlack.ico" >> "%CC2GIT_FOLDER%\ClearCase2Git.exe"
	IconIndex=0 >> "%CC2GIT_FOLDER%\ClearCase2Git.exe"
	mklink "%USERPROFILE%\Start Menu\Programs" "%CC2GIT_FOLDER%\ClearCase2Git.exe"

	goto:eof
)

rem zipjs.bat unzip -source %CD%\cc2git.zip -destination %CC2GIT_FOLDER%
rem	mklink %USERPROFILE%\Desktop "%CC2GIT_FOLDER%\ClearCase2Git.exe"
rem	set objFolder = %USERPROFILE%\Desktop\
rem	set objFolderItem = objFolder.ParseName("ClearCase2Git.lnk")
rem	set ObjShortcut = objFolderItem.GetLink
rem	ObjShortcut.SetIconLocation "%CC2GIT_FOLDER%\cc2gitLogoBlack.ico"
rem	ObjShortcut.Save

