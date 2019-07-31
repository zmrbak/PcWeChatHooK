// dllmain.cpp : 定义 DLL 应用程序的入口点。
#pragma region 头文件
#include "pch.h"
#include "resource.h"
#include "shellapi.h"
#include <string>
#include <tchar.h> 
#include <sstream>
#include <string>
#include <iomanip>
#include <strstream>
#include <list>
#include <iostream>
#include <map>
#include <tuple>
#include <CommCtrl.h>
#include <fstream>
#include <iomanip>
#include <commdlg.h>  
#pragma comment(lib, "Version.lib")
#pragma endregion

using namespace std;

#pragma region 声明函数
INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam);
VOID	ShowDemoUI(HMODULE hModule);
VOID	HookWx();
VOID	UnHookWx();
VOID	InsertUserList();
VOID	RecieveUserInfoHook();
string	Dec2Hex(DWORD i);
LPCWSTR String2LPCWSTR(string text);
LPCWSTR GetMsgByAddress(DWORD memAddress);
BOOL	IsWxVersionValid();
wstring String2Wstring(string str);
VOID	UnInject();
VOID	InitListContrl();
VOID	ListContrlSetItems();
VOID	CheckLoginFinished();
VOID	SaveUserToTxtFie();
string	Wstring2String(const wstring& wstr);
VOID	SendImageFile(wstring wxId, wstring fileName);
#pragma endregion

#pragma region 定义变量
DWORD wxBaseAddress = 0;
//HOOK标志
BOOL isWxHooked = FALSE;
//对话框句柄
HWND hWinDlg;
//跳回地址
DWORD jumBackAddress = 0;
//我们要提取的寄存器内容
DWORD r_esi = 0;
//此HOOK匹配的微信版本
const string wxVersoin = "2.6.7.57";
//我自己的微信ID
string myWxId = "";
HWND hListView = NULL;
DWORD overWritedCallAdd = 0;
DWORD hookAddress = 0;
BOOL isCheckLoginRunning = FALSE;
class TEXT_WX
{
public:
	wchar_t* pWxid = nullptr;
	DWORD length = 0;
	DWORD maxLength = 0;
	DWORD fill1 = 0;
	DWORD fill2 = 0;
	wchar_t wxid[1024] = { 0 };

	TEXT_WX(wstring wsWxid)
	{
		const wchar_t* temp = wsWxid.c_str();
		wmemcpy(wxid, temp, wsWxid.length());
		length = wsWxid.length();
		maxLength = wsWxid.capacity();
		fill1 = 0;
		fill2 = 0;
		pWxid = wxid;
	}
};
//HOOK前的代码
CHAR originalCode[5] = { 0 };
typedef tuple <
	//wxid1
	wstring,
	//wxName
	wstring,
	//v1
	wstring,
	//nickName
	wstring
> USER_INFO;
//定义用户列表
list<USER_INFO> userInfoList(1);
wstring selectedWxid = L"";
#pragma endregion

#pragma region 使用VS Detours调试，必须有一个"用不到的"导出函数
VOID __declspec(dllexport) Test()
{
	//OutputDebugString(TEXT("开始调试"));
}
#pragma endregion

//DLL入口函数
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		//##########################################################
		//
		//注意：仅适配PC微信2.6.7.57版本，其它版本不可用
		//
		//##########################################################

		HANDLE hANDLE = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)ShowDemoUI, hModule, NULL, 0);
		if (hANDLE != 0)
		{
			CloseHandle(hANDLE);
		}
		break;
	}
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

//显示操作的窗口
VOID ShowDemoUI(HMODULE hModule)
{
	//获取WeChatWin.dll的基址
	while (wxBaseAddress == 0)
	{
		wxBaseAddress = (DWORD)GetModuleHandle(TEXT("WeChatWin.dll"));
		Sleep(100);
	}

	DialogBox(hModule, MAKEINTRESOURCE(IDD_MAIN), NULL, &DialogProc);
}

