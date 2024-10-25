// ConsoleApplication1.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "PaxAliasPro.h"
#include <string>
#include <iostream>
#include <vector>
#include <filesystem>
using namespace std;
namespace fs = experimental::filesystem;




int main()
{
	int typePos;
	string temp;
	string path = "./";
	vector<string> files;
	for (auto &p : fs::directory_iterator(path)) {
		temp = p.path().string();
		if (temp.find(".L5K") != string::npos) { //tag found in line
			typePos = temp.find(".L5K");
			files.push_back(temp.substr(2, temp.size() -1));
			cout << temp << endl;
		}
	}

	for (int i = 0; i < static_cast<int>(files.size()); i++)
	PaxAliasPro p(files[i]);
	
	return 0;
}

