#include "stdafx.h"
#include <windows.h>
#include <ce_setup.h>

codeINSTALL_INIT Install_Init(
    HWND        hwndParent,
    BOOL        fFirstCall, 
    BOOL        fPreviouslyInstalled,
    LPCTSTR     pszInstallDir
    )
{
    return codeINSTALL_INIT_CONTINUE;
}

codeINSTALL_EXIT Install_Exit(
    HWND    hwndParent,
    LPCTSTR pszInstallDir,
    WORD    cFailedDirs,
    WORD    cFailedFiles,
    WORD    cFailedRegKeys,
    WORD    cFailedRegVals,
    WORD    cFailedShortcuts
    )
{
	PROCESS_INFORMATION pi      = {0};
    codeINSTALL_EXIT    cie     = codeINSTALL_EXIT_DONE;

	// We are provided with the installation folder the
	// user has installed the application into. So prepend
	// the name of the application we want to launch.
	TCHAR szPath[MAX_PATH];
	_tcscpy(szPath, pszInstallDir);
	_tcscat(szPath, _T("\\"));
	_tcscat(szPath, _T("PockeTwit.exe"));

	// Start the application, and don't wait for it to exit
    if (!CreateProcess(szPath, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, &pi))
	{
		MessageBox(GetForegroundWindow(), szPath, L"failed", MB_OK);
	    cie = codeINSTALL_EXIT_UNINSTALL;
	}

	return cie;
}



codeUNINSTALL_INIT Uninstall_Init(
    HWND        hwndParent,
    LPCTSTR     pszInstallDir
    )
{
	
    return codeUNINSTALL_INIT_CONTINUE;
}



codeUNINSTALL_EXIT Uninstall_Exit(
    HWND    hwndParent
    )
{
    return codeUNINSTALL_EXIT_DONE;
}

