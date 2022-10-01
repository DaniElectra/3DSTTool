# 3DSTTool  
![GitHub](https://img.shields.io/github/license/DaniElectra/3DSTTool?style=for-the-badge) ![GitHub release (latest by date)](https://img.shields.io/github/v/release/DaniElectra/3DSTTool?style=for-the-badge)  
3DSTTool is a command-line tool for managing 3DST files from the Nintendo Anime Channel application.  

## Features  

- **Encode** a given image to a 3DST file:  
```
./3DSTTool encode

  -w, --width        Specify the width of the output image. Must be a power of 2.

  -h, --height       Specify the height of the output image. Must be a power of 2.

  -f, --format       (Default: rgba8) The format type to use when saving.
  Available options: rgba8, rgb8, a8, rgba4

  -y, --flip         Flip the image when saving.

  --help             Display this help screen.

  --version          Display version information.

  input (pos. 0)     Required. The image file to convert.

  output (pos. 1)    Required. The 3DST file save location.
```  
- **Decode** a given 3DST file to an image:  
```
./3DSTTool decode

  -w, --width        Optional: Specify the width of the output image.

  -h, --height       Optional: Specify the height of the output image.

  -f, --format       (Default: png) The image format to use when saving.
  Available options: bmp, emf, exif, gif, ico / icon, png, jpg / jpeg, tiff, wmf

  -y, --flip         Flip the image when saving.

  --help             Display this help screen.

  --version          Display version information.

  input (pos. 0)     Required. The 3DST file to convert.

  output (pos. 1)    Required. The image file save location.
```  

### To-do list  
The project is in a **very early** state, since it's my first time coding in C# from scratch. There are some pending features that I want to add:  

- Support for more 3DST image formats:  

|          | Is supported? |
|----------|:-------------:|
| RGBA8    |       ✅       |
| RGB8     |       ✅       |
| A8       |       ✅       |
| ETC1     |       ❌       |
| RGBA5551 |       ❌       |
| RGBA565  |       ❌       |
| RGBA4    |       ✅       | 

- Write the documentation  
- Test compatibility on Linux
- Support for multiple images at once (multithreading?)

## Build  
To build the tool, you will need the following NuGet packages:  

- [CommandLineParser](https://www.nuget.org/packages/CommandLineParser) 2.9.1  
- [System.Drawing.Common](https://www.nuget.org/packages/System.Drawing.Common) 6.0.0  
- [System.Memory.Data](https://www.nuget.org/packages/System.Memory.Data) 6.0.0

## License  
This project can licensed under the [LGPL-3.0](LICENSE) license or any later version.  

## Credits  
Credits to:  

- The [3DS Apps Revival Team](https://discord.gg/2nCGTHSV9e) for making the initial investigation about the inner workings of Nintendo Anime Channel.  
- [gdkchan](https://github.com/gdkchan) and the [Ohana3DS-Rebirth](https://github.com/gdkchan/Ohana3DS-Rebirth) project for some code regarding the RGBA8 color format.  
