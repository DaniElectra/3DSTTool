# 3DSTTool  
3DSTTool is a command-line tool for managing 3DST files from the Nintendo Anime Channel application.  

## Features  

- **Encode** a given image to a 3DST file: `3DSTTool.exe encode input.png output.3dst`  
- **Decode** a given 3DST file to an image: `3DSTTool.exe decode input.3dst output.png`  

### To-do list  
The project is in a **very early** state, since it's my first time coding in C# from scratch. There are some pending features that I want to add:  

- Support for more 3DST image formats (it only supports RGBA8 currently)  
- Write the documentation  
- Test compatibility on Linux
- Support for multiple images at once (multithreading?)

## License  
This project can licensed under the [LGPL-3.0](LICENSE) license or any later version.  

## Credits  
Credits to:  

- The [3DS Apps Revival Team](https://discord.gg/2nCGTHSV9e) for making the initial investigation about the inner workings of Nintendo Anime Channel.  
- [gdkchan](https://github.com/gdkchan) and the [Ohana3DS-Rebirth](https://github.com/gdkchan/Ohana3DS-Rebirth) project for some code regarding the RGBA8 color format.  