//窗口回调函数，处理窗口事件
INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam)
{
	hWinDlg = hwndDlg;
	switch (uMsg)
	{
	case WM_INITDIALOG:
	{
		//检查微信版本
		if (IsWxVersionValid() == FALSE)
		{
			string msg = "微信版本不匹配\n";
			msg.append("请使用PC微信");
			msg.append(wxVersoin);
			MessageBox(NULL, String2LPCWSTR(msg), TEXT("错误"), MB_OK | MB_ICONERROR);

			EndDialog(hwndDlg, 0);
			UnInject();
			break;
		}

		//在登录微信前注入
		//判断微信是否登录
		int* loginStatus = (int*)(wxBaseAddress + 0x125D56C);
		if (*loginStatus != 0)
		{
			MessageBox(NULL, TEXT("请在登录微信前启用此功能"), TEXT("错误"), MB_OK | MB_ICONERROR);
			EndDialog(hwndDlg, 0);
			UnInject();
			break;
		}

		InitListContrl();

		//DLL加载后，就启动Inline HOOK
		HookWx();
		break;
	}
	case WM_CLOSE:
		//关闭窗口事件
		EndDialog(hwndDlg, 0);
		break;

	case WM_NOTIFY:
	{
		NMHDR* pHDR = (NMHDR*)lParam;
		if ((pHDR->code == NM_CLICK) && (pHDR->idFrom == IDC_LIST))
		{
			LPNMLISTVIEW	pnmv = (LPNMLISTVIEW)lParam;

			//根据索引pnmv->iItem获取文本 
			LVITEM  lv;
			TCHAR  buf[64] = { 0 };
			lv.iSubItem = 1;
			lv.pszText = buf; //向系统提供写入内存
			lv.cchTextMax = sizeof(buf);//buf的长度
			SendMessage(pHDR->hwndFrom, LVM_GETITEMTEXT, pnmv->iItem, (LPARAM)& lv);
			selectedWxid = buf;
		}
		break;
	}
	case WM_COMMAND:
		//保存到文件
		if (wParam == IDC_SAVE)
		{
			SaveUserToTxtFie();
			break;
		}

		//IDC_REFRESH
		if (wParam == IDC_REFRESH)
		{
			ListContrlSetItems();
			break;
		}

		//打开视频帮助页面
		if (wParam == IDC_163CLASS)
		{
			ShellExecute(hwndDlg, TEXT("open"), TEXT("http://t.cn/EXUbebQ"), NULL, NULL, NULL);
			break;
		}

		//GitHub
		if (wParam == IDC_GITHUB)
		{
			ShellExecute(hwndDlg, TEXT("open"), TEXT("https://github.com/zmrbak/PcWeChatHooK"), NULL, NULL, NULL);
			break;
		}

		//重启微信
		if (wParam == IDC_WX_REBOOT)
		{
			//获取微信程序路径
			TCHAR szAppName[MAX_PATH];
			GetModuleFileName(NULL, szAppName, MAX_PATH);

			//启动新进程
			STARTUPINFO StartInfo;
			ZeroMemory(&StartInfo, sizeof(StartInfo));
			StartInfo.cb = sizeof(StartInfo);

			PROCESS_INFORMATION procStruct;
			ZeroMemory(&procStruct, sizeof(procStruct));
			StartInfo.cb = sizeof(STARTUPINFO);

			if (CreateProcess((LPCTSTR)szAppName, NULL, NULL, NULL, FALSE, NORMAL_PRIORITY_CLASS, NULL, NULL, &StartInfo, &procStruct))
			{
				CloseHandle(procStruct.hProcess);
				CloseHandle(procStruct.hThread);
			}

			//终止当前进程
			TerminateProcess(GetCurrentProcess(), 0);
			break;
		}

		//选择文件
		if (wParam == IDC_SELECT)
		{
			// 公共对话框结构。   
			OPENFILENAME ofn;
			// 保存获取文件名称的缓冲区。        
			TCHAR szFile[MAX_PATH];
			// 初始化选择文件对话框。     
			ZeroMemory(&ofn, sizeof(OPENFILENAME));
			ofn.lStructSize = sizeof(OPENFILENAME);
			ofn.hwndOwner = NULL;
			ofn.lpstrFile = szFile;
			ofn.lpstrFile[0] = '\0';
			ofn.nMaxFile = sizeof(szFile);
			ofn.lpstrFilter = L"All(*.*)\0*.*\0Text(*.txt)\0*.TXT\0\0";
			ofn.nFilterIndex = 1;
			ofn.lpstrFileTitle = NULL;
			ofn.nMaxFileTitle = 0;
			ofn.lpstrInitialDir = NULL;
			ofn.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;
			// 显示打开选择文件对话框。
			if (GetOpenFileName(&ofn))
			{
				HWND hFileHelper = GetDlgItem(hwndDlg, IDC_FILENAME);
				SetWindowText(hFileHelper, szFile);
			}
		}

		if (wParam == IDC_SEND)
		{
			//要发送的文件
			TCHAR szFile[MAX_PATH];
			HWND hFileHelper = GetDlgItem(hwndDlg, IDC_FILENAME);
			GetWindowText(hFileHelper, szFile, MAX_PATH);
			wstring fileName = szFile;
			if (fileName.length() == 0)
			{
				MessageBox(NULL, L"请先选择要发送的文件", L"错误", MB_OK | MB_ICONERROR);
				return FALSE;
			}

			if (selectedWxid.length() == 0)
			{
				MessageBox(NULL, L"请先选择接收文件的联系人", L"错误", MB_OK | MB_ICONERROR);
				return FALSE;
			}

			SendImageFile(selectedWxid, fileName);
		}
		break;
	default:
		break;
	}
	return FALSE;
}

