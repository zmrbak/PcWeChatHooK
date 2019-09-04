// Wx67.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <list>
#include<sstream>
using namespace std;
class MyClass
{
public:
	int a = 0;
	string b = "";
};


int main()
{
	list<MyClass> myList;
	printf("%p", myList);

	for (size_t i = 1; i < 5; i++)
	{
		MyClass myClass;
		myClass.a = i;

		stringstream ss;
		string str;
		ss << i;
		ss >> str;
		myClass.b = "String"+ str;
		myList.push_back(myClass);
	}

    std::cout << "Hello World!\n";
}
