# WebLaunchPad
Show fancy colors on a Novation Launchpad MK2 via API 

## Idea
The basic idea of this project is to create an API that allows you to
set the colors on a Novation Launchpad MK2. You should also be able to
upload GIFs that are then being animated on the Launchpad.

## Progress
Heavily WIP.

## Structure
- _WebLaunchPad.Api_ contains nothing but API stuff.
- _WebLaunchPad.Communication_ handles the conversion from fancy DTOs
  to raw bytes which are then sent to the devices. It is only called by
  the API.
- _WebLaunchPad.Images_ handles all the image processing. Uses 
  [SkiaSharp](https://www.nuget.org/packages/SkiaSharp).

## Hosting
This project is supposed to run on a spare Raspberry Pi so it will be limited
to run on Linux. Because on Linux MIDI devices are basically just files that
we can write data to. Therefore we don't need any third party packages.

This also means that Docker will may not be supported.

## License
MIT