//发送文件
VOID SendImageFile(wstring weiXinId, wstring weiXinFile)
{
	//微信ID
	TEXT_WX wxId(weiXinId);
	//被发送的文件
	TEXT_WX wxFileName(weiXinFile);
	//缓冲区
	CHAR buffer[0x45C] = { 0 };

	DWORD call_1 = wxBaseAddress + 0x7FD40;
	DWORD call_2 = wxBaseAddress + 0x2E4630;
	//DWORD call_3 = wxBaseAddress + 0x4DC80;

	//0F489CB4  |> \53            |push ebx                          ;  fileName; Case 2 of switch 0F489B08
	//0F489CB5  |.  FF75 CC       |push [local.13]                   ;  wxid
	//0F489CB8  |.  8D85 A4FBFFFF |lea eax,[local.279]               ;  buffer [ebp-0x45C]
	//0F489CBE  |.  50            |push eax
	//0F489CBF  |.  51            |push ecx                          ;  0
	//0F489CC0  |.  E8 7B60FBFF   |call WeChatWi.0F43FD40            ;  0x7FD40，发送图片1
	//0F489CC5  |.  83C4 04       |add esp,0x4
	//0F489CC8  |.  8BC8          |mov ecx,eax
	//0F489CCA  |.  E8 61A92100   |call WeChatWi.0F6A4630

	////fileName; 
	//WeChatWin.dll+C9CB4 - 53                    - push ebx
	////wxid
	//WeChatWin.dll+C9CB5 - FF 75 CC              - push [ebp-34]
	////buffer 
	//WeChatWin.dll+C9CB8 - 8D 85 A4FBFFFF        - lea eax,[ebp-0000045C]
	//WeChatWin.dll+C9CBE - 50                    - push eax
	////0
	//WeChatWin.dll+C9CBF - 51                    - push ecx
	////7FD40
	//WeChatWin.dll+C9CC0 - E8 7B60FBFF           - call WeChatWin.dll+7FD40
	//WeChatWin.dll+C9CC5 - 83 C4 04              - add esp,04 { 4 }
	//WeChatWin.dll+C9CC8 - 8B C8                 - mov ecx,eax
	////2E4630
	//WeChatWin.dll+C9CCA - E8 61A92100           - call WeChatWin.dll+2E4630
	//WeChatWin.dll+C9CCF - C7 45 FC 07000000     - mov [ebp-04],00000007 { 7 }

	DWORD myESP = 0;

	__asm
	{
		mov myESP, esp

		//filePath
		//WeChatWin.dll+C9CB4 - 53                    - push ebx
		lea ebx, wxFileName
		push ebx

		//wxid
		//WeChatWin.dll+C9CB5 - FF 75 CC              - push [ebp-34]
		lea eax, wxId
		push eax

		//buffer
		//WeChatWin.dll+C9CB8 - 8D 85 A4FBFFFF        - lea eax,[ebp-0000045C]
		//WeChatWin.dll+C9CBE - 50                    - push eax
		lea eax, buffer
		push eax

		//0x0
		//WeChatWin.dll+C9CBF - 51                    - push ecx
		push 0x0

		//call 
		//WeChatWin.dll+C9CC0 - E8 7B60FBFF           - call WeChatWin.dll+7FD40
		call call_1

		//WeChatWin.dll+C9CC5 - 83 C4 04              - add esp,04 { 4 }
		add esp, 0x4

		//WeChatWin.dll+C9CC8 - 8B C8                 - mov ecx,eax
		mov ecx, eax

		//WeChatWin.dll+C9CCA - E8 61A92100           - call WeChatWin.dll+2E4630
		call call_2

		//释放
		/*lea ecx,buffer
		call call_3*/

		//##########################################################################
		//恢复堆栈
		mov eax, myESP
		mov esp, eax
		//##########################################################################
	}
}


