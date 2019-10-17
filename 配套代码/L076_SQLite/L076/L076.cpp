// L076.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//
#include"sqlite3.h"
#include<stdio.h>
#include<stdlib.h>
#include<string.h>

int MyCallback(void* para, int nColumn, char** colValue, char** colName);//回调函数

int main(int argc, char* argv[])
{
	sqlite3* db = NULL;
	int result = sqlite3_open("test.db", &db);

	if (result != SQLITE_OK)
	{
		printf("open database text.db failed \n");
		return 0;
	}
	else
	{
		printf("open database text.db success \n");
	}
	////////开始执行sqlite
	const char* sql = "create table Student(t_id integer primary key autoincrement, t_name varchar(15), t_age integer)";
	char* errMsg = NULL;
	result = sqlite3_exec(db,
		sql,
		NULL,
		NULL,
		&errMsg);
	if (result != SQLITE_OK)
	{
		printf("create table Student failed\n");
		printf("error conde %d \t error message:%s", result, errMsg);
	}
	//插入数据
	errMsg = NULL;
	sql = "insert into Student(t_name, t_age) values ('dwb', 23)";
	result = sqlite3_exec(db,
		sql,
		NULL,
		NULL,
		&errMsg);
	printf("insert message1:%s \n", errMsg);

	errMsg = NULL;
	sql = "insert into Student(t_name, t_age) values ('dhx', 25)";
	result = sqlite3_exec(db,
		sql,
		NULL,
		NULL,
		&errMsg);
	printf("insert message2:%s \n", errMsg);

	errMsg = NULL;
	sql = "insert into Student(t_name, t_age) values ('dwz', 21)";
	result = sqlite3_exec(db,
		sql,
		NULL,
		NULL,
		&errMsg);
	printf("insert message3:%s \n", errMsg);

	////////////////////////////////////////////////////////
	errMsg = NULL;
	sql = "select * from Student;";
	result = sqlite3_exec(db, sql, MyCallback, NULL, &errMsg);
	printf("select message:%s \n", errMsg);

	//执行不用回调函数的sql语句，先要设置函数所需的参数
	printf("\nUSEING sqlite3_get_table()----------------------------\n");
	sql = "select * from Student;";

	int nCols;
	int nRows;
	char** azResult;
	errMsg = NULL;
	int index = 0;

	result = sqlite3_get_table(db, sql, &azResult, &nRows, &nCols, &errMsg);
	printf("result = %d \t errMsg = %s \n", result, errMsg);
	printf("rows:%d \t cols: %d \n", nRows, nCols);
	index = nCols;

	printf("++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n");
	for (int i = 0; i < nRows; i++)
	{
		for (int j = 0; j < nCols; j++)
		{
			printf("%s::%s", azResult[j], azResult[index]);
			index++;
			printf("\n");
		}
	}
	printf("++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n");

	sqlite3_free_table(azResult);
	sqlite3_close(db);

	return 0;
}

int MyCallback(void* para, int nColumn, char** colValue, char** colName)
{
	printf("----------------------------------------------------\n");
	printf("包含的列数：%d\n", nColumn);
	for (int i = 0; i < nColumn; i++)
	{
		printf("%s :%s\n", *(colName + i), colValue[i]);//指针和数组的两种写法
	}
	printf("----------------------------------------------------\n");
	return 0;
}