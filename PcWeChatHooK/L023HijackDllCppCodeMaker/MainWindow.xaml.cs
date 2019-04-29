using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace L023HijackDllCppCodeMaker
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<String> mouduleList = new List<string>();
        String wxPath = "";
        List<String> knownDLLsList = new List<string>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComboRefresh();
            knownDLLsList.Add("wow64.dll");
            knownDLLsList.Add("wow64cpu.dll");
            knownDLLsList.Add("wow64win.dll");
            knownDLLsList.Add("wowarmhw.dll");
            knownDLLsList.Add("advapi32.dll");
            knownDLLsList.Add("clbcatq.dll");
            knownDLLsList.Add("combase.dll");
            knownDLLsList.Add("COMDLG32.dll");
            knownDLLsList.Add("coml2.dll");
            knownDLLsList.Add("difxapi.dll");
            knownDLLsList.Add("gdi32.dll");
            knownDLLsList.Add("gdiplus.dll");
            knownDLLsList.Add("IMAGEHLP.dll");
            knownDLLsList.Add("IMM32.dll");
            knownDLLsList.Add("kernel32.dll");
            knownDLLsList.Add("MSCTF.dll");
            knownDLLsList.Add("MSVCRT.dll");
            knownDLLsList.Add("NORMALIZ.dll");
            knownDLLsList.Add("NSI.dll");
            knownDLLsList.Add("ole32.dll");
            knownDLLsList.Add("OLEAUT32.dll");
            knownDLLsList.Add("PSAPI.dll");
            knownDLLsList.Add("rpcrt4.dll");
            knownDLLsList.Add("sechost.dll");
            knownDLLsList.Add("Setupapi.dll");
            knownDLLsList.Add("SHCORE.dll");
            knownDLLsList.Add("SHELL32.dll");
            knownDLLsList.Add("SHLWAPI.dll");
            knownDLLsList.Add("user32.dll");
            knownDLLsList.Add("WLDAP32.dll");
            knownDLLsList.Add("WS2_32.dll");
        }

        /// <summary>
        /// 刷新模块列表
        /// </summary>
        private void ComboRefresh()
        {
            mouduleList.Clear();
            comboxMoudles.ItemsSource = null;
            //读取注册表(注册表读不出来)
            //HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\KnownDLLs
            //RegistryKey hkeyLocalMachine = Registry.LocalMachine;
            //RegistryKey system = hkeyLocalMachine.OpenSubKey("SYSTEM", true);
            //RegistryKey currentControlSet = system.OpenSubKey("CurrentControlSet", true);
            //RegistryKey control = currentControlSet.OpenSubKey("Control", true);
            //RegistryKey sessionManager = control.OpenSubKey("Session Manager", true);
            //RegistryKey knownDLLs = sessionManager.OpenSubKey("KnownDLLs", true);

            Process[] processes = Process.GetProcessesByName("WeChat");
            if (processes.Length == 0)
            {
                MessageBox.Show("请先启动微信，然后点击下面刷新按钮", "错误！", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
            if (wxPath == "")
            {
                wxPath = System.IO.Path.GetDirectoryName(processes[0].MainModule.FileName);

            }

            foreach (ProcessModule item in processes[0].Modules)
            {
                //排除微信目录下的DLL文件
                if (System.IO.Path.GetDirectoryName(item.FileName) == wxPath) continue;

                //是否需要排除系统禁止的DLL
                if (checkBoxBanned.IsChecked == true)
                {
                    String fileName = System.IO.Path.GetFileName(item.FileName);

                    Boolean banned = false;
                    foreach (String knownDLL in knownDLLsList)
                    {
                        if (fileName.ToUpper() == knownDLL.ToUpper())
                        {
                            banned = true;
                            break;
                        }
                    }
                    if (banned == true)
                    {
                        continue;
                    }
                }

                mouduleList.Add(item.FileName);
            }
            mouduleList.Sort();
            comboxMoudles.ItemsSource = mouduleList;
        }

        //刷新模块列表
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ComboRefresh();
        }

        /// <summary>
        /// 创建Cpp源代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (comboxMoudles.Text == "")
            {
                MessageBox.Show("对应的模块未选中，请重试", "错误！", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            CreateCppFile();
        }

        //创建CPP文件
        private void CreateCppFile()
        {
            String dllFileName = comboxMoudles.Text;
            String cppFileName = Environment.CurrentDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(dllFileName) + ".cpp";

            //IntPtr hFileBase = Win32Native._lopen(@"C:/Windows/System32/ATL.dll", Win32Native.OF_SHARE_COMPAT);
            IntPtr hFileBase = Win32Native._lopen(dllFileName, Win32Native.OF_SHARE_COMPAT);

            IntPtr hFileMapping = Win32Native.CreateFileMapping(hFileBase, Win32Native.NULL, Win32Native.PAGE_READONLY, 0, 0, null);
            IntPtr psDos32pe = Win32Native.MapViewOfFile(hFileMapping, Win32Native.FILE_MAP_READ, 0, 0, Win32Native.NULL);  // e_lfanew 248       
            IMAGE_DOS_HEADER sDos32pe = (IMAGE_DOS_HEADER)Marshal.PtrToStructure(psDos32pe, typeof(IMAGE_DOS_HEADER));
            IntPtr psNt32pe = (IntPtr)(sDos32pe.e_lfanew + (long)psDos32pe);
            IMAGE_NT_HEADERS sNt32pe = (IMAGE_NT_HEADERS)Marshal.PtrToStructure(psNt32pe, typeof(IMAGE_NT_HEADERS));
            // 63 63 72 75 6E 2E 63 6F 6D  
            IntPtr psExportDirectory = Win32Native.ImageRvaToVa(psNt32pe, psDos32pe, sNt32pe.OptionalHeader.ExportTable.VirtualAddress, Win32Native.NULL);
            IMAGE_EXPORT_DIRECTORY sExportDirectory = (IMAGE_EXPORT_DIRECTORY)Marshal.PtrToStructure(psExportDirectory, typeof(IMAGE_EXPORT_DIRECTORY));
            IntPtr ppExportOfNames = Win32Native.ImageRvaToVa(psNt32pe, psDos32pe, sExportDirectory.AddressOfNames, Win32Native.NULL);

            List<String> cppDataList = new List<string>();
            for (uint i = 0, nNoOfExports = sExportDirectory.NumberOfNames; i < nNoOfExports; i++)
            {
                IntPtr pstrExportOfName = Win32Native.ImageRvaToVa(psNt32pe, psDos32pe, (uint)Marshal.ReadInt32(ppExportOfNames, (int)(i * 4)), Win32Native.NULL);
                cppDataList.Add(Marshal.PtrToStringAnsi(pstrExportOfName));
                //Console.WriteLine(Marshal.PtrToStringAnsi(pstrExportOfName));
            }
            Win32Native.UnmapViewOfFile(psDos32pe);
            Win32Native.CloseHandle(hFileMapping);
            Win32Native._lclose(hFileBase);

            StringBuilder stringBuilder = new StringBuilder();


            //检查是否被系统禁用
            Boolean banned = false;
            String fileName = System.IO.Path.GetFileName(comboxMoudles.Text);
            foreach (String knownDLL in knownDLLsList)
            {
                if (fileName.ToUpper() == knownDLL.ToUpper())
                {
                    banned = true;
                    break;
                }
            }
            if (banned == true)
            {
                String banMessage =
@"//您要进行劫持的DLL文件：" + dllFileName + @"
//在系统的监控列表中，除非你在\HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\ExcludeFromKnownDlls中
//添加字符：" + fileName + @"
//否则，使用“" + fileName + @"”劫持是无效的！
";
                stringBuilder.Append(banMessage + Environment.NewLine);
            }



            //DLL文件头部注释
            String codeText =
@"//被劫持的DLL文件：" + dllFileName + @"
//在编译之后，请把文件名改为: " + System.IO.Path.GetFileName(comboxMoudles.Text) + @"
//然后，放在此文件夹中:" + wxPath + @"\

//VS 2019中的编译设置
//配置属性->连接器->常规->启用增量链接:否 (/INCREMENTAL:NO)

//应用程序将在加载此DLL之后
//如果微信目录下存在WxHook.dll，则会自动加载此DLL
//你也可以将其指定为其它DLL文件，请自行修改LoadHookDll()中的相的参数。

//包含的头文件
#pragma once
#include ""pch.h""
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <strsafe.h>
";
            stringBuilder.Append(codeText + Environment.NewLine);

            //宏定义
            codeText =
@"//宏定义
#define EXTERNC extern ""C""
#define NAKED __declspec(naked)
#define EXPORT __declspec(dllexport)
#define ALCPP EXPORT NAKED
#define ALSTD EXTERNC EXPORT NAKED void __stdcall
#define ALCFAST EXTERNC EXPORT NAKED void __fastcall
#define ALCDECL EXTERNC EXPORT NAKED void __cdecl
";
            stringBuilder.Append(codeText);


            //变量定义
            codeText = @"
//变量定义
#pragma region OriginalAddress";
            stringBuilder.Append(codeText + Environment.NewLine);
            for (int i = 0; i < cppDataList.Count; i++)
            {
                string item = cppDataList[i];
                //PVOID pfnGetFileVersionInfoA;
                //String comment = @"#pragma comment(linker, ""/EXPORT:" + item + "=_DG_" + item + @",@" + (i + 1) + @""")";
                String comment = @"PVOID pfn" + item + ";";
                stringBuilder.Append(comment + Environment.NewLine);
            }

            codeText = @"
#pragma endregion

// 原始模块句柄
HMODULE m_hModule = NULL;

// 微信HOOK的DLL句柄
HMODULE hModuleWxHook = NULL;

// 获取原始函数地址
FARPROC WINAPI GetAddress(PCSTR pszProcName)
{
	return GetProcAddress(m_hModule, pszProcName);
}

// 加载原始模块
inline BOOL WINAPI Load()
{
	TCHAR szPath[MAX_PATH] = {};
	//获取系统目录
	GetSystemDirectory(szPath, MAX_PATH);
	StringCchCat(szPath, MAX_PATH, TEXT(""\\" + System.IO.Path.GetFileName(comboxMoudles.Text) + @"""));

	m_hModule = ::LoadLibraryW(szPath);
	return (m_hModule != NULL);
}

// 卸载模块
inline VOID UnLoad()
{
	if (m_hModule)
	{
		::FreeLibrary(m_hModule);
		m_hModule = NULL;
	}
}
";
            stringBuilder.Append(codeText + Environment.NewLine);

            codeText = @"
//预先获取所有原函数的地址
#pragma region GetOriginalAddress
FARPROC WINAPI GetOriginalAddress()
{";
            stringBuilder.Append(codeText + Environment.NewLine);

            for (int i = 0; i < cppDataList.Count; i++)
            {
                string item = cppDataList[i];
                //pfnGetFileVersionInfoA = GetAddress("GetFileVersionInfoA");
                String comment = @"	pfn" + item + " = GetAddress(\"" + item + "\");";
                stringBuilder.Append(comment + Environment.NewLine);
            }

            codeText = @"
	return NULL;
}
#pragma endregion";
            stringBuilder.Append(codeText + Environment.NewLine);

            codeText = @"
//装载微信HOOK
VOID LoadHookDll()
{
	//只对WeChat.exe进行注入的DLL加载
	HMODULE hModule = GetModuleHandle(TEXT(""WeChat.exe""));
	if (hModule == 0)
	{
		return;
	}
    
	WIN32_FIND_DATA FindFileData;
	HANDLE  hFind = FindFirstFile(TEXT(""WxHook.dll""), &FindFileData);

	if (hFind != INVALID_HANDLE_VALUE)
	{
		hModuleWxHook = ::LoadLibraryW(TEXT(""WxHook.dll""));
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 入口函数
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		DisableThreadLibraryCalls(hModule);
		if (Load())
		{
			GetOriginalAddress();
			LoadHookDll();
		}
		else
		{
			return FALSE;
		}
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		UnLoad();
		break;
	}
	return TRUE;
}
";
            stringBuilder.Append(codeText);

            codeText = @"
// 函数被调用时，转发到系统原始DLL的函数
#pragma region OriginalLibraryApi
namespace OriginalLibraryApi
{
";
            stringBuilder.Append(codeText);
            for (int i = 0; i < cppDataList.Count; i++)
            {
                string item = cppDataList[i];
                //ALCDECL GetFileVersionInfoA(void)
                //{
                //    __asm JMP pfnGetFileVersionInfoA;
                //}
                String comment = @"
	ALCDECL " + item + @"(void) 
		{
			__asm	JMP pfn" + item + @";
		}
";
                stringBuilder.Append(comment);
            }

            codeText = @"}
#pragma endregion";

            stringBuilder.Append(codeText);

            //写cpp
            System.IO.File.WriteAllText(cppFileName, stringBuilder.ToString());

            Process.Start("notepad.exe", cppFileName);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            String netStudyUrl = "https://study.163.com/course/introduction/1209042813.htm?utm_source=qq&utm_medium=webShare&utm_campaign=share&utm_content=courseIntro&share=2&shareId=480000001858469";
            Process.Start(netStudyUrl);
        }

        private void CheckBoxBanned_Click(object sender, RoutedEventArgs e)
        {
            ComboRefresh();
        }
    }
}