//Inline Hook接收消息
VOID HookWx()
{
	//判断是否已经HOOK
	if (isWxHooked == FALSE)
	{
		isWxHooked = TRUE;

		//启动一个线程用来更新ListView
		HANDLE hANDLE = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)CheckLoginFinished, NULL, NULL, 0);
		if (hANDLE != 0)
		{
			CloseHandle(hANDLE);
		}

		//WeChatWin.dll+175FA4 - 0F85 6B020000         - jne WeChatWin.dll+176215
		//WeChatWin.dll+175FAA - E8 0186EDFF           - call WeChatWin.dll+4E5B0
		//WeChatWin.dll+175FAF - 53                    - push ebx
		//#######################################################################
		//下面5个字节，进行HOOK
		////HOOK
		//WeChatWin.dll+175FB0 - E8 4BA60F00           - call WeChatWin.dll+270600
		//#######################################################################
		//WeChatWin.dll+175FB5 - 83 F8 03              - cmp eax,03 { 3 }
		//WeChatWin.dll+175FB8 - 0F85 43010000         - jne WeChatWin.dll+176101
		//WeChatWin.dll+175FBE - 8D 8D 28FFFFFF        - lea ecx,[ebp-000000D8]

		hookAddress = wxBaseAddress + 0x175FB0;
		string debugMsg = "Hook的地址：\t";
		debugMsg.append(Dec2Hex(hookAddress));
		OutputDebugString(String2LPCWSTR(debugMsg));

		//被覆盖的CALL目标
		overWritedCallAdd = wxBaseAddress + 0x270600;

		//跳回的地址
		jumBackAddress = hookAddress + 5;

		//组装跳转数据
		BYTE jmpCode[5] = { 0 };
		jmpCode[0] = 0xE9;

		//新跳转指令中的数据=跳转的地址-原地址（HOOK的地址）-跳转指令的长度
		*(DWORD*)& jmpCode[1] = (DWORD)RecieveUserInfoHook - hookAddress - 5;

		//保存当前位置的指令,在unhook的时候使用。
		ReadProcessMemory(GetCurrentProcess(), (LPVOID)hookAddress, originalCode, 5, 0);

		//覆盖指令
		//WeChatWin.dll + 310573 - B9 E8CF2B10 - mov ecx, WeChatWin.dll + 125CFE8{ (0) }
		WriteProcessMemory(GetCurrentProcess(), (LPVOID)hookAddress, jmpCode, 5, 0);
	}
}
//UnHook接收消息
VOID UnHookWx()
{
	if (isWxHooked == TRUE)
	{
		//恢复指令
		WriteProcessMemory(GetCurrentProcess(), (LPVOID)hookAddress, originalCode, 5, 0);

		isWxHooked = FALSE;
	}
}
//Inline Hook 跳转到这里，读取内存信息
__declspec(naked) VOID RecieveUserInfoHook()
{
	//#######################################################################
	//下面5个字节，进行HOOK
	////HOOK
	//WeChatWin.dll+175FB0 - E8 4BA60F00           - call WeChatWin.dll+270600
	//#######################################################################


	//保存现场
	__asm
	{
		//提取eax寄存器内容，放在一个变量中
		mov r_esi, esi

		//保存寄存器
		pushad
		pushf
	}

	//调用接收消息的函数
	InsertUserList();

	//恢复现场
	__asm
	{
		popf
		popad

		//补充被覆盖的代码
		call overWritedCallAdd

		//跳回被HOOK指令的下一条指令
		jmp jumBackAddress
	}
}
//获取登录的微信信息，将其保存在内存中
VOID InsertUserList()
{
	//ESI
	//群信息
	//$+8      >0E6663E8  UNICODE "10048068352@chatroom"
	//$+C      >00000014
	//$+10     >00000020
	//$+30     >0E754838  UNICODE "v1_861e5d0c5f981fd285a767a14d77ef0f84c9165df4b1f2a"
	//$+34     >0000006C
	//$+38     >00000080
	//$+48     >00000003
	//$+64     >0E78AB98  UNICODE "富足人生分享02"
	//$+68     >00000008
	//$+6C     >00000008
	//$+A0     >00000001
	//$+A4     >0E78AC18  UNICODE "FZRSFX02"
	//$+A8     >00000008
	//$+AC     >00000008
	//$+B8     >0E665CB8  UNICODE "fuzurenshengfenxiang02"
	//$+BC     >00000016
	//$+C0     >00000020
	//$+130    >0000000F
	//$+13C    >00000001
	//$+140    >00000001
	//$+148    >0E74E548
	//$+14C    >00000176

	//个人微信号信息
	//$+8      >0E715000  UNICODE "mu082921"
	//$+C      >00000008
	//$+10     >00000008
	//$+1C > 0E25E4A8  UNICODE "wz028a"
	//$+30     >0E754DB0  UNICODE "v1_299504fbcb8f9b4dd6bcd047a27e18592784624319e92ad"
	//$+34     >0000004C
	//$+38     >00000080
	//$+64     >0E253F30  UNICODE "青龙·嘉禾园微时代【招代理】"
	//$+68     >0000000E
	//$+6C     >00000010
	//$+A0     >00000001
	//$+A4     >0E253FC0  UNICODE "QLJHYWSDZDL"
	//$+A8     >0000000B
	//$+AC     >00000010
	//$+B8     >0E64A558  UNICODE "qinglongjiaheyuanweishidaizhaodaili"
	//$+BC     >00000023
	//$+C0     >00000040
	//$+130    >0000000F
	//$+13C    >00000001
	//$+140    >00000001
	//$+148    >047DB360
	//$+14C    >000002AA


	//公众号？
	//$ ==>    >00001000
	//$+8      >035FF3B0  UNICODE "weixin"
	//$+C      >00000006
	//$+10     >00000008
	//$+30     >0E754EC8  UNICODE "v1_c8d90e1960c4e8aca7ac312ad40f0b8696a323335d624b7"
	//$+34     >0000004C
	//$+38     >00000080
	//$+48     >00000001
	//$+4C     >00000038
	//$+64     >0480B520  UNICODE "微信团队"
	//$+68     >00000004
	//$+6C     >00000004
	//$+A0     >00000001
	//$+A4     >0480B610  UNICODE "WXTD"
	//$+A8     >00000004
	//$+AC     >00000004
	//$+B8     >0E25E0C0  UNICODE "weixintuandui"
	//$+BC     >0000000D
	//$+C0     >00000010
	//$+130    >0000000F
	//$+13C    >00000001
	//$+140    >00000001
	//$+148    >0E7512A8
	//$+14C    >0000028E


	//个人微信号、群号
	wstring wxid1 = GetMsgByAddress(r_esi + 0x8);
	wstring wxName = GetMsgByAddress(r_esi + 0x1C);
	wstring v1 = GetMsgByAddress(r_esi + 0x30);
	//个人微信昵称、群昵称
	wstring nickName = GetMsgByAddress(r_esi + 0x64);
	USER_INFO userInfo(wxid1, wxName, v1, nickName);

	for (auto& userInfoOld : userInfoList)
	{
		wstring wxid = get<0>(userInfoOld);
		if (wxid == wxid1)
		{
			return;
		}
	}

	userInfoList.push_front(userInfo);
}
//检查微信是否已经登录
VOID CheckLoginFinished()
{
	if (isCheckLoginRunning == TRUE)
	{
		return;
	}

	isCheckLoginRunning = TRUE;
	DWORD oldListLenA = 0;
	DWORD oldListLenB = 0;

	while (true)
	{
		DWORD listLen = userInfoList.size();
		if (oldListLenA != listLen)
		{
			oldListLenA = listLen;
			oldListLenB = 0;
			Sleep(1000);
			continue;
		}

		if (oldListLenB == 0)
		{
			//更新列表
			ListContrlSetItems();
			oldListLenB = oldListLenA;
		}

		Sleep(500);
	}
}
//检查微信版本是否匹配
BOOL IsWxVersionValid()
{
	WCHAR VersionFilePath[MAX_PATH];
	if (GetModuleFileName((HMODULE)wxBaseAddress, VersionFilePath, MAX_PATH) == 0)
	{
		return FALSE;
	}

	string asVer = "";
	VS_FIXEDFILEINFO* pVsInfo;
	unsigned int iFileInfoSize = sizeof(VS_FIXEDFILEINFO);
	int iVerInfoSize = GetFileVersionInfoSize(VersionFilePath, NULL);
	if (iVerInfoSize != 0) {
		char* pBuf = new char[iVerInfoSize];
		if (GetFileVersionInfo(VersionFilePath, 0, iVerInfoSize, pBuf)) {
			if (VerQueryValue(pBuf, TEXT("\\"), (void**)& pVsInfo, &iFileInfoSize)) {
				//主版本2.6.7.57
				//2
				int s_major_ver = (pVsInfo->dwFileVersionMS >> 16) & 0x0000FFFF;
				//6
				int s_minor_ver = pVsInfo->dwFileVersionMS & 0x0000FFFF;
				//7
				int s_build_num = (pVsInfo->dwFileVersionLS >> 16) & 0x0000FFFF;
				//57
				int s_revision_num = pVsInfo->dwFileVersionLS & 0x0000FFFF;

				//把版本变成字符串
				strstream wxVer;
				wxVer << s_major_ver << "." << s_minor_ver << "." << s_build_num << "." << s_revision_num;
				wxVer >> asVer;
			}
		}
		delete[] pBuf;
	}

	//版本匹配
	if (asVer == wxVersoin)
	{
		return TRUE;
	}

	//版本不匹配
	return FALSE;
}
//注入的DLL自己卸载自己
VOID UnInject()
{
	HMODULE hModule = NULL;

	//GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS 会增加引用计数
	//因此，后面还需执行一次FreeLibrary
	//直接使用本函数（UnInject）地址来定位本模块
	GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS, (LPWSTR)& UnInject, &hModule);

	if (hModule != 0)
	{
		//减少一次引用计数
		FreeLibrary(hModule);
		//从内存中卸载
		FreeLibraryAndExitThread(hModule, 0);
	}
}

