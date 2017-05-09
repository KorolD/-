#ifndef InheritantSheetH
#define InheritantSheetH InheritantSheet

#include "BaseSheet.h"

class InheritantSheet:public BaseSheet{
	public:
	InheritantSheet(int amount):BaseSheet(amount){}
	double Average(); //среднее по всем ключам таблицы
	int GetAmountTotal(); //возвращает колво элементов в таблице
	int LessThanOrEqual(double average);//возвращает кол-во ключей <=average во всей таблице
};

#endif
