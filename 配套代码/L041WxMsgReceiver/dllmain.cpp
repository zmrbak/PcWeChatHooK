// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "resource.h"
#include "shellapi.h"
#include <string>
#include <tchar.h> 
#include <iostream>
#include <sstream>
#include <string>
#include <iomanip>
#include <strstream>

#pragma comment(lib, "Version.lib")

using namespace std;

//声明函数
VOID ShowDemoUI(HMODULE hModule);
INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam);
VOID HookWx();
VOID UnHookWx();
VOID RecieveMsg();
VOID RecieveMsgHook();
string Dec2Hex(DWORD i);
LPCWSTR String2LPCWSTR(string text);
LPCWSTR GetMsgByAddress(DWORD memAddress);
BOOL IsWxVersionValid();
wstring String2Wstring(string str);

//定义变量
DWORD wxBaseAddress = 0;
//HOOK标志
BOOL isWxHooked = FALSE;
//对话框句柄
HWND hWinDlg;
//跳回地址
DWORD jumBackAddress = 0;
//我们要提取的寄存器内容
DWORD r_esp = 0;
//此HOOK匹配的微信版本
const string wxVersoin = "2.6.7.57";
//我自己的微信ID
string myWxId = "";

CHAR originalCode[5] = { 0 };


//使用VS+Detours调试，必须一个没用的导出函数
VOID __declspec(dllexport) Test()
{
	//OutputDebugString(TEXT("开始调试"));
}


//DLL入口函数
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
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
	wxBaseAddress = (DWORD)GetModuleHandle(TEXT("WeChatWin.dll"));
	string text = "微信基址：\t";
	text.append(Dec2Hex(wxBaseAddress));
	OutputDebugString(String2LPCWSTR(text));

	DialogBox(hModule, MAKEINTRESOURCE(IDD_MAIN), NULL, &DialogProc);
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

//窗口回调函数，处理窗口事件
INT_PTR CALLBACK DialogProc(_In_ HWND   hwndDlg, _In_ UINT   uMsg, _In_ WPARAM wParam, _In_ LPARAM lParam)
{
	hWinDlg = hwndDlg;
	switch (uMsg)
	{
	case WM_INITDIALOG:
		break;

	case WM_CLOSE:
		//关闭窗口事件
		EndDialog(hwndDlg, 0);
		break;
	case WM_COMMAND:

		//接收消息
		if (wParam == ID_START)
		{
			OutputDebugString(TEXT("接收消息按钮被点击"));
			HookWx();

			HWND hEditor = GetDlgItem(hwndDlg, IDC_MSG);
			SetWindowText(hEditor, TEXT("开始准备接收微信消息......\r\n"));
			break;
		}

		//停止接收
		if (wParam == ID_STOP)
		{
			OutputDebugString(TEXT("停止接收按钮被点击"));
			UnHookWx();

			HWND hFileHelper = GetDlgItem(hwndDlg, IDC_MSG);
			SetWindowText(hFileHelper, TEXT("停止准备接收微信消息......"));
			break;
		}


		//打开视频帮助页面
		if (wParam == ID_HELP)
		{
			OutputDebugString(TEXT("帮助按钮被点击"));
			ShellExecute(hwndDlg, TEXT("open"), TEXT("http://t.cn/EXUbebQ"), NULL, NULL, NULL);
			break;
		}


		//QQ群交流
		if (wParam == ID_GITHUB)
		{
			OutputDebugString(TEXT("QQ群交流按钮被点击"));
			ShellExecute(hwndDlg, TEXT("open"), TEXT("https://github.com/zmrbak/PcWeChatHooK"), NULL, NULL, NULL);
			break;
		}

		//重启微信
		if (wParam == ID_WX_REBOOT)
		{
			OutputDebugString(TEXT("重启微信按钮被点击"));

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
				//WaitForSingleObject(procStruct.hProcess, INFINITE);
				CloseHandle(procStruct.hProcess);
				CloseHandle(procStruct.hThread);
			}

			//终止当前进程
			TerminateProcess(GetCurrentProcess(), 0);
			break;
		}
		break;
	default:
		break;
	}
	return FALSE;
}

//Hook接收消息
VOID HookWx()
{
	//判断是否已经HOOK
	if (isWxHooked == FALSE)
	{
		isWxHooked = TRUE;

		//WeChatWin.dll+0x310573
		int hookAddress = wxBaseAddress + 0x310573;
		string debugMsg = "Hook的地址：\t";
		debugMsg.append(Dec2Hex(hookAddress));
		OutputDebugString(String2LPCWSTR(debugMsg));

		//跳回的地址
		jumBackAddress = hookAddress + 5;

		//组装跳转数据
		BYTE jmpCode[5] = { 0 };
		jmpCode[0] = 0xE9;

		//新跳转指令中的数据=跳转的地址-原地址（HOOK的地址）-跳转指令的长度
		*(DWORD*)& jmpCode[1] = (DWORD)RecieveMsgHook - hookAddress - 5;

		//保存当前位置的指令,在unhook的时候使用。
		ReadProcessMemory(GetCurrentProcess(), (LPVOID)hookAddress, originalCode, 5, 0);

		//覆盖指令 B9 E8CF895C //mov ecx,0x5C89CFE8
		WriteProcessMemory(GetCurrentProcess(), (LPVOID)hookAddress, jmpCode, 5, 0);
	}
}

