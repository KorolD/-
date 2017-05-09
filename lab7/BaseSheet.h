#ifndef BaseSheetH
#define BaseSheetH BaseSheet

#include "Stack.h"

class BaseSheet{
	public:
	BaseSheet(int Amount);
	~BaseSheet();
	void Add(int key, String data);
	String* FindKey(int key);
	void DeleteKey(int key);
	String ShowNum(int num);
	int GetLinesAmount();    //возвращает количество хэш-адресов

	protected:
	int amount;    //количество хэш-адресов
	class ItemStack* stacks;
};

#endif
