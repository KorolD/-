#include "InheritantSheet.h"

	double InheritantSheet::Average(){
		int total_amount=GetAmountTotal();
		double average=0;
		for(int i=0; i<amount; i++){
			average+=double(stacks[i].Average()*stacks[i].GetAmount())/total_amount;
		}
		return average;
	}

	int InheritantSheet::GetAmountTotal(){
		int total_amount=0;
		for(int i=0; i<amount; i++){
			total_amount+=stacks[i].GetAmount();
		}
		return total_amount;
	}

	int InheritantSheet::LessThanOrEqual(double average){
		int k=0; //счётчик подходящих элементов
		for(int i=0; i<amount; i++){
			k+=stacks[i].LessThanOrEq(average);
		}
		return k;
	}