//保存微信联系人到文本文件
VOID SaveUserToTxtFie()
{
	wstring wxUserFileName = L"WxUserLists.txt";
	DWORD index = 0;

	//作为输出文件打开
	ofstream ofile;
	ofile.open(wxUserFileName, ios_base::trunc | ios_base::binary | ios_base::in);
	//char const* const utf16head = "\xFF\xFE ";
	//ofile.write(utf16head, 2);

	for (auto& userInfoOld : userInfoList)
	{
		wstring wxid1 = get<0>(userInfoOld);
		wstring wxName = get<1>(userInfoOld);
		wstring nickName = get<3>(userInfoOld);

		if (wxid1 == L"" && wxName == L"" && nickName == L"") continue;

		index++;

		wstring userText = L"";
		userText.append(to_wstring(index))
			.append(L"\t")
			.append(wxid1)
			.append(L"\t")
			.append(wxName)
			.append(L"\t")
			.append(nickName)
			.append(L"\r\n");

		string strintStr = Wstring2String(userText);
		char const* pos = (char const*)strintStr.c_str();

		////写入文件
		ofile.write(pos, strintStr.length());
	}
	ofile.flush();
	ofile.close();
	ShellExecute(NULL, NULL, L"notepad.exe", wxUserFileName.c_str(), L".\\", SW_SHOW);
}

