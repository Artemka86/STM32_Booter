#include "stm32f1xx_hal.h"
#include "can.h"
#include "led.h"

CAN_HandleTypeDef hcan1;
CAN_FilterConfTypeDef filter;
uint32_t prescaler;
enum can_bus_state bus_state;

void can_init(void) {
    // default to 125 kbit/s
    //prescaler = 36;  //For 36MHZ  APB1
    hcan1.Instance = CAN1;
    bus_state = OFF_BUS;
}

void can_set_filter(uint32_t id, uint32_t mask) {
    // see page 825 of RM0091 for details on filters
    // set the standard ID part
    filter.FilterIdHigh = (id & 0x7FF) << 5;
    // add the top 5 bits of the extended ID
    filter.FilterIdHigh += (id >> 24) & 0xFFFF;
    // set the low part to the remaining extended ID bits
    filter.FilterIdLow += ((id & 0x1FFFF800) << 3);

    // set the standard ID part
    filter.FilterMaskIdHigh = (mask & 0x7FF) << 5;
    // add the top 5 bits of the extended ID
    filter.FilterMaskIdHigh += (mask >> 24) & 0xFFFF;
    // set the low part to the remaining extended ID bits
    filter.FilterMaskIdLow += ((mask & 0x1FFFF800) << 3);

    filter.FilterMode = CAN_FILTERMODE_IDMASK;
    filter.FilterScale = CAN_FILTERSCALE_32BIT;
    filter.FilterNumber = 0;
    filter.FilterFIFOAssignment = CAN_FIFO0;
    filter.BankNumber = 0;
    filter.FilterActivation = ENABLE;

    //if (bus_state == ON_BUS) {
	HAL_CAN_ConfigFilter(&hcan1, &filter);
				//HAL_CAN_Receive_IT(&hcan1,CAN_FIFO0);
   // }
}


void can_test_send(void){
				 hcan1.pTxMsg->StdId=0x1F1;
     //Set DLC 
		 hcan1.pTxMsg->DLC =8;
     //Set data bytes
		 hcan1.pTxMsg->Data[0]=0x00;
     hcan1.pTxMsg->Data[1]=0xAA;
     hcan1.pTxMsg->Data[2]=0xAA;
     hcan1.pTxMsg->Data[3]=0xFF;
     hcan1.pTxMsg->Data[4]=0xFF;
     hcan1.pTxMsg->Data[5]=0x00;
     hcan1.pTxMsg->Data[6]=0x00;
     hcan1.pTxMsg->Data[7]=0x00;
	//while (1)
  //{
     //Set packet ID 

     //Start transmission
     HAL_CAN_Transmit(&hcan1, 100);
		 HAL_Delay(300);


		//}
	
	
}



void can_enable(void) {
    if (bus_state == OFF_BUS) {
	hcan1.Init.Prescaler = 6;//prescaler;
	hcan1.Init.Mode = CAN_MODE_NORMAL;
	hcan1.Init.SJW = CAN_SJW_1TQ;
	hcan1.Init.BS1 = CAN_BS1_6TQ;
	hcan1.Init.BS2 = CAN_BS2_5TQ;
	hcan1.Init.TTCM = DISABLE;
	hcan1.Init.ABOM = DISABLE;
	hcan1.Init.AWUM = DISABLE;
	hcan1.Init.NART = DISABLE;
	hcan1.Init.RFLM = DISABLE;
	hcan1.Init.TXFP = DISABLE;
        hcan1.pTxMsg = NULL;
        HAL_CAN_Init(&hcan1);
        bus_state = ON_BUS;
	can_set_filter(0, 0);
    }

	__HAL_CAN_FIFO_RELEASE(&hcan1,CAN_FilterFIFO0);
		//HAL_CAN_Receive_IT(&hcan1,CAN_FIFO0);
		
		
}

void can_disable(void) {
    if (bus_state == ON_BUS) {
        // do a bxCAN reset (set RESET bit to 1)
        hcan1.Instance->MCR |= CAN_MCR_RESET;
        bus_state = OFF_BUS;
    }
    //HAL_GPIO_WritePin(GPIOB, GPIO_PIN_1, GPIO_PIN_RESET);
}

void can_set_bitrate(enum can_bitrate bitrate) {
    if (bus_state == ON_BUS) {
        // cannot set bitrate while on bus
        return;
    }

    switch (bitrate) {
    case CAN_BITRATE_10K:
	prescaler = 600;
        break;
    case CAN_BITRATE_20K:
	prescaler = 300;
        break;
    case CAN_BITRATE_50K:
	prescaler = 120;
        break;
    case CAN_BITRATE_100K:
        prescaler = 60;
        break;
    case CAN_BITRATE_125K:
        prescaler = 48;
        break;
    case CAN_BITRATE_250K:
        prescaler = 24;
        break;
    case CAN_BITRATE_500K:
        prescaler = 12;
        break;
    case CAN_BITRATE_750K:
        prescaler = 8;
        break;
    case CAN_BITRATE_1000K:
        prescaler = 6;
        break;
    }
}

void can_set_silent(uint8_t silent) {
    if (bus_state == ON_BUS) {
        // cannot set silent mode while on bus
        return;
    }
    if (silent) {
        hcan1.Init.Mode = CAN_MODE_SILENT;
    } else {
        hcan1.Init.Mode = CAN_MODE_NORMAL;
    }
}

uint32_t can_tx(CanTxMsgTypeDef *tx_msg, uint32_t timeout) {
    uint32_t status;

    // transmit can frame
    hcan1.pTxMsg = tx_msg;
	status=0;
    status = HAL_CAN_Transmit(&hcan1, timeout);
    if(status!=0)HAL_CAN_Init(&hcan1);
	
	//HAL_CAN_FIFO_RELEASE(&hcan,CAN_FilterFIFO0);
	__HAL_CAN_FIFO_RELEASE(&hcan1,CAN_FilterFIFO0);
	//led_on();
    return 0;//status;
}

uint32_t can_rx(CanRxMsgTypeDef *rx_msg, uint32_t timeout) {
    uint32_t status;

    hcan1.pRxMsg = rx_msg;

    status = HAL_CAN_Receive(&hcan1, CAN_FIFO0, timeout);
    
	//led_on();
    return status;
}

uint8_t is_can_msg_pending(uint8_t fifo) {
    if (bus_state == OFF_BUS) {
        return 0;
    }
    return (__HAL_CAN_MSG_PENDING(&hcan1, fifo) > 0);
}
