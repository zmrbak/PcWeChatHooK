// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "resource.h"
#include <Windows.h>
#include <string>
#include <CommCtrl.h>
#include <fstream>
#include <list>
#include "shellapi.h"

using namespace std;
//对话框句柄
HWND hWinDlg;
HWND hListView = NULL;
//用户列表
list<wstring> userInfoList(1);


//声明函数
INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam);
VOID	ShowDemoUI(HMODULE hModule);
VOID	DoAction();
VOID	InitListContrl();
VOID	SaveToTxtFie();
string	WstringToString(const wstring& wstr);
LPCWSTR	GetMsgByAddress(DWORD memAddress);
VOID	AddList(DWORD res_eax);

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
		InitListContrl();
		break;
	}
	case WM_CLOSE:
		//关闭窗口事件
		EndDialog(hwndDlg, 0);
		break;
	case WM_COMMAND:

		//调用函数
		if (wParam == ID_GET)
		{
			DoAction();
			break;
		}

		//保存到文件
		if (wParam == IDC_SAVE)
		{
			SaveToTxtFie();
			break;
		}
		break;
	default:
		break;
	}
	return FALSE;
}

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
}

VOID SaveToTxtFie()
{
	wstring wxUserFileName = L"WxUserLists.txt";
	DWORD index = 0;

	//作为输出文件打开
	ofstream ofile;
	ofile.open(wxUserFileName, ios_base::trunc | ios_base::binary | ios_base::in);
	//char const* const utf16head = "\xFF\xFE ";
	//ofile.write(utf16head, 2);

	for (auto& wxid : userInfoList)
	{
		index++;

		wstring userText = L"";
		userText.append(to_wstring(index))
			.append(L"\t")
			.append(wxid)
			.append(L"\r\n");

		string strintStr = WstringToString(userText);
		char const* pos = (char const*)strintStr.c_str();

		////写入文件
		ofile.write(pos, strintStr.length());
	}
	ofile.flush();
	ofile.close();

	ShellExecute(NULL, NULL, L"notepad.exe", wxUserFileName.c_str(), L".\\", SW_SHOW);
}

std::string WstringToString(const std::wstring& wstr)
{
	LPCWSTR pwszSrc = wstr.c_str();
	int nLen = WideCharToMultiByte(CP_ACP, 0, pwszSrc, -1, NULL, 0, NULL, NULL);
	if (nLen == 0)
		return std::string("");

	char* pszDst = new char[nLen];
	if (!pszDst)
		return std::string("");

	WideCharToMultiByte(CP_ACP, 0, pwszSrc, -1, pszDst, nLen, NULL, NULL);
	std::string str(pszDst);
	delete[] pszDst;
	pszDst = NULL;

	return str;
}