//UnHook接收消息
VOID UnHookWx()
{
	if (isWxHooked == TRUE)
	{
		//恢复指令
		//B9 E8CF895C
		//mov ecx,0x5C89CFE8
		//##################################################################################
		// 2019-06-20 修订
		// 感谢群里的Lost朋友（1580500224）发现，这个地址中的数据不是固定的。
		// 因此，在HOOK的时候，需要保存这个地址中的数据，然后在unhook的时候恢复。
		//##################################################################################
		//BYTE originalCode[5] = { 0xB9,0xE8,0xCF,0x89,0x5C };

		//恢复指令的地址
		int hookAddress = wxBaseAddress + 0x310573;

		//恢复指令
		WriteProcessMemory(GetCurrentProcess(), (LPVOID)hookAddress, originalCode, 5, 0);

		isWxHooked = FALSE;
	}
}

//跳转到这里，让我们自己处理消息
__declspec(naked) VOID RecieveMsgHook()
{
	//5B950554  |.  C700 00000000         mov dword ptr ds:[eax],0x0
	//5B95055A  |.  C740 04 00000000      mov dword ptr ds:[eax+0x4],0x0
	//5B950561  |.  C740 08 00000000      mov dword ptr ds:[eax+0x8],0x0
	//5B950568  |.  A3 1CCC8A5C           mov dword ptr ds:[0x5C8ACC1C],eax
	//5B95056D  |>  50                    push eax
	//5B95056E  |.  A1 E8CF895C           mov eax,dword ptr ds:[0x5C89CFE8]
	//Inline HOOK这一句
	//mov ecx,0x5C89CFE8
	//5B950573  |.  B9 E8CF895C           mov ecx,WeChatWi.5C89CFE8

		//保存现场
	__asm
	{
		//补充被覆盖的代码
		//5B950573  |.  B9 E8CF895C           mov ecx,WeChatWi.5C89CFE8
		/*
		有不少同学在这里遇到一些困惑，现在添加一些备注，如果还时不能理解，请在群中咨询。
		
		上面的注释，来自于OD
		
		其中，5B950573，是当前程序中的地址。
		由于每次微信启动后，WeChatWin.dll在内存中的地址不一样，因此，这个地址也是不一样的。

		B9 E8CF895C，这是当前"5B950573"地址中保存的机器码，程序编译的时候确定的，通常是不会变化的。

		mov ecx,WeChatWi.5C89CFE8，这一句，是“伪汇编语言”，是机器码“B9 E8CF895C”经过解析之后，让“人”看的内容
		而对应的汇编语言代码是“mov ecx, 0x5C89CFE8”。在OD中，双击这一行，弹出的对话框中就是这一句。


		我们在HOOK的时候，恰好把这一句“B9 E8CF895C”（mov ecx, 0x5C89CFE8）覆盖掉了，那么程序运行到这里的时候，
		需要把这一句执行掉，否则程序可能崩溃。

		B9 E8CF895C  ==> mov ecx, 0x5C89CFE8
		E8CF895C     ==> 0x5C89CFE8（倒着看，两个字符一次分割。关键词“大端、小端”，可以请教度娘，有详细解释）
		由于在加载WeChatWin.dll的时候，会经过一次内存重新定位，因此，这个数据可能每次都会不同。
		最简单的方式是，使用基址+偏移来计算这个数字。（使用CE反编译这一段代码，CE会自定计算出来的）

		通常情况下，我们不应该这样“硬编码”。
		而应该在覆盖之前，备份这句代码，等程序运行到这里的时候，再执行备份出来的代码。

		注：课程演示代码，以简单、易学为主。
		如果要做成产品，则需要进一步优化，“硬编码”是“大忌”！切记！！。
		*/
		//mov ecx,0x5C89CFE8
		mov ecx, 0x5C89CFE8

		//提取esp寄存器内容，放在一个变量中

		//这里使用全局变量 r_esp，将导致丢失消息，原因：多线程情况下，新数据覆盖旧数据
		//如何使用局部变量，请继续学习课程（后面课程中，有讲解），或者自行探索，或在群里问师兄弟们。
		mov r_esp, esp

		//保存寄存器
		pushad
		pushf
	}

	//调用接收消息的函数
	RecieveMsg();

	//恢复现场
	__asm
	{
		popf
		popad

		//跳回被HOOK指令的下一条指令
		jmp jumBackAddress
	}
}

