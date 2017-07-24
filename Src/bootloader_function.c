#include "stm32f1xx_hal.h"
#include "can.h"
#include "bootloader_function.h"
# include "usbd_cdc_if.h"


extern char command_buffer[50];
extern unsigned char RAM_buff[2048];
extern unsigned char page_for_writing;
extern uint16_t page_offset;


//****************************************************************************//
// ASCII char convert to int (input HEX char '0'...'9'..'A'..'F')
//****************************************************************************//
int hexint(char input_symbol)
        {
            switch (input_symbol)
            {
                case '0':
                    return 0;
                    break;
                case '1':
                    return 1;
                    break;
                case '2':
                    return 2;
                    break;
                case '3':
                    return 3;
                    break;
                case '4':
                    return 4;
                    break;
                case '5':
                    return 5;
                    break;
                case '6':
                    return 6;
                    break;
                case '7':
                    return 7;
                    break;
                case '8':
                    return 8;
                    break;
                case '9':
                    return 9;
                    break;
                case 'A':
                    return 10;
                    break;
                case 'B':
                    return 11;
                    break;
                case 'C':
                    return 12;
                    break;
                case 'D':
                    return 13;
                    break;
                case 'E':
                    return 14;
                    break;
                case 'F':
                    return 15;
                    break;
            }

            return 0;
        }
//***********************************************************************************************//
// END  hexint
//***********************************************************************************************//				

				
//***********************************************************************************************//
//convert half-byte to ASCII char (input:0....F)
//***********************************************************************************************//				
char inthex(unsigned char a){
	
	switch (a){
		case 0:
			return '0';
	    break;
    case 1:
			return '1';
		  break;
		case 2:
			return '2';
		  break;
		case 3:
			return '3';
		  break;
		case 4:
			return '4';
		  break;
		case 5:
			return '5';
		  break;
		case 6:
			return '6';
	    break;
    case 7:
			return '7';
		  break;
		case 8:
			return '8';
		  break;
		case 9:
			return '9';
		  break;
		case 0x0A:
			return 'A';
		  break;
		case 0x0b:
			return 'B';
		  break;	
			case 0x0C:
			return 'C';
		  break;
		case 0x0D:
			return 'D';
		  break;
		case 0x0E:
			return 'E';
		  break;
		case 0x0F:
			return 'F';
		  break;
	
	
	}
	
return 0;	
}
//**********************************************************************************//
//END inthex
//**********************************************************************************//

//**********************************************************************************//
//            END OF HEX Converter
//**********************************************************************************//



//***********************************************************************************************//
//   RTEA
//***********************************************************************************************//

void RTEA_decoder(void)
{
	uint32_t a, b, kw;
  static uint32_t key[4];
  long r;
	int i,j;
	kw=4;
	          //keys
	          key[0] = 0xA321456C;
            key[1] = 0x67AFCDB0;
            key[2] = 0xF9A8C5E6;
            key[3] = 0xC13EA371;
	          //key[4] = 0xC13EA371;
	
	
r=10;	
j=0;
i=0;	
	
	
	for(j=0;j<256;j++){
	
		
		a=RAM_buff[i+3]*0x1000000+RAM_buff[i+2]*0x10000+RAM_buff[i+1]*0x100+RAM_buff[i];
		b=RAM_buff[i+7]*0x1000000+RAM_buff[i+6]*0x10000+RAM_buff[i+5]*0x100+RAM_buff[i+4];
		
		
		for (r=kw*4+31;r!=-1;r--){

			b-=(a+((a<<6)^(a>>8))+key[r%kw]+r);
			r--;
			a-=b+((b<<6)^(b>>8))+key[r%kw]+r;
		}	
	
		RAM_buff[i+3]=a/0x1000000;
		RAM_buff[i+2]=(a%0x1000000)/0x10000;
	  RAM_buff[i+1]=(a%0x10000)/0x100;
		RAM_buff[i]=a%0x100;
	  
		RAM_buff[i+7]=b/0x1000000;
		RAM_buff[i+6]=(b%0x1000000)/0x10000;
	  RAM_buff[i+5]=(b%0x10000)/0x100;
		RAM_buff[i+4]=b%0x100;
	
	i=i+8;
	}
	
	
	
}
//************************************************************************************************//
// END RTEA
//************************************************************************************************//




//*************************************************************************************************//
// ERASE FLASH Page N
//*************************************************************************************************//
void flash_erase_page(unsigned int flash_page){

	
	FLASH_EraseInitTypeDef EraseInitStruct;
	uint32_t AddressDes=0x08000000;
	uint32_t PageError=0;

	HAL_FLASH_Unlock();
	EraseInitStruct.TypeErase=TYPEERASE_PAGES;
  EraseInitStruct.PageAddress=AddressDes+flash_page*0x800;
  EraseInitStruct.NbPages=1;
	
  if(HAL_FLASHEx_Erase(&EraseInitStruct,&PageError)!=HAL_OK){
		HAL_FLASH_Lock();
	return;
	}	
}
//****************************************************************************************************//
//  END ERASE FLASH Page N
//****************************************************************************************************//