#pragma region 列表框处理
//向初始化ListView
void InitListContrl()
{
	hListView = GetDlgItem(hWinDlg, IDC_LIST);

	ListView_SetExtendedListViewStyleEx(hListView,
		LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES,
		LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES
	);

	DWORD i = 0;

	//添加ListView表头
	LVCOLUMN pcol = { 0 };
	pcol.mask = LVCF_FMT | LVCF_WIDTH | LVCF_TEXT | LVCF_SUBITEM;
	pcol.fmt = LVCFMT_LEFT;
	pcol.cx = 50;
	pcol.pszText = (LPWSTR)L"序号";
	ListView_InsertColumn(hListView, i++, &pcol);

	pcol.cx = 140;
	pcol.pszText = (LPWSTR)L"微信ID";
	ListView_InsertColumn(hListView, i++, &pcol);

	pcol.cx = 120;
	pcol.pszText = (LPWSTR)L"微信名";
	ListView_InsertColumn(hListView, i++, &pcol);

	pcol.cx = 200;
	pcol.pszText = (LPWSTR)L"昵称";
	ListView_InsertColumn(hListView, i++, &pcol);
}
//向ListView中添加数据
VOID ListContrlSetItems()
{
	//列表清空
	ListView_DeleteAllItems(hListView);

	//添加数据项
	LVITEM item = { 0 };
	item.mask = LVIF_TEXT | LVCF_FMT | LVCF_WIDTH;

	//行号++
	DWORD index = 0;
	for (auto& userInfoOld : userInfoList)
	{

		wstring wxid1 = get<0>(userInfoOld);
		wstring wxName = get<1>(userInfoOld);
		//wstring v1 = get<2>(userInfoOld);
		wstring nickName = get<3>(userInfoOld);

		if (wxid1 == L"" && wxName == L"") continue;

		//行号++
		index++;

		//单元格序号
		DWORD i = 0;

		//插入一行
		item.iItem = index;
		item.iSubItem = i++;
		wstring id(to_wstring(index));
		item.pszText = (LPWSTR)id.c_str();
		ListView_InsertItem(hListView, &item);

		//添加单元格数据
		//微信ID
		item.iItem = index - 1;
		item.iSubItem = i++;
		item.pszText = (LPWSTR)wxid1.c_str();
		ListView_SetItem(hListView, &item);

		//添加单元格数据
		//微信名
		item.iSubItem = i++;
		item.pszText = (LPWSTR)wxName.c_str();
		ListView_SetItem(hListView, &item);

		//添加单元格数据
		//昵称
		item.iSubItem = i++;
		item.pszText = (LPWSTR)nickName.c_str();
		ListView_SetItem(hListView, &item);
	}
}
#pragma endregion

