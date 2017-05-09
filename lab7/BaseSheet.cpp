#include "BaseSheet.h"
#define ABS(a) (a >= 0)?(a):(-(a))  //модуль числа без include<math>

	BaseSheet::BaseSheet(int Amount){
		amount = Amount;
		stacks = new ItemStack[amount];
		return;
	}

	BaseSheet::~BaseSheet(){
		for(int i = 0; i < amount; i++){
			stacks[i].~ItemStack();
		}
		delete stacks;
		return;
	}

	void BaseSheet::Add(int key, String data){
		stacks[ABS(key % amount)].Add(key, data);//здесь остаток от деления бывает отрицательным
		return;
	}

	String* BaseSheet::FindKey(int key){
		struct Item* temp=stacks[ABS(key % amount)].FindKey(key);
		if(temp){
			return &(temp -> data); //т.к. возвращаем указатель
		}else{
			return NULL;
		}
	}

	void BaseSheet::DeleteKey(int key){
		stacks[ABS(key % amount)].DeleteKey(key);
		return;
	}

	String BaseSheet::ShowNum(int num){//получаем хэш-адрес: кл1-зн1, кл2-зн2, кл3-зн3 ...
		return (IntToStr(num) + ": " + stacks[num].Show());
	}

	int BaseSheet::GetLinesAmount(){
		return amount;
	}

