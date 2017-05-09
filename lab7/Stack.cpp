#include "Stack.h"

	ItemStack::ItemStack(){
		stack_ptr=NULL;
		return;
	}

	ItemStack::~ItemStack(){
		if(!stack_ptr){
			return;
		}
		struct Item* temp=stack_ptr;  //темп нужен т.к. стэк однонаправленный
		while(stack_ptr){
			stack_ptr=stack_ptr->next;
			delete temp;
			temp=stack_ptr;
		}
		delete temp;
		return;
	}

	bool ItemStack::IsEmpty(){
		if(stack_ptr){
			return true;
		}else{
			return false;
		}
	}

	struct Item* ItemStack::FindKey(int key){
		struct Item *temp=stack_ptr;
		while(temp){
			if(temp->key==key){
				return temp;
			}
			temp=temp->next;
		}
		return temp;   //если не находит ключ возвращает NULL
	}

	void ItemStack::DeleteKey(int key){
		if(!stack_ptr){
			return;
		}
		if(key==stack_ptr->key){ //это можно проверить ниже в while
			struct Item *temp=stack_ptr; //но тогда этот while не будет таким же понятным и простым
			stack_ptr=stack_ptr->next;
			delete temp;
			return;
		}
		struct Item *temp0=stack_ptr, *temp=stack_ptr->next;
		while(temp){
			if(temp->key==key){
				temp0->next=temp->next;
				delete temp;
				return;
			}
			temp0=temp;
			temp=temp->next;
		}
		return;
	}

	void ItemStack::Add(int key, String data){
		if(!stack_ptr){    //если стэк пуст, создаём его
			stack_ptr=new struct Item;
			stack_ptr->key=key;
			stack_ptr->data=data;
			stack_ptr->next=NULL;
			return;
		}
		struct Item* temp=new struct Item;//иначе, добавляем элемент в начало
		temp->next=stack_ptr;
		temp->key=key;
		temp->data=data;
		stack_ptr=temp;
		return;
	}

	String ItemStack::Show(){
		struct Item* temp=stack_ptr;
		String out="";
		while(temp){
			out+=IntToStr(temp->key)+"-"+temp->data+", ";
			temp=temp->next;
		}
		out.SetLength(out.Length()-2); //обрезаем 2 последних символа ", "
		return out;
	}

	int ItemStack::GetAmount(){
		int amount=0;
		struct Item* temp=stack_ptr;
		while(temp){
			amount++;
			temp=temp->next;
		}
		return amount;
	}

	double ItemStack::Average(){
		int num=GetAmount();
		double average=0;
		struct Item* temp=stack_ptr;
		while(temp){
			average+=double(temp->key)/num;
			temp=temp->next;
		}
		return average;
	}

	int ItemStack::LessThanOrEq(double average){
		struct Item* temp=stack_ptr;
		int amount=0;
		while(temp){
			if(temp->key<=average){
				amount++;
			}
			temp=temp->next;
		}
		return amount;
	}
