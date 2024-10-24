// ConsoleApplication1.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <iomanip>
#include <sstream>
#include "PaxALiasPro.h"
using namespace std;

PaxAliasPro::PaxAliasPro(string filename) {
	//=====Read L5K======//
	ifstream file(filename);
	//ifstream file("test.txt");
	string x;
	string tagType;
	while (!file.eof())
	{
		getline(file, x);
		this->l5k += x + "\n";
	}
	file.close();


	//=====Modify L5K======//
	this->getPTags();

	for (int i = 0; i < static_cast<int>(tagArr.size()); i++) {

		addAlias(tagArr.at(i), tagTypeArr.at(i));
		cout << left << setw(15) << tagTypeArr.at(i) << " | " << setw(30) << tagArr.at(i) << setw(3) << right << fixed << setprecision(1) << ((static_cast<double>(i) + 1.) / static_cast<double>(tagArr.size())) * 100 << "%" << endl;
		
	}


	//=====Write L5K======//
	ofstream ofile("New_" + filename);
	istringstream l5kStream(this->l5k);
	//string x;
	while (!l5kStream.eof())
	{
		getline(l5kStream, x);
		ofile << x << endl;	//pass line back in
	}
	ofile.close();

}

void PaxAliasPro::getPTags() {

	string x;
	istringstream l5kStream(this->l5k);
	vector<string> tagTypes = {"P_VSD", "P_ValveC", "P_ValveMO", "P_ValvMP", "P_ValveSO", "P_Motor", "P_Dout", "P_Aout", "P_Motor2Spd", "P_ValveHO", "P_MotorRev"};
	string tag;
	int tagPos;


	while (getline(l5kStream, x))
	{
		for (int i = 0; i < tagTypes.size(); i++) {
			tag = " : " + tagTypes[i];
			if (x.find(tag) != string::npos) //tag found in line
			{
				while (x[0] == '\t')	//erase leading tabs
					x = x.erase(0, 1);
				tagPos = x.find(tag);
				tagArr.push_back(x.substr(0, tagPos));
				tagTypeArr.push_back(tagTypes[i]);
			}
		}
	}

	l5kStream.clear();
}



void PaxAliasPro::addAlias(string const stringIn, string const tagType)
{
	string x;
	istringstream l5kStream(l5k);
	l5k.clear();

	
	while (getline(l5kStream, x))
	{
		if ((x.find(" : " + tagType) != string::npos) && (x.find(stringIn) != string::npos)) //tag found in line
		{
			while (x.find("END_TAG") == string::npos) {
				l5k += x + "\n";
				getline(l5kStream, x);
			}

			if (stringIn.substr(stringIn.size() - 4, 4) == tagType) {
				l5k += stringIn + "P_Intlk OF " + stringIn.substr(0, stringIn.size() - tagType.size()) + "P_Intlk ;" + "\n";
				l5k += stringIn + "P_Perm OF " + stringIn.substr(0, stringIn.size() - tagType.size()) + "P_Perm ;" + "\n";
				l5k += x + "\n";
			}
			else {
				l5k += stringIn + "_Intlk OF " + stringIn + "_P_INTLK ;" + "\n";
				l5k += stringIn + "_Perm OF " + stringIn + "_P_PERM ;" + "\n";
				l5k += x + "\n";
			}

		}
		else
			l5k += x + "\n";
	}


}

