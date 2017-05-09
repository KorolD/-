#ifndef StackH                 //вот это не даёт повторно включить .h файл
#define StackH Stack           //пусть остаётся, хотя повторных включений и нет

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

class ItemStack{  //стэк для каждого хэш-адреса
	public:
	ItemStack();
	~ItemStack();
	struct Item* FindKey(int k);
	void DeleteKey(int key);
	void Add(int key, String data);
	String Show();//возвращает строку из данного стэка вида: ключ1-значение1, ключ2-зн2...
	int LessThanOrEq(double average);  //возвращает кол-во ключей <=average
	double Average(); //возвращает среднее по ключам из стжка
	bool IsEmpty();    //true - если в стэк пуст
	int GetAmount();  //возвращает кол-во ключей в данном стэке

	private:
	struct Item* stack_ptr;
};

#endif