VOID DoAction()
{
	//122A8258  13A9A3F0  UNICODE "5847657683@chatroom"
	//群聊微信ID
	WCHAR wxid[50];
	HWND hROOMID = GetDlgItem(hWinDlg, IDC_ROOMID);
	GetWindowText(hROOMID, wxid, 50);
	TEXT_WX wxRoomId(wxid);

	//获取微信基址
	HMODULE dllAdress = GetModuleHandleA("WeChatWin.dll");

	//要调用的CALL
	DWORD call1 = (DWORD)dllAdress + 0x415030;
	DWORD call2 = (DWORD)dllAdress + 0x2B6E80;
	DWORD call3 = (DWORD)dllAdress + 0x41D7F0;
	DWORD call4 = (DWORD)dllAdress + 0x415B20;

	//返回结果
	DWORD res_eax = 0;

	//缓冲区
	//[local.93]==>[ebp-00000174]
	char userListBuff[0x174] = { 0 };

	DWORD myESP = 0;

	//获取群用户
	__asm
	{
		//5C64FBD5  |> \8D8D 9CFEFFFF lea ecx,[local.89]
		//5C64FBDB  |.  E8 50542100   call WeChatWi.5C865030
		//5C64FBE0  |.  C745 FC 01000>mov [local.1],0x1
		//5C64FBE7  |.  C785 8CFEFFFF>mov [local.93],0x0
		//5C64FBF1  |.  C785 90FEFFFF>mov [local.92],0x0
		//5C64FBFB  |.  C785 94FEFFFF>mov [local.91],0x0
		//5C64FC05  |.  8D85 9CFEFFFF lea eax,[local.89]
		//5C64FC0B  |.  C645 FC 02    mov byte ptr ss:[ebp-0x4],0x2
		//5C64FC0F  |.  50            push eax
		//5C64FC10  |.  53            push ebx
		//5C64FC11  |.  E8 6A720B00   call WeChatWi.5C706E80
		//5C64FC16  |.  8BC8          mov ecx,eax
		//5C64FC18  |.  E8 D3DB2100   call WeChatWi.5C86D7F0                   ;  数据构造完成
		//5C64FC1D  |.  8D85 8CFEFFFF lea eax,[local.93]                       ;  数据生成
		//5C64FC23  |.  50            push eax
		//5C64FC24  |.  8D8D 9CFEFFFF lea ecx,[local.89]
		//5C64FC2A  |.  E8 F15E2100   call WeChatWi.5C865B20
		//5C64FC2F  |.  83EC 0C       sub esp,0xC
		//5C64FC32  |.  8D85 8CFEFFFF lea eax,[local.93]                       ;  数据生成


		//###########################################################################################
		mov myESP, esp

		//WeChatWin.dll+1FFBD5 - 8D 8D 9CFEFFFF        - lea ecx,[ebp-00000164]
		lea ecx, userListBuff[0x174 - 0x164]

		//第一个调用
		//WeChatWin.dll+1FFBDB - E8 50542100           - call WeChatWin.dll+415030
		call call1

		//WeChatWin.dll+1FFBE0 - C7 45 FC 01000000     - mov [ebp-04],00000001 { 1 }
		mov userListBuff[0x174 - 0x4], 1

		//WeChatWin.dll+1FFBE7 - C7 85 8CFEFFFF 00000000 - mov [ebp-00000174],00000000 { 0 }
		//WeChatWin.dll+1FFBF1 - C7 85 90FEFFFF 00000000 - mov [ebp-00000170],00000000 { 0 }
		//WeChatWin.dll+1FFBFB - C7 85 94FEFFFF 00000000 - mov [ebp-0000016C],00000000 { 0 }
		//WeChatWin.dll+1FFC05 - 8D 85 9CFEFFFF        - lea eax,[ebp-00000164]
		lea eax, userListBuff[0x174 - 0x164]

		//WeChatWin.dll+1FFC0B - C6 45 FC 02           - mov byte ptr [ebp-04],02 { 2 }
		mov userListBuff[0x174 - 0x4], 2

		//WeChatWin.dll+1FFC0F - 50                    - push eax
		push eax

		//WeChatWin.dll+1FFC10 - 53                    - push ebx
		lea ebx, wxRoomId
		push ebx

		//第二个调用
		//WeChatWin.dll+1FFC11 - E8 6A720B00           - call WeChatWin.dll+2B6E80
		call call2

		//WeChatWin.dll+1FFC16 - 8B C8                 - mov ecx,eax
		mov ecx, eax

		//第三个调用
		//WeChatWin.dll+1FFC18 - E8 D3DB2100           - call WeChatWin.dll+41D7F0
		call call3

		//WeChatWin.dll+1FFC1D - 8D 85 8CFEFFFF        - lea eax,[ebp-00000174]
		lea eax, userListBuff[0]

		//WeChatWin.dll+1FFC23 - 50                    - push eax
		push eax

		//WeChatWin.dll+1FFC24 - 8D 8D 9CFEFFFF        - lea ecx,[ebp-00000164]
		lea ecx, userListBuff[0x174 - 0x164]

		//第四个调用
		//WeChatWin.dll+1FFC2A - E8 F15E2100           - call WeChatWin.dll+415B20
		call call4

		//---------------------------------------------提取寄存器数据---------------------------------
		mov res_eax, eax
		//---------------------------------------------提取寄存器数据---------------------------------

		//WeChatWin.dll+1FFC2F - 83 EC 0C              - sub esp,0C { 12 }
		//sub esp, 0x0C

		//恢复esp寄存器
		mov eax, myESP
		mov esp, eax
		//###########################################################################################
	}

	AddList(res_eax);

}

VOID AddList(DWORD res_eax)
{
	DWORD userList = *((DWORD*)res_eax);

	//微信群成员数量
	int userCount = *((int*)(res_eax + 0xB0));

	//列表清空
	ListView_DeleteAllItems(hListView);

	//清空内存中联系人列表
	userInfoList.clear();


	for (int i = 0; i < userCount; i++)
	{
		wstring wxid = GetMsgByAddress(userList + (i * 20));
		userInfoList.push_back(wxid);
	}

	//联系人排序
	userInfoList.sort();


	//添加数据项
	DWORD index = 0;
	LVITEM item = { 0 };
	item.mask = LVIF_TEXT | LVCF_FMT | LVCF_WIDTH;
	for (auto& wxid : userInfoList)
	{
		//插入一行
		index++;
		item.iItem = index;
		item.iSubItem = 0;
		wstring id(to_wstring(index));
		item.pszText = (LPWSTR)id.c_str();
		ListView_InsertItem(hListView, &item);


		//添加单元格数据
		//微信ID
		item.iItem = index - 1;
		item.iSubItem = 1;
		item.pszText = (LPWSTR)wxid.c_str();
		ListView_SetItem(hListView, &item);
	}
}

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
