#ifndef InheritantSheetH
#define InheritantSheetH InheritantSheet

#include "BaseSheet.h"

class InheritantSheet:public BaseSheet{
	public:
	InheritantSheet(int amount):BaseSheet(amount){}
	double Average(); //������� �� ���� ������ �������
	int GetAmountTotal(); //���������� ����� ��������� � �������
	int LessThanOrEqual(double average);//���������� ���-�� ������ <=average �� ���� �������
};

#endif
