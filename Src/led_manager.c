#include "led_manager.h"
#include "stm32f1xx_hal.h"


void my_led_on(unsigned char led)
{
  switch(led){
		case 1:HAL_GPIO_WritePin(GPIOA, GPIO_PIN_4, GPIO_PIN_SET);break;
		case 2:HAL_GPIO_WritePin(GPIOA, GPIO_PIN_5, GPIO_PIN_SET);break;
		case 3:HAL_GPIO_WritePin(GPIOA, GPIO_PIN_6, GPIO_PIN_SET);break;
	}
}
	
void my_led_off(unsigned char led)
{
  switch(led){
		case 1:HAL_GPIO_WritePin(GPIOA, GPIO_PIN_4, GPIO_PIN_RESET);break;
		case 2:HAL_GPIO_WritePin(GPIOA, GPIO_PIN_5, GPIO_PIN_RESET);break;
		case 3:HAL_GPIO_WritePin(GPIOA, GPIO_PIN_6, GPIO_PIN_RESET);break;
	}
}
//End LED Functions****************************************************************//