#pragma region 内存读取
//读取内存中的字符串
//存储格式
//xxxxxxxx:字符串地址（memAddress）
//xxxxxxxx:字符串长度（memAddress +4）
LPCWSTR GetMsgByAddress(DWORD memAddress)
{
	//获取字符串长度
	DWORD msgLength = *(DWORD*)(memAddress + 4);
	DWORD msgMaxLength = *(DWORD*)(memAddress + 4);
	if (msgLength == 0 || msgLength > msgMaxLength)
	{
		WCHAR* msg = new WCHAR[1];
		msg[0] = 0;
		return msg;
	}

	WCHAR* msg = new WCHAR[msgLength + 1];
	ZeroMemory(msg, msgLength + 1);

	//复制内容
	wmemcpy_s(msg, msgLength + 1, (WCHAR*)(*(DWORD*)memAddress), msgLength + 1);
	return msg;
}

//某地址中数据的十六进制字符串
wstring GetHexStringByAddress(DWORD memAddress)
{
	//获取字符串长度
	DWORD data = *(DWORD*)(memAddress);

	//定义字符串流
	wstringstream ioss;

	//存放转化后字符
	wstring s_temp;

	//以8位十六制(大写)形式输出
	ioss.fill('0');
	ioss << setiosflags(ios::uppercase) << setw(8) << hex << data;
	ioss >> s_temp;

	return L"0x" + s_temp;
}
#pragma endregion

