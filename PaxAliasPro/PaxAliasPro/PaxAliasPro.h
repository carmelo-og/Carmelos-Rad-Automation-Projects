#pragma once
#ifndef PAXALIASPRO_H
#define PAXALIASPRO_H

#include <string>
#include <vector>
using namespace std;

class PaxAliasPro {
private:
	string l5k;
	vector<string> tagArr;
	vector<string> tagTypeArr;
public:
	PaxAliasPro(string filename);
	void getPTags();
	void addAlias(string const stringIn, string const tagType);
};

#endif