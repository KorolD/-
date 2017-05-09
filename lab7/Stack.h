#ifndef StackH                 //��� ��� �� ��� �������� �������� .h ����
#define StackH Stack           //����� �������, ���� ��������� ��������� � ���

#ifndef vclH
#define vclH vcl

#include <vcl.h>
#include <string>

#endif

struct Item{
	~Item(){
		data.~UnicodeString();
	}

	struct Item* next;
	int key;
	String data;
};

class ItemStack{  //���� ��� ������� ���-������
	public:
	ItemStack();
	~ItemStack();
	struct Item* FindKey(int k);
	void DeleteKey(int key);
	void Add(int key, String data);
	String Show();//���������� ������ �� ������� ����� ����: ����1-��������1, ����2-��2...
	int LessThanOrEq(double average);  //���������� ���-�� ������ <=average
	double Average(); //���������� ������� �� ������ �� �����
	bool IsEmpty();    //true - ���� � ���� ����
	int GetAmount();  //���������� ���-�� ������ � ������ �����

	private:
	struct Item* stack_ptr;
};

#endif