#pragma region 字符串处理函数
//将string转换成wstring  
wstring String2Wstring(string str)
{
	wstring result;
	//获取缓冲区大小，并申请空间，缓冲区大小按字符计算  
	int len = MultiByteToWideChar(CP_ACP, 0, str.c_str(), str.size(), NULL, 0);
	TCHAR* buffer = new TCHAR[len + 1];
	//多字节编码转换成宽字节编码  
	MultiByteToWideChar(CP_ACP, 0, str.c_str(), str.size(), buffer, len);
	buffer[len] = '\0';             //添加字符串结尾  
	//删除缓冲区并返回值  
	result.append(buffer);
	delete[] buffer;
	return result;
}

//字符串转换
string Wstring2String(const wstring& wstr)
{
	LPCWSTR pwszSrc = wstr.c_str();
	int nLen = WideCharToMultiByte(CP_ACP, 0, pwszSrc, -1, NULL, 0, NULL, NULL);
	if (nLen == 0)
		return string("");

	char* pszDst = new char[nLen];
	if (!pszDst)
		return string("");

	WideCharToMultiByte(CP_ACP, 0, pwszSrc, -1, pszDst, nLen, NULL, NULL);
	string str(pszDst);
	delete[] pszDst;
	pszDst = NULL;

	return str;
}

//把string 转换为 LPCWSTR
LPCWSTR String2LPCWSTR(string text)
{
	//原型：
	//typedef _Null_terminated_ CONST WCHAR *LPCWSTR, *PCWSTR;
	//typedef wchar_t WCHAR;

	size_t size = text.length();
	WCHAR* buffer = new WCHAR[size + 1];
	MultiByteToWideChar(CP_ACP, 0, text.c_str(), -1, buffer, size + 1);

	//确保以 '\0' 结尾
	buffer[size] = 0;
	return buffer;
}

//将int转成16进制字符串
string Dec2Hex(DWORD i)
{
	//定义字符串流
	stringstream ioss;

	//存放转化后字符
	string s_temp;

	//以8位十六制(大写)形式输出
	ioss.fill('0');
	ioss << setiosflags(ios::uppercase) << setw(8) << hex << i;
	ioss >> s_temp;

	return "0x" + s_temp;
}

#pragma endregion