VOID RecieveMsg()
{
	//这个函数，取到数据马上启动一个线程去处理，并且立即返回，不要执行太多逻辑。
	//这里是课程演示代码，不宜搞得太复杂。

	wstring receivedMessage = TEXT("");
	BOOL isFriendMsg = FALSE;
	//[[esp]]
	//信息块位置
	DWORD** msgAddress = (DWORD * *)r_esp;

	//消息类型[[esp]]+0x30
	//[01文字] [03图片] [31转账XML信息] [22语音消息] [02B视频信息]
	//感谢：.順唭_自嘫ɑ、unravel提供类型消息。
	DWORD msgType = *((DWORD*)(**msgAddress + 0x30));
	receivedMessage.append(TEXT("消息类型:"));
	switch (msgType)
	{
	case 0x01:
		receivedMessage.append(TEXT("文字"));
		break;
	case 0x03:
		receivedMessage.append(TEXT("图片"));
		break;

	case 0x22:
		receivedMessage.append(TEXT("语音"));
		break;
	case 0x25:
		receivedMessage.append(TEXT("好友确认"));
		break;
	case 0x28:
		receivedMessage.append(TEXT("POSSIBLEFRIEND_MSG"));
		break;
	case 0x2A:
		receivedMessage.append(TEXT("名片"));
		break;
	case 0x2B:
		receivedMessage.append(TEXT("视频"));
		break;
	case 0x2F:
		//石头剪刀布
		receivedMessage.append(TEXT("表情"));
		break;
	case 0x30:
		receivedMessage.append(TEXT("位置"));
		break;
	case 0x31:
		//共享实时位置
		//文件
		//转账
		//链接
		receivedMessage.append(TEXT("共享实时位置、文件、转账、链接"));
		break;
	case 0x32:
		receivedMessage.append(TEXT("VOIPMSG"));
		break;
	case 0x33:
		receivedMessage.append(TEXT("微信初始化"));
		break;
	case 0x34:
		receivedMessage.append(TEXT("VOIPNOTIFY"));
		break;
	case 0x35:
		receivedMessage.append(TEXT("VOIPINVITE"));
		break;
	case 0x3E:
		receivedMessage.append(TEXT("小视频"));
		break;
	case 0x270F:
		receivedMessage.append(TEXT("SYSNOTICE"));
		break;
	case 0x2710:
		//系统消息
		//红包
		receivedMessage.append(TEXT("红包、系统消息"));
		break;
	case 0x2712:
		receivedMessage.append(TEXT("撤回消息"));
		break;
	default:
		wostringstream oss;
		oss.fill('0');
		oss << setiosflags(ios::uppercase) << setw(8) << hex << msgType;
		receivedMessage.append(TEXT("未知:0x"));
		receivedMessage.append(oss.str());
		break;
	}
	receivedMessage.append(TEXT("\r\n"));

	//dc [[[esp]] + 0x114]
	//判断是群消息还是好友消息
	//相关信息
	wstring msgSource2 = TEXT("<msgsource />\n");
	wstring msgSource = TEXT("");
	msgSource.append(GetMsgByAddress(**msgAddress + 0x168));

	if (msgSource.length() <= msgSource2.length())
	{
		receivedMessage.append(TEXT("收到好友消息:\r\n"));
		isFriendMsg = TRUE;
	}
	else
	{
		receivedMessage.append(TEXT("收到群消息:\r\n"));
		isFriendMsg = FALSE;
	}

	//好友消息
	if (isFriendMsg == TRUE)
	{
		receivedMessage.append(TEXT("好友wxid：\r\n"))
			.append(GetMsgByAddress(**msgAddress + 0x40))
			.append(TEXT("\r\n\r\n"));
	}
	else
	{
		receivedMessage.append(TEXT("群号：\r\n"))
			.append(GetMsgByAddress(**msgAddress + 0x40))
			.append(TEXT("\r\n\r\n"));

		receivedMessage.append(TEXT("消息发送者：\r\n"))
			.append(GetMsgByAddress(**msgAddress + 0x114))
			.append(TEXT("\r\n\r\n"));

		receivedMessage.append(TEXT("相关信息：\r\n"));
		receivedMessage += msgSource;
		receivedMessage.append(TEXT("\r\n\r\n"));
	}

	receivedMessage.append(TEXT("消息内容：\r\n"))
		.append(GetMsgByAddress(**msgAddress + 0x68))
		.append(TEXT("\r\n\r\n"));


	receivedMessage.append(TEXT("未知内容：\r\n"))
		.append(GetMsgByAddress(**msgAddress + 0x128))
		.append(TEXT("\r\n\r\n"));

	//文本框输出信息
	SetWindowText(GetDlgItem(hWinDlg, IDC_MSG), receivedMessage.c_str());
}

//读取内存中的字符串
//存储格式
//xxxxxxxx:字符串地址（memAddress）
//xxxxxxxx:字符串长度（memAddress +4）
LPCWSTR GetMsgByAddress(DWORD memAddress)
{
	//获取字符串长度
	DWORD msgLength = *(DWORD*)(memAddress + 4);
	if (msgLength == 0)
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
