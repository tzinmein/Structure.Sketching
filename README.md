# Structure.Sketching

[![Build Status](https://github.com/tzinmein/Structure.Sketching/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tzinmein/Structure.Sketching/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

Structure.Sketching is an image processing library targeting the .NET LTS, currently 8.0. It is in beta. The namespaces may be adjusted as it is cleaned up for production, but otherwise it should be fairly safe to use.

## Supported Formats/Filters

Currently the library supports the following file types:

* JPG
* BMP - Reading: 32bit, 24bit, 16bit, 8bit, 8bit RLE, 4bit, and 1bit. Writing: 24bit
* PNG - Reading: RGB, RGBA, Greyscale, Greysale+alpha, Palette. Writing: RGBA
* GIF - Includes animation support

There are also a number of filters within the library for a number of different purposes:

- Contrast stretching
- Gamma correction
- HSV and RGB equalization
- Adaptive HSV and RGB equalization
- Kuwahara smoothing
- Median smoothing
- SNN Blur
- Image resizing
- Image cropping
- Canvas resizing
- Image flipping
- Affine transformations including:
  - Rotatation
  - Scaling
  - Skewing
  - Translatation
- Image resizing and affine transformations can use the following resampling filters:
  - Bell
  - Bicubic
  - Bilinear
  - Box
  - Catmull Rom
  - Cosine
  - Cubic B Spline
  - Cubic Convolution
  - Hermite
  - Lanczos3
  - Lanczos8
  - Mitchell
  - Nearest Neighbor
  - Quadratic B Spline
  - Quadratic
  - Robidoux
  - Robidoux Sharp
  - Robidoux Soft
  - Triangle
- Bump map generation
- Canny Edge Detection
- Gaussian Blur
- Lomograph
- Normal map generation
- Polaroid
- Image blending
- Glow
- Vignette
- Constrict
- Dilate
- Unsharp
- Box Blur
- Embossing
- Edge detection techniques including:
  - Kayyali
  - Kirsh
  - Laplace Edge Detection
  - Laplacian of Gaussian Edge Detection
  - Prewitt
  - Roberts Cross
  - Robinson
  - Scharr
- Sharpen
- Sharpen Less
- Sobel Embossing
- Alpha manipulation
- Black and White
- Blue, green, and red filters
- Brigtness manipulation
- Contrast manipulation
- Greyscale 601 and 709
- Hue manipulation
- Kodachrome
- Saturation
- Sepia Tone
- Temperature
- Color blindness filters including:
	- Achromatomaly
	- Achromatopsia
	- Deuteranomaly
	- Deuteranopia
	- Protanomaly
	- Protanopia
	- Tritanomaly
	- Tritanopia
- Adaptive Threshold
- Non Maximal Suppression
- Threshold
- Image addition, subtraction, division, multiplication, modulo, and, or, and xor functions.
- Turbulence
- Solarize
- Sin Wave
- Posterize
- Pointillism
- Pixellate
- Noise
- Logarithm
- Jitter
- Color replacement
- Color inversion
- There are also generic classes for color matrix (using a 5x5 matrix), affine transformations, and convolution filters.

There are also a couple of other items in here including:

- Perlin Noise generation
- The library also includes the ability to draw lines, rectangles, and ellipses, both filled and the outline.
- Image to ASCII art
- Image to Base64 string

That said hopefully the list will grow with time.

## Usage

The library is straightforward to use:

```csharp
new Image("ExampleImage.jpg")
	.Apply(new CannyEdgeDetection(Color.Black, Color.White, .9f, .1f))
	.Save("ExampleImage2.jpg");
```

The `Image` class is designed with a fluent interface, offering flexibility in loading images. You can provide the source as a string pointing to a file, a stream, or a byte array. Additionally, you can specify the dimensions (width and height) when creating a blank image.

The `Apply` function allows you to apply a filter to the image. You have the option to define a rectangle as the second parameter, restricting the filter's effect to a specific portion of the image.

The `Save` function offers flexibility in saving images. You have the option to either specify a file name or a stream. Additionally, you can define the desired file format using an enum, allowing you to save the image in your preferred format.

For wiki documentation... *akan datang* (coming soon).

## Installation

To install it run the following command in the Package Manager Console:

```pwsh
Install-Package Tz.Structure.Sketching
```

## Credits

Originally by [James Craig](https://github.com/JaCraig/Structure.Sketching)

## License

Structure.Sketching is released under the [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0.txt) license.
