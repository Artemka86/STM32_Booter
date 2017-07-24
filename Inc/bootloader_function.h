int hexint(char input_symbol);
char inthex(unsigned char a);
void RTEA_decoder(void);
void flash_erase_page(unsigned int flash_page);
void Internal_Flash_Write(unsigned char* data, unsigned int address, unsigned int count);
void flash_write_page(unsigned int flash_page);
void RAM_buffer_fill(void);
void set_page(unsigned char page);
void operation_manager(char operation);

