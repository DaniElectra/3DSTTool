# 3DSTTool  
![GitHub](https://img.shields.io/github/license/DaniElectra/3DSTTool?style=for-the-badge) ![GitHub release (latest by date)](https://img.shields.io/github/v/release/DaniElectra/3DSTTool?style=for-the-badge)  
3DSTTool is a command-line tool for managing 3DST files from the Nintendo Anime Channel application.  

## Features  

- **Encode** a given image to a 3DST file:  
```
./3DSTTool encode

  -o, --output      The 3DST file save location.

  -w, --width       Specify the width of the output image(s). Must be a power of 2.

  -h, --height      Specify the height of the output image(s). Must be a power of 2.

  -f, --format      (Default: rgba8) The format type to use when saving.
  Available options: rgba8, rgb8, a8, rgba5551, rgb565, rgba4

  -y, --flip        Flip the image(s) when saving.

  --help            Display this help screen.

  --version         Display version information.

  input (pos. 0)    Required. The image file(s) to convert.
```  
- **Decode** a given 3DST file to an image:  
```
./3DSTTool decode

  -o, --output      The image file save location.

  -w, --width       Specify the width of the output image(s).

  -h, --height      Specify the height of the output image(s).

  -f, --format      (Default: png) The image format to use when saving.
  Available options: astc, avif, bmp, dng, gif, heif, ico / icon, jpg / jpeg, ktx, pkm, png, wbmp, webp

  -y, --flip        Flip the image(s) when saving.

  --help            Display this help screen.

  --version         Display version information.

  input (pos. 0)    Required. The 3DST file(s) to convert.
```  

### To-do list  
Although the project is *almost* ready for final release, there are some missing features that I want to add before v1.0:  

- Support for all 3DST image formats:  

|          | Is supported? |
|----------|:-------------:|
| RGBA8    |       ✅       |
| RGB8     |       ✅       |
| A8       |       ✅       |
| ETC1     |       ❌       |
| RGBA5551 |       ✅       |
| RGB565   |       ✅       |
| RGBA4    |       ✅       | 

- Write proper documentation (not just the help information)  


## Building  
To build the tool, you will need .NET Core 3.1 and the following NuGet packages:  

- [CommandLineParser](https://www.nuget.org/packages/CommandLineParser) 2.9.1+  
- [SkiaSharp](https://www.nuget.org/packages/SkiaSharp) 2.88.3+  
- [System.Memory.Data](https://www.nuget.org/packages/System.Memory.Data) 6.0.0+

## Note for Linux users  
To use this tool under Linux, you will need to install the proper package for your distro. You can learn more about this issue [here](https://github.com/mono/SkiaSharp/wiki/SkiaSharp-Native-Assets-for-Linux).  

### The generic solution  
You can try adding the following NuGet package as a dependency to the project: [SkiaSharp.NativeAssets.Linux](https://www.nuget.org/packages/SkiaSharp.NativeAssets.Linux).  

### For Arch-based distros  
In some distributions like Arch Linux, you can install the SkiaSharp library as a system package:  
```shell
sudo pacman -S skia-sharp
```

## License  
This project can licensed under the [LGPL-3.0](LICENSE) license or any later version.  

## Credits  
Credits to:  

- The [3DS Apps Revival Team](https://discord.gg/2nCGTHSV9e) for making the initial investigation about the inner workings of Nintendo Anime Channel.  
- [gdkchan](https://github.com/gdkchan) and the [Ohana3DS-Rebirth](https://github.com/gdkchan/Ohana3DS-Rebirth) project for some code regarding the various color formats.  