//*****************************************************************************************************//
// Low level FLASH writing. Without HAL
//See: https://habrahabr.ru/post/213771/
//  EighthMayer(c)
//*****************************************************************************************************//
void Internal_Flash_Write(unsigned char* data, unsigned int address, unsigned int count) {
	unsigned int i;

	while (FLASH->SR & FLASH_SR_BSY);
	if (FLASH->SR & FLASH_SR_EOP) {
		FLASH->SR = FLASH_SR_EOP;
	}

	FLASH->CR |= FLASH_CR_PG;

	for (i = 0; i < count; i += 2) {
		*(volatile unsigned short*)(address + i) = (((unsigned short)data[i + 1]) << 8) + data[i];
		while (!(FLASH->SR & FLASH_SR_EOP));
		FLASH->SR = FLASH_SR_EOP;
	}

	FLASH->CR &= ~(FLASH_CR_PG);
}
//******************************************************************************************************//
// End Low Level FLASH Writing
//*****************************************************************************************************//

//*************************************************************************************************//
// WRITE FLASH Page N
//*************************************************************************************************//
void flash_write_page(unsigned int flash_page){
	
	uint32_t AddressDes=0x08000000,offset;//ADDR_FLASH_PAGE_1;
  unsigned char usb_tx_message_write_ok[20]={"Y"};

		offset=flash_page;
		offset=offset*0x800;
		
		RTEA_decoder();
    Internal_Flash_Write(RAM_buff,(AddressDes+offset),0x800);
    HAL_Delay(20);
	  CDC_Transmit_FS(usb_tx_message_write_ok,1);
	
	
	
}
//****************************************************************************************************//
//  END WRITE FLASH Page N
//****************************************************************************************************//

//*************************************************************************************************//
// Fill RAM buffer
//*************************************************************************************************//
void RAM_buffer_fill(void){
unsigned char usb_tx_message_ram_string[3]={"Y"};
	

	RAM_buff[page_offset]=(hexint(command_buffer[0])*0x10)+hexint(command_buffer[1]);
	/*RAM_buff[page_offset+1]=(hexint(command_buffer[2])*0x10)+hexint(command_buffer[3]);
	RAM_buff[page_offset+2]=(hexint(command_buffer[4])*0x10)+hexint(command_buffer[5]);
	RAM_buff[page_offset+3]=(hexint(command_buffer[6])*0x10)+hexint(command_buffer[7]);
	RAM_buff[page_offset+4]=(hexint(command_buffer[8])*0x10)+hexint(command_buffer[9]);
	RAM_buff[page_offset+5]=(hexint(command_buffer[10])*0x10)+hexint(command_buffer[11]);
	RAM_buff[page_offset+6]=(hexint(command_buffer[12])*0x10)+hexint(command_buffer[13]);
	RAM_buff[page_offset+7]=(hexint(command_buffer[14])*0x10)+hexint(command_buffer[15]);*/
	
	page_offset=page_offset+1;//8
	if(page_offset==0x800)page_offset=0;
	CDC_Transmit_FS(usb_tx_message_ram_string,1);

	
}
//****************************************************************************************************//
//  END Fill RAM buffer
//****************************************************************************************************//



//****************************************************************************************************//
// Set Page for Writing
//****************************************************************************************************//
void set_page(unsigned char page){
//unsigned int i;
unsigned char usb_tx_message_ack[1]={"Y"};	
	page_for_writing=page;
	page_offset=0x00;
	flash_erase_page(page_for_writing);
	CDC_Transmit_FS(usb_tx_message_ack,1);	
}
//****************************************************************************************************//
// END Set page for Writing
//****************************************************************************************************//


//****************************************************************************************************//
// Operation Manager
//****************************************************************************************************//
#define EEPROM_TYPE command_buffer[0]
void operation_manager(char operation){
unsigned int page_number;
//HAL_Delay(3);

	
	
switch(operation){	
	
	case'E':
    page_number=atoi(command_buffer);
		flash_erase_page(page_number);
  break;
	
 	case'W':
	  page_number=atoi(command_buffer);
    flash_write_page(page_number);
  break;
	
	case'B':
    RAM_buffer_fill();
  break;

  case'S':
   page_number=atoi(command_buffer);
	 set_page(page_number);
  break;
 }//Close SWITCH

}

//****************************************************************************************************//
// END     Operation Manager
//****************************************************************************************************//

