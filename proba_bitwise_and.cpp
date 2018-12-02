#include "pch.h"
#include <iostream>
#include <string>
#include <fstream>

using namespace std;

static const string base64_chars =
"ABCDEFGHIJKLMNOPQRSTUVWXYZ"
"abcdefghijklmnopqrstuvwxyz"
"0123456789+/";

static inline bool is_base64(unsigned char c) {
	return (isalnum(c) || (c == '+') || (c == '/'));
}

string base64(unsigned char const* tekst, unsigned int dlugosc)
{
	string kod;
	int i = 0;
	int j = 0;
	unsigned char znak3[3];
	unsigned char znak4[4];

	while (dlugosc--)
	{
		znak3[i++] = *(tekst++);
		if (i == 3) {
			znak4[0] = (znak3[0] & 0xfc) >> 2;
			znak4[1] = ((znak3[0] & 0x03) << 4) + ((znak3[1] & 0xf0) >> 4);
			znak4[2] = ((znak3[1] & 0x0f) << 2) + ((znak3[2] & 0xc0) >> 6);
			znak4[3] = znak3[2] & 0x3f;

			for (i = 0; (i < 4); i++)
				kod += base64_chars[znak4[i]];
			i = 0;
		}
	}
	if (i)
	{
		for (j = i; j < 3; j++)
			znak3[j] = '\0';

		znak4[0] = (znak3[0] & 0xfc) >> 2;
		znak4[1] = ((znak3[0] & 0x03) << 4) + ((znak3[1] & 0xf0) >> 4);
		znak4[2] = ((znak3[1] & 0x0f) << 2) + ((znak3[2] & 0xc0) >> 6);
		znak4[3] = znak3[2] & 0x3f;

		for (j = 0; (j < i + 1); j++)
			kod += base64_chars[znak4[j]];

		while ((i++ < 3))
			kod += '=';

	}
	return kod;
}

string base64_decode(string const& kod) {
	int in_len = kod.size();
	int i = 0;
	int j = 0;
	int in_ = 0;
	unsigned char znak4[4], znak3[3];
	string tekst;

	while (in_len-- && (kod[in_] != '=') && is_base64(kod[in_])) {
		znak4[i++] = kod[in_]; in_++;
		if (i == 4) {
			for (i = 0; i < 4; i++)
				znak4[i] = base64_chars.find(znak4[i]);

			znak3[0] = (znak4[0] << 2) + ((znak4[1] & 0x30) >> 4);
			znak3[1] = ((znak4[1] & 0xf) << 4) + ((znak4[2] & 0x3c) >> 2);
			znak3[2] = ((znak4[2] & 0x3) << 6) + znak4[3];

			for (i = 0; (i < 3); i++)
				tekst += znak3[i];
			i = 0;
		}
	}

	if (i) {
		for (j = i; j < 4; j++)
			znak4[j] = 0;

		for (j = 0; j < 4; j++)
			znak4[j] = base64_chars.find(znak4[j]);

		znak3[0] = (znak4[0] << 2) + ((znak4[1] & 0x30) >> 4);
		znak3[1] = ((znak4[1] & 0xf) << 4) + ((znak4[2] & 0x3c) >> 2);
		znak3[2] = ((znak4[2] & 0x3) << 6) + znak4[3];

		for (j = 0; (j < i - 1); j++) tekst += znak3[j];
	}

	return tekst;
}

int main()
{
	int tryb;
	cout << "Wybierz 1 aby zakodować plik\nWybierz 2, aby zdekodować plik\n";
	cin >> tryb;

		if (tryb == 1)
		{
			string stegka;
			cout << "Podaj pełną ścieżkę pliku do zakodowania\n";
			cin.ignore();
			getline(cin, stegka);
			ifstream myfile(stegka, ios_base::binary);
			if (myfile.is_open())
			{
				string do_zakodowania;
				do_zakodowania.assign((istreambuf_iterator<char>(myfile)),
					istreambuf_iterator<char>());
				string zakodowane = base64(reinterpret_cast<const unsigned char*>(do_zakodowania.c_str()),
					do_zakodowania.length());
				ofstream zapis("output.b64");
				zapis << zakodowane;
				cout << "Zakodowany plik output.b64 został zapisany do folderu, w którym znajduje się aplikacja.";
			}
			else
			{
				cout << "Nieprawidłowa ścieżka pliku. Aplikacja zostanie zamknięta.";
				return;
			}
		}
		else if (tryb == 2)
		{
			string stegka;
			cout << "Podaj pełną ścieżkę pliku do zakodowania\n";
			cin.ignore();
			getline(cin, stegka);
			ifstream myfile(stegka, ios_base::binary);
			if (myfile.is_open())
			{
				string do_odkodowania;
				do_odkodowania.assign((istreambuf_iterator<char>(myfile)),
					istreambuf_iterator<char>());
				string odkodowane = base64_decode(do_odkodowania.c_str());
				cout << odkodowane;
				ofstream zapis("output.txt");
				zapis << odkodowane;
				cout << "Odkodowany plik output.txt został zapisany do folderu, w którym znajduje się aplikacja.";
			}
			else
			{
				cout << "Nieprawidłowa ścieżka pliku. Aplikacja zostanie zamknięta.";
				return;
			}
		}
		else
		{
			return 0;
		}

}