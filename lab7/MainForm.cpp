#define FLOOR(a) (double(int((a)*10000+0.5))/10000)
						//при приведении double к int отсекается дробная часть
/*т.к. про уникальность ключей никто ничего не говорил, эта проверка отсутсвует.
  Но, если встретятся несколько одинаковых ключей, все операции будут проходить с вершиной стека с одинаковым ключом
  Таким образом "Это не баг, это фича"
  Или вообще дополнительная функциональность
*/
#pragma hdrstop

#include "MainForm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm1 *Form1;
class InheritantSheet* sheet=NULL;

bool IsInt(String s){ //true - если s состоит только из цифр и возможно '-' первым символом
	if(!s.Length()){
		return false;
	}
	for(int i=1; i<=s.Length(); i++){
		if(s[i]>'9'||s[i]<'0'){
			if(i==1&&s[1]=='-'){
				continue;
			}else{
				return false;
			}
		}
	}
	return true;
}
void ShowSheet(){   //выводит всю таблицу в мемо1
	Form1->Memo1->Lines->Clear();
	if(sheet){
		for(int i=0; i<sheet->GetLinesAmount();i++){
			Form1->Memo1->Lines->Add(sheet->ShowNum(i));
		}
	}
	return;
}
//---------------------------------------------------------------------------
__fastcall TForm1::TForm1(TComponent* Owner)
	: TForm(Owner)
{
	StringGrid1->Cells[0][0]="Ключи";
	StringGrid1->Cells[1][0]="Значения";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::Button1Click(TObject *Sender) //Заполнить
{
	int amount=1; //кол-во элементов
	while(IsInt(StringGrid1->Cells[0][amount])&&amount<100){
		amount++;
	}
	amount--;
	if(!amount){  //если нечем заполнять
		ShowSheet();
		return;
	}
	if(sheet){  //удаляет старую таблицу
		sheet->~InheritantSheet();
	}
	sheet=new InheritantSheet(amount); //кол-во адресов=ков-ву элементов, это делает работу
	for(int i=1; i<=amount; i++){      //с таблицей более наглядной
		sheet->Add(StringGrid1->Cells[0][i].ToInt(),StringGrid1->Cells[1][i]);
	}
	ShowSheet();
	return;
}
//---------------------------------------------------------------------------
void __fastcall TForm1::Button3Click(TObject *Sender) //Добавить
{
	if(IsInt(Edit1->Text)){
		if(!sheet){ //если таблицы ещё нет, то создадим таблицу с 29 адресами
			sheet=new InheritantSheet(29);//29-простое число, и потому слуяайные ключи
		}                                 //прекрасно распределятся по таблице
		sheet->Add(Edit1->Text.ToInt(),Edit2->Text);
	}else{
		ShowMessage("Введите целый ключ");
	}
	ShowSheet();
	return;
}
//---------------------------------------------------------------------------
void __fastcall TForm1::Button2Click(TObject *Sender)//Удалить
{
	if(IsInt(Edit1->Text)){
		if(sheet){
			sheet->DeleteKey(Edit1->Text.ToInt());
		}else{
			ShowMessage("Таблица пуста");
			return;
        }
		if(!sheet->GetAmountTotal()){
			sheet->~InheritantSheet();
			sheet=NULL;
		}
	}else{
		ShowMessage("Введите целый ключ");
	}
	ShowSheet();
	return;
}
//---------------------------------------------------------------------------
void __fastcall TForm1::Button4Click(TObject *Sender) //Найти
{
	if(IsInt(Edit1->Text)){
		if(sheet){
			String* temp=sheet->FindKey(Edit1->Text.ToInt());
			if(temp){ //может вернуться NULL, если ключ не найден
				Memo1->Lines->Clear();
				Memo1->Lines->Add("Ключ:");
				Memo1->Lines->Add(Edit1->Text);
				Memo1->Lines->Add("Значение:");
				Memo1->Lines->Add(*temp);//здесь используется указатель, на поле элемента таблицы,
			}else{                       //которое будет освобождено позже
				Memo1->Lines->Clear();
				Memo1->Lines->Add("Ключ не найден");
            }
		}else{
			ShowMessage("Таблица пуста");
        }
	}else{
		ShowMessage("Введите целый ключ");
	}
	return;
}
//---------------------------------------------------------------------------
void __fastcall TForm1::Button5Click(TObject *Sender)//ключи <= среднего
{
	if(sheet){
		if(!sheet->GetAmountTotal()){
			ShowMessage("Таблица пуста");
		}else{
			double average=sheet->Average();
			Memo1->Lines->Clear();
			Memo1->Lines->Add("Среднее арифметическое по ключам:");
			Memo1->Lines->Add(FloatToStr(FLOOR(average))); //округление макросом до 4 цифры после ,
			Memo1->Lines->Add("Количество элементов с ключами меньше среднего либо равных ему:");
			Memo1->Lines->Add(IntToStr(sheet->LessThanOrEqual(average)));
        }
	}else{
		ShowMessage("Таблица пуста");
	}
	return;
}
//---------------------------------------------------------------------------
void __fastcall TForm1::Button7Click(TObject *Sender)//Выход
{
	if(sheet){
		sheet->~InheritantSheet();
	}
	Close();
}
//---------------------------------------------------------------------------
void __fastcall TForm1::Button6Click(TObject *Sender)//Показать таблицу
{
	ShowSheet();//именно эта функция показывает таблицу, и её тело могло бы быть здесь
	return;     //но ShowSheet() >>>> Button6Click(Sender)
}               //или даже       >>>> ButtonShowSheetClick(Sender)
//---------------------------------------------------------------------